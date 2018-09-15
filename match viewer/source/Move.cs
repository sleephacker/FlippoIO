using System;

namespace FlippoMatchViewer
{
	public struct Move
	{
		public readonly bool white;
		public readonly int x;
		public readonly int y;

		public Move(String[] args)
		{
			if(args.Length != 3) throw new Exception("Incorrect number of arguments");
			white = bool.Parse(args[0]);
			x = int.Parse(args[1]);
			y = int.Parse(args[2]);
		}
	}
}