using System;
using System.Collections.Generic;

namespace FlippoIO
{
	public class RandomPlayer : Player
	{
		public override String GetName() => "Built-in Random Player";
		public override PlayerInstance GetInstance(Match match) => new RandomPlayerInstance();
	}

	class RandomPlayerInstance : PlayerInstance
	{
		private bool white;
		private Board board;
		private Random random = new Random();

		public RandomPlayerInstance() { }

		public RandomPlayerInstance(Board board, bool white)
		{
			this.board = board;
			this.white = white;
		}

		public void Designate(bool white) => this.white = white;

		public void Quit() => board = null;

		public void SendMove(Move move) => board = board.GetBoard(move);

		public void Start() => board = new Board();

		public Move GetMove()
		{
			List<Move> possibleMoves = new List<Move>();
			for(int x = 0; x < 8; x++)
				for(int y = 0; y < 8; y++)
					if(!board[x, y].occupied)
					{
						Move m = new Move(x, y, white);
						if(Referee.IsLegal(m, board)) possibleMoves.Add(m);
					}
			Move move = possibleMoves[random.Next(possibleMoves.Count)];
			SendMove(move);
			return move;
		}
	}
}
