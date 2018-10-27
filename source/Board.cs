using System;

namespace FlippoIO
{
	public class Board
	{
		private Tile[,] tiles = new Tile[8, 8];
		public Tile this[int x, int y] => tiles[x, y]; // read-only public indexer

		public int CountColor(bool white)
		{
			int n = 0;
			foreach(Tile t in tiles)
				if(t.occupied && (t.white == white))
					n++;
			return n;
		}

		public Board()
		{
			for(int x = 0; x < 8; x++)
				for(int y = 0; y < 8; y++)
					if(x < 3 || x > 4 || y < 3 || y > 4)
						tiles[x, y] = new Tile(false, false);
					else
						tiles[x, y] = new Tile(true, (x % 2) == (y % 2)); // fill center with chess pattern
		}

		public Board GetBoard(Move move) // gets the resulting board of a move
		{
			Board result = new Board(this);
			result.tiles[move.x, move.y] = new Tile(true, move.white); // place the new stone

			throw new NotImplementedException(); // CENSORED: flip any stones that should be flipped by this move

			return result;
		}

		private Board(Board board) // copying constructor
		{
			for(int x = 0; x < 8; x++)
				for(int y = 0; y < 8; y++)
					tiles[x, y] = board.tiles[x, y];
		}

		public bool IsAttached(int x, int y)
		{
			throw new NotImplementedException(); // CENSORED: return true if the tile at x, y is attached to a non-empty tile
		}

		public bool CanFlip(Move move)
		{
			throw new NotImplementedException(); // CENSORED: return true if executing this move would cause at least one stone to be flipped
		}

		public override String ToString()
		{
			String str = "";
			for(int x = 0; x < 8; x++)
			{
				for(int y = 0; y < 8; y++)
					str += this[x, y] + " ";
				str = str.Substring(0, str.Length - 1); // remove last space
				str += Environment.NewLine;
			}
			return str;
		}

		public String ToStandardString() // Generates a string of a non-configurable format for consistent parsing
		{
			String str = "";
			for(int x = 0; x < 8; x++)
			{
				for(int y = 0; y < 8; y++)
					str += this[x, y].occupied ? this[x, y].white ? "W" : "B" : ".";
				str += Environment.NewLine;
			}
			return str;
		}
	}
}
