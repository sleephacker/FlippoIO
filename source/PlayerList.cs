using System;
using System.Collections.Generic;

namespace FlippoIO
{
	public struct Alias
	{
		public String alias;
		public String fullName;
	}

	public static class PlayerList
	{
		public static List<Player> players = new List<Player>();
		public static List<Alias> aliases = new List<Alias>();

		public static void Init()
		{
			players.Add(new RandomPlayer());
			aliases.Add(new Alias { alias = "Random", fullName = "Built-in Random Player" });
		}

		public static void AddPlayer(String[] args)
		{
			if(args.Length != 2) throw new Exception("Wrong number of arguments. A name and path must be specified.");
			Player newPlayer = new ExePlayer(args[1], args[0]);
			players.Add(newPlayer);
			Console.WriteLine("Added player " + args[0] + ", located at: " + args[1]);
		}

		public static void AddAlias(String[] args)
		{
			if(args.Length != 2) throw new Exception("Wrong number of arguments. An alias and full name must be specified.");
			Player player = GetPlayer(args[1]);
			aliases.Add(new Alias { alias = args[0], fullName = player.GetName() });
			Console.WriteLine("Added alias: " + args[0] + " = " + player.GetName());
		}

		public static Player GetPlayer(String name)
		{
			String aliasName = GetAlias(name);
			if(aliasName != null) name = aliasName;
			foreach(Player p in players)
				if(p.GetName().Equals(name))
					return p;
			throw new Exception("Player: " + name + " not found.");
		}

		public static String GetAlias(String name)
		{
			foreach(Alias a in aliases)
				if(a.alias == name)
					return a.fullName;
			return null;
		}

		public static void DisplayScoreBoard()
		{
			Scheduler.WaitForCompletion();

			players.Sort((Player x, Player y) => y.Score - x.Score);
			int maxLength = 0;
			foreach(Player p in players)
				if(p.GetName().Length > maxLength)
					maxLength = p.GetName().Length;
			maxLength += 4; // allows for up to 99 players
			int n = 1;
			foreach(Player p in players)
			{
				String line = (n++) + ". " + p.GetName();
				int padding = maxLength - line.Length;
				for(int i = 0; i < padding; i++) line += " ";
				line += " | " + p.Score;
				Console.WriteLine(line);
			}
		}

		public static void ClearScoreBoard()
		{
			Scheduler.WaitForCompletion();
			foreach(Player p in players) p.ClearScore();
		}
	}
}
