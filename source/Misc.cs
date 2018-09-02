using System;
using System.Collections.Generic;

namespace FlippoIO
{
	public static class Misc
	{
		public static IEnumerable<String> BoardToString(Board board)
		{
			for(int x = 0; x < 8; x++)
			{
				String line = "";
				for(int y = 0; y < 8; y++)
					line += board[x, y] + " ";
				yield return line.Substring(0, line.Length - 1); // remove last space
			}
		}

		public static String GetCaiaString(int x, int y) => (char)(x + 'A') + "" + (y + 1);

		public static int[] ExtractCaiaString(String str)
		{
			try
			{
				int x = str[0] - 'A';
				int y = int.Parse(str.Substring(1)) - 1;
				return new int[] { x, y };
			}
			catch
			{
				throw new InvalidMove(str);
			}
		}
	}
}
