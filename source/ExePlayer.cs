using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace FlippoIO
{
	public class ExePlayer : Player
	{
		public readonly String path;
		public readonly String name;

		public ExePlayer(String path)
		{
			if(!File.Exists(path)) throw new Exception("The specified player file doesn't exist.");
			this.path = path;
			name = path;
		}

		public ExePlayer(String path, String name)
		{
			if(!File.Exists(path)) throw new Exception("The specified player file doesn't exist.");
			this.path = path;
			this.name = name;
		}

		public override string GetName() => name;

		public override PlayerInstance GetInstance(Match match) => new ExePlayerInstance(this, match);
	}

	public class ExePlayerInstance : PlayerInstance
	{
		private ExePlayer player;
		private Match match;
		private bool white;
		private String cmd;
		private String args;
		private Process program;
		private Stopwatch timer;
		private AsyncThread errorThread;
		private EventWaitHandle outputDone = new EventWaitHandle(false, EventResetMode.ManualReset);
		private String stdError;
		public String PlayerLog => stdError;

		public ExePlayerInstance(ExePlayer player, Match match)
		{
			this.player = player;
			this.match = match;
			if(player.path.EndsWith(".jar"))
			{
				cmd = Settings.JavaCmd;
				args = "-jar " + player.path;
			}
			else if(player.path.EndsWith(".py"))
			{
				cmd = Settings.PythonCmd;
				args = player.path;
			}
			else
			{
				cmd = player.path;
				args = null;
			}
		}

		private void WakeUp()
		{
			if(!Settings.SuspendPlayers) return;

			foreach(ProcessThread pT in program.Threads)
			{
				IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);
				if(pOpenThread == IntPtr.Zero)
					continue;
				int suspendCount = 0;
				do
					suspendCount = ResumeThread(pOpenThread);
				while(suspendCount > 0);
				CloseHandle(pOpenThread);
				if(suspendCount < 0)
					throw new Exception("Failed to resume a thread in program " + player.name);
			}
		}

		private void Sleep()
		{
			if(!Settings.SuspendPlayers) return;

			foreach(ProcessThread pT in program.Threads)
			{
				IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);
				if(pOpenThread == IntPtr.Zero)
					continue;
				SuspendThread(pOpenThread);
				CloseHandle(pOpenThread);
			}
		}

		public void Start()
		{
			Stopwatch startTime = Stopwatch.StartNew();

			ProcessStartInfo startInfo = new ProcessStartInfo(cmd)
			{
				Arguments = args,
				UseShellExecute = false, // needs to be false to enable stream redirecting
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true
			};
			program = Process.Start(startInfo);
			timer = new Stopwatch();
			Sleep();
			
			program.Exited += new EventHandler((object src, EventArgs args) => { outputDone.Set(); }); // ensure the main match thread will wake up if the program crashes.
			errorThread = AsyncThread.Borrow(ReadError);

			startTime.Stop();
			Debug.WriteLine(match.Prefix + "Started player " + player.name + " in " + startTime.ElapsedMilliseconds + " ms. Used command: " + cmd + " " + args);
		}

		private void Terminate() // TODO: access is denied sometimes...
		{
			bool success = false;
			try
			{
				program.Kill();
				success = true;
			}
			catch(InvalidOperationException e) // this could mean the program has already exited
			{
				if(!program.HasExited) throw e;
			}
			catch(Exception e)
			{
				Debug.WriteLine(match.Prefix + "Exception occured while trying to kill " + player.name);
				Debug.WriteLine(e.Message);
				Debug.WriteLine(e.StackTrace);
			}

			if(success) Debug.WriteLine(match.Prefix + "Terminated player " + player.name + ".");
		}

		private void SendLine(String line)
		{
			program.StandardInput.WriteLine(line);
			program.StandardInput.Flush();
		}

		private String ReadLine()
		{
			WakeUp();
			timer.Start();
			String line = program.StandardOutput.ReadLine();
			timer.Stop();
			Sleep();
			if(program.HasExited) throw new Crash();
			if(timer.ElapsedMilliseconds > Settings.TimeLimit) throw new Timeout();
			return line;
		}

		private String ReadLineAsync()
		{
			String line = null;
			AsyncThread thread = AsyncThread.Borrow(() =>
			{
				line = program.StandardOutput.ReadLine();
				outputDone.Set();
			});
			WakeUp();
			timer.Start();
			while(timer.ElapsedMilliseconds <= Settings.TimeLimit)
			{
				bool success = outputDone.WaitOne((int)(Settings.TimeLimit + Settings.ReadTimeMargin - timer.ElapsedMilliseconds));
				if(success) break; // success may also be true when the program crashes, see Start()
			}
			timer.Stop();
			Sleep();
			outputDone.Reset();
			AsyncThread.Return(thread, line != null);
			if(program.HasExited) throw new Crash();
			if(timer.ElapsedMilliseconds > Settings.TimeLimit) throw new Timeout();
			return line;
		}

		private void ReadError() // TODO: using ReadToEnd() causes logs to be lost...
		{
			//stdError = program.StandardError.ReadToEnd();
			while(!program.StandardError.EndOfStream)
				stdError += program.StandardError.ReadLine() + Environment.NewLine;
		}

		public void Designate(bool white)
		{
			this.white = white;
			if(white)
				SendLine("Start");
		}

		public Move GetMove()
		{
			String line = Settings.AsyncRead ? ReadLineAsync() : ReadLine();
			int[] xy = Misc.ExtractCaiaString(line); // will throw the appropriate exception if the line sent wasn't a valid move
			return new Move(xy[0], xy[1], white);
		}

		public void SendMove(Move move)
		{
			SendLine(Misc.GetCaiaString(move.x, move.y));
		}

		public void Quit()
		{
			try
			{
				Debug.WriteLine(match.Prefix + "Player " + player.name + " used " + timer.ElapsedMilliseconds + " ms.");
				if(!program.HasExited)
				{
					SendLine("Quit");
					WakeUp();
					if(Settings.KillPrograms)
					{
						Thread.Sleep(Settings.MillisecondsBeforeKill);
						Terminate();
					}
					program.WaitForExit();
				}
			}
			finally
			{
				AsyncThread.Return(errorThread, program.HasExited);
			}
		}

		// Win32 stuff
		[Flags]
		public enum ThreadAccess : int
		{
			TERMINATE = (0x0001),
			SUSPEND_RESUME = (0x0002),
			GET_CONTEXT = (0x0008),
			SET_CONTEXT = (0x0010),
			SET_INFORMATION = (0x0020),
			QUERY_INFORMATION = (0x0040),
			SET_THREAD_TOKEN = (0x0080),
			IMPERSONATE = (0x0100),
			DIRECT_IMPERSONATION = (0x0200)
		}

		[DllImport("kernel32.dll")]
		static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
		[DllImport("kernel32.dll")]
		static extern uint SuspendThread(IntPtr hThread);
		[DllImport("kernel32.dll")]
		static extern int ResumeThread(IntPtr hThread);
		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		static extern bool CloseHandle(IntPtr handle);
	}
}
