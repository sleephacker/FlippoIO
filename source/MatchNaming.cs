using System.Collections.Generic;

namespace FlippoIO
{
	public class PairNumber
	{
		public Player white;
		public Player black;
		public int number;

		public PairNumber(Player white, Player black)
		{
			this.white = white;
			this.black = black;
			number = 1;
		}
	}

	public static class MatchNaming
	{
		private static int genericNumber = 1;
		private static List<PairNumber> numbers = new List<PairNumber>();
		private static object numbersLock = new object();

		public static string GetName(Player white, Player black)
		{
			if(Settings.NameMatchByPlayers) return white.GetName() + " VS " + black.GetName() + " #" + GetNumber(white, black);
			else return "Match #" + (genericNumber++);
		}

		private static int GetNumber(Player white, Player black)
		{
			PairNumber number = null;
			foreach(PairNumber n in numbers)
				if(n.white == white && n.black == black)
					number = n;
			if(number == null)
				numbers.Add(number = new PairNumber(white, black));
			return number.number++;
		}
	}
}
