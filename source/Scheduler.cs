using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace FlippoIO
{
	public static class Scheduler // TODO: a deadlock on schedulLock occured once, it is not known if this can still occur
	{
		private static List<Match> schedule = new List<Match>();
		private static int matchesScheduled = 0;
		private static int matchesCompleted = 0;
		private static EventWaitHandle scheduleDone = new EventWaitHandle(true, EventResetMode.ManualReset);
		private static object scheduleLock = new object();
		private static List<Thread> threads = new List<Thread>();
		private static EventWaitHandle matchesAvailable = new EventWaitHandle(false, EventResetMode.ManualReset);
		private static bool isRunning = false;

		public static void Stop()
		{
			if(!isRunning) return;
			lock(scheduleLock)
				if(schedule.Count != 0) throw new Exception("Schedule must be empty before attempting to stop.");
			foreach(Thread t in threads)
				t.Abort();
			matchesAvailable.Reset();
			AsyncThread.KillAll();
			matchesScheduled = matchesCompleted = 0;
			scheduleDone.Set();
			isRunning = false;
		}

		public static void Start()
		{
			for(int t = 0; t < Settings.ThreadCount; t++)
			{
				Thread thread = new Thread(new ThreadStart(MatchThread));
				thread.Start();
				threads.Add(thread);
			}
			isRunning = true;
		}

		public static void ScheduleMatch(Match match)
		{
			if(!isRunning) Start();
			lock(scheduleLock)
			{
				schedule.Add(match);
				matchesScheduled++;
				matchesAvailable.Set();
				scheduleDone.Reset();
			}
		}

		public static void WaitForCompletion()
		{
			scheduleDone.WaitOne();
		}

		private static void MatchThread()
		{
			Random random = new Random();
			while(true)
			{
				try
				{
					Match match = null;
					lock(scheduleLock)
					{
						if(schedule.Count > 0)
						{
							match = schedule[0];
							schedule.RemoveAt(0);
						}
						else
							matchesAvailable.Reset();
					}
					if(match != null) // TODO: this sometimes gets stuck when an exception occurs in match.Save()
					{
						try
						{
							match.RunMatch();
							match.white.AddScore(match.ScoreWhite);
							match.black.AddScore(match.ScoreBlack);
							match.Save();
						}
						finally
						{
							lock(scheduleLock)
								if(++matchesCompleted == matchesScheduled)
									scheduleDone.Set();
						}
					}
					else
						matchesAvailable.WaitOne();
				}
				catch(ThreadAbortException e) { break; }
				catch(Exception e)
				{
					Debug.WriteLine("Exception occured in scheduler thread:", true);
					Debug.WriteLine(e.Message, true);
					Debug.WriteLine(e.StackTrace, true);
					break;
				}
			}
		}
	}
}
