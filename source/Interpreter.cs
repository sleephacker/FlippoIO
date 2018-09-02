using System;
using System.Collections.Generic;
using System.IO;

namespace FlippoIO
{
	public struct Command
	{
		public delegate void CommandFunction(String[] args);

		public String keyword;
		public CommandFunction function;
	}

	public struct RepCommand
	{
		public delegate void CommandFunction(String[] args, int num);

		public String keyword;
		public CommandFunction function;
	}

	public static class Interpreter
	{
		public static readonly Command[] commandList = new Command[]
		{
			new Command { keyword = "set", function = CmdSet },
			new Command { keyword = "player", function = CmdPlayer },
			new Command { keyword = "alias", function = CmdAlias },
			new Command { keyword = "score", function = CmdScore },
			new Command { keyword = "repeat", function = CmdRepeat },
			new Command { keyword = "include", function = CmdInclude },
		};

		public static readonly RepCommand[] repList = new RepCommand[]
		{
			new RepCommand { keyword = "match", function = CmdMatch },
			new RepCommand { keyword = "dual", function = CmdDual },
			new RepCommand { keyword = "competition", function = CmdCompetition },
		};

		public static void Init()
		{
			PlayerList.Init();
			//Scheduler.Start();
		}

		public static void DoFile(String path)
		{
			using(StreamReader file = new StreamReader(File.OpenRead(path)))
			{
				while(!file.EndOfStream) DoCommand(file.ReadLine());
			}
		}

		public static void DoCommand(String command)
		{
			try
			{
				if(command.Length == 0) return; // ignore empty lines
				if(command.StartsWith("#")) return; // ignore comments
				List<String> split = Split(command);
				if(split.Count == 0) throw new Exception("Command must contain at least one word.");
				String keyword = split[0];
				split.RemoveAt(0);
				String[] args = split.ToArray();
				bool found = false;
				foreach(Command c in commandList)
					if(c.keyword.Equals(keyword))
					{
						found = true;
						c.function(args);
						break;
					}
				if(!found)
					foreach(RepCommand c in repList)
						if(c.keyword.Equals(keyword))
						{
							found = true;
							c.function(args, 1);
							break;
						}
				if(!found) throw new Exception("Command: " + keyword + " wasn't found.");
			}
			catch(Exception e)
			{
				Debug.WriteLine("An exception occured while interpreting command: " + command, true);
				Debug.WriteLine(e.Message, true);
				Debug.WriteLine(e.StackTrace, true);
			}
		}

		public static List<String> Split(String str)
		{
			String[] simpleSplit = str.Split(' ');
			String tmp = null;
			bool open = false;
			List<String> result = new List<String>();
			foreach(String s in simpleSplit)
			{
				if(!open)
				{
					if(s.StartsWith("\""))
					{
						if(s.EndsWith("\""))
							result.Add(s.Substring(1, s.Length - 2));
						else
						{
							open = true;
							tmp = s.Substring(1);
						}
					}
					else if(s.EndsWith("\"")) throw new Exception("Invalid string.");
					else
					{
						result.Add(s);
					}
				}
				else
				{
					if(s.EndsWith("\""))
					{
						tmp += " " + s.Substring(0, s.Length - 1);
						result.Add(tmp);
						open = false;
					}
					else
						tmp += " " + s;
				}
			}
			if(open) throw new Exception("Invalid string.");
			return result;
		}

		public static void CmdSet(String[] args)
		{
			Scheduler.WaitForCompletion();
			Scheduler.Stop();
			Settings.Set(args);
		}

		public static void CmdPlayer(String[] args)
		{
			PlayerList.AddPlayer(args);
		}

		public static void CmdAlias(String[] args)
		{
			PlayerList.AddAlias(args);
		}

		public static void CmdScore(String[] args)
		{
			if(args.Length != 1) throw new Exception("Wrong number of arguments. Expected one argument.");
			args = args;
			switch(args[0])
			{
				case "display":
					PlayerList.DisplayScoreBoard();
					break;
				case "clear":
					PlayerList.ClearScoreBoard();
					break;
				default:
					throw new Exception("Invalid argument: " + args[0]);
			}
		}

		public static void CmdRepeat(String[] args)
		{
			if(args.Length < 2) throw new Exception("Wrong number of arguments. The number of repetitions and a command to repeat must be specified.");
			try
			{
				int num = int.Parse(args[0]);
				String keyword = args[1];
				String[] cmd_args = new String[args.Length - 2];
				Array.Copy(args, 2, cmd_args, 0, cmd_args.Length);
				bool found = false;
				foreach(RepCommand c in repList)
					if(c.keyword.Equals(keyword))
					{
						found = true;
						c.function(cmd_args, num);
						break;
					}
				if(!found) throw new Exception("Command: " + keyword + " wasn't found.");
			}
			catch(Exception e)
			{
				Debug.WriteLine("An exception occured while interpreting command: rep " + String.Join(" ", args), true);
				Debug.WriteLine(e.Message, true);
				Debug.WriteLine(e.StackTrace, true);
			}
		}

		public static void CmdInclude(String[] args)
		{
			if(args.Length != 1) throw new Exception("Wrong number of arguments. A single filename should be supplied.");
			DoFile(args[0]);
		}

		public static void CmdMatch(String[] args, int num)
		{
			if(args.Length != 2) throw new Exception("Wrong number of arguments. A match requires two players.");
			for(int n = 0; n < num; n++)
				Scheduler.ScheduleMatch(new Match(PlayerList.GetPlayer(args[0]), PlayerList.GetPlayer(args[1])));
		}

		public static void CmdDual(String[] args, int num)
		{
			if(args.Length != 2) throw new Exception("Wrong number of arguments. A match requires two players.");
			for(int n = 0; n < num; n++)
			{
				Scheduler.ScheduleMatch(new Match(PlayerList.GetPlayer(args[0]), PlayerList.GetPlayer(args[1])));
				Scheduler.ScheduleMatch(new Match(PlayerList.GetPlayer(args[1]), PlayerList.GetPlayer(args[0])));
			}
		}

		public static void CmdCompetition(String[] args, int num)
		{
			if(args.Length < 2) throw new Exception("Wrong number of arguments. A competition requires at least two players.");
			Player[] players = new Player[args.Length];
			for(int i = 0; i < args.Length; i++)
				players[i] = PlayerList.GetPlayer(args[i]);
			foreach(Player white in players)
				foreach(Player black in players)
					if(white != black)
						for(int n = 0; n < num; n++)
							Scheduler.ScheduleMatch(new Match(white, black));
		}
	}
}
