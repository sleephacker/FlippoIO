namespace FlippoIO
{
	public struct Move
	{
		public readonly int x;
		public readonly int y;
		public readonly bool white;

		public Move(int x, int y, bool white)
		{
			this.x = x;
			this.y = y;
			this.white = white;
		}

		public override string ToString()
		{
			return Misc.GetCaiaString(x, y);
		}
	}
}
