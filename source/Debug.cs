using System;

namespace FlippoIO
{
	public static class Debug
	{
		public static void WriteLine(object value) => WriteLine(value.ToString());
		public static void WriteLine(String value) => WriteLine(value.ToString(), false);
		public static void WriteLine(String value, bool important)
		{
			if(important || Settings.LogDebugInfo)
				Console.WriteLine(value);
		}
	}
}
