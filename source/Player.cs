using System;

namespace FlippoIO
{
	public abstract class Player
	{
		public abstract String GetName();
		public abstract PlayerInstance GetInstance(Match match);

		private int score = 0;
		public int Score => score;
		public void AddScore(int score) => this.score += score;
		public void ClearScore() => score = 0;
	}

	public interface PlayerInstance
	{
		void Start();
		void Quit();
		void Designate(bool white);
		void SendMove(Move move);
		Move GetMove();
	}
}
