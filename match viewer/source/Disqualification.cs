using System;
using System.Globalization;
using System.IO;

namespace FlippoMatchViewer
{
	public struct Disqualification
	{
		public readonly bool disqualified;
		public readonly int move;
		public readonly String reason;

		public static Disqualification GetNone()
		{
			return new Disqualification(false, -1, null);
		}

		public Disqualification(String[] args, StreamReader file)
		{
			if(args.Length != 2) throw new Exception("Incorrect number of arguments for Disqualification");
			disqualified = true;
			move = int.Parse(args[0], CultureInfo.InvariantCulture);
			int lines = int.Parse(args[1], CultureInfo.InvariantCulture);
			reason = "";
			for(int l = 0; l < lines; l++)
			{
				reason += file.ReadLine();
			}
		}

		public Disqualification(bool disqualified, int move, String reason)
		{
			this.disqualified = disqualified;
			this.move = move;
			this.reason = reason;
		}
	}
}
