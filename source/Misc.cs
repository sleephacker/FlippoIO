using System;

namespace FlippoIO
{
	public static class Misc
	{
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
