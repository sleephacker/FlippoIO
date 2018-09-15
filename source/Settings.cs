using System;

namespace FlippoIO
{
	public static class Settings
	{
		public static bool LogDebugInfo = false;
		public static bool SuspendPlayers = false; // this is necessary for the most accurate time measurement, but also slows down matches a lot.
		public static bool AsyncRead = true;
		public static bool KillPrograms = true;
		public static bool NameMatchByPlayers = true;
		public static bool SavePlayerLogs = true;
		public static bool SaveMatchAsFile = true;
		public static bool SaveMatchAsTXT = false;
		public static int TimeLimit = 5000;
		public static int ReadTimeMargin = 200;
		public static int MillisecondsBeforeKill = 0;
		public static int ThreadCount = Environment.ProcessorCount;
		public static String JavaCmd = "java";
		public static String PythonCmd = "py";
		public static String JavaScriptCmd = "Cscript";
		public static char WhiteChar = 'X';
		public static char BlackChar = 'O';
		public static char EmptyChar = '.';

		public static void Set(String[] args)
		{
			if(args.Length != 2)
				throw new Exception("Wrong number of arguments, all settings require two arguments.");
			else
			{
				switch(args[0])
				{
					case "LogDebugInfo":
						LogDebugInfo = bool.Parse(args[1]);
						break;
					case "SuspendPlayers":
						SuspendPlayers = bool.Parse(args[1]);
						break;
					case "AsyncRead":
						AsyncRead = bool.Parse(args[1]);
						break;
					case "KillPrograms":
						KillPrograms = bool.Parse(args[1]);
						break;
					case "NameMatchByPlayers":
						NameMatchByPlayers = bool.Parse(args[1]);
						break;
					case "SavePlayerLogs":
						SavePlayerLogs = bool.Parse(args[1]);
						break;
					case "SaveMatchAsFile":
						SaveMatchAsFile = bool.Parse(args[1]);
						break;
					case "SaveMatchAsTXT":
						SaveMatchAsTXT = bool.Parse(args[1]);
						break;
					case "TimeLimit":
						TimeLimit = int.Parse(args[1]);
						break;
					case "ReadTimeMargin":
						ReadTimeMargin = int.Parse(args[1]);
						break;
					case "MillisecondsBeforeKill":
						MillisecondsBeforeKill = int.Parse(args[1]);
						break;
					case "ThreadCount":
						ThreadCount = int.Parse(args[1]);
						break;
					case "JavaCmd":
						JavaCmd = args[1];
						break;
					case "PythonCmd":
						PythonCmd = args[1];
						break;
					case "JavaScriptCmd":
						JavaScriptCmd = args[1];
						break;
					case "WhiteChar":
						WhiteChar = char.Parse(args[1]);
						break;
					case "BlackChar":
						BlackChar = char.Parse(args[1]);
						break;
					case "EmptyChar":
						EmptyChar = char.Parse(args[1]);
						break;
					default:
						throw new Exception("The specified setting doesn't exist.");
				}
				// no exception was thrown: the setting was changed successfully.
				Debug.WriteLine("Changed setting: " + args[0] + " = " + args[1]);
			}
		}
	}
}
