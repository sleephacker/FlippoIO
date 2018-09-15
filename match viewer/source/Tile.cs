namespace FlippoMatchViewer
{
	public struct Tile
	{
		public readonly bool occupied;
		public readonly bool white;

		public Tile(bool occupied, bool white)
		{
			this.occupied = occupied;
			this.white = white;
		}
	}
}
