using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlippoIO
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

		public Disqualification(bool disqualified, int move, String reason)
		{
			this.disqualified = disqualified;
			this.move = move;
			this.reason = reason;
		}
	}
}
