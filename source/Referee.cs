using System;

namespace FlippoIO
{
	public class DisqualificationException : Exception { }

	public class Timeout : DisqualificationException { }

	public class Crash : DisqualificationException { }

	public class InvalidMove : DisqualificationException
	{
		public readonly String move;
		public InvalidMove(String move) => this.move = move;
	}

	public class IllegalMove : DisqualificationException
	{
		public readonly Move move;
		public IllegalMove(Move move) => this.move = move;
	}

	public static class Referee
	{
		public static bool IsLegal(Move move, Board board)
		{
			if(move.x < 0 || move.x > 7 || move.y < 0 || move.y > 7) return false;
			if(board[move.x, move.y].occupied) return false;
			if(!board.IsAttached(move.x, move.y)) return false;
			if(board.CanFlip(move)) return true;
			// This move can't flip anything, so if any other move could have flipped something, then this move is illegal.
			for(int x = 0; x < 8; x++)
				for(int y = 0; y < 8; y++)
					if(!board[x, y].occupied)
						if(board.CanFlip(new Move(x, y, move.white))) return false;
			return true;
		}
	}
}
