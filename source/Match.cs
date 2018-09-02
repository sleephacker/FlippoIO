using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace FlippoIO
{
	public class Match
	{
		public readonly String name;
		public String Prefix => "[" + name + "] ";
		public readonly Player white;
		public readonly Player black;
		private List<Move> moves = new List<Move>(); // 0 - 59
		private List<Board> boards = new List<Board>(); // 0 - 60
		public IReadOnlyList<Move> Moves => moves.AsReadOnly();
		public IReadOnlyList<Board> Boards => boards.AsReadOnly();
		private Disqualification whiteDisqualification = Disqualification.GetNone();
		private Disqualification blackDisqualification = Disqualification.GetNone();
		public Disqualification WhiteDisqualification => whiteDisqualification;
		public Disqualification BlackDisqualification => blackDisqualification;
		private String whiteLog;
		private String blackLog;
		public String WhiteLog => whiteLog;
		public String BlackLog => blackLog;
		private int scoreWhite;
		private int scoreBlack;
		public int ScoreWhite => scoreWhite;
		public int ScoreBlack => scoreBlack;

		public Match(Player white, Player black)
		{
			this.white = white;
			this.black = black;
			name = MatchNaming.GetName(white, black);
		}

		public Match(Player white, Player black, String name)
		{
			this.white = white;
			this.black = black;
			this.name = name;
		}

		public void SaveToTXT(String path)
		{
			try
			{
				Directory.CreateDirectory(path);
				String matchPath = path + "\\match.txt";
				String whitePath = path + "\\white.txt";
				String blackPath = path + "\\black.txt";
				File.Delete(matchPath);
				File.Delete(whitePath);
				File.Delete(blackPath);

				// Write match log
				using(StreamWriter file = new StreamWriter(File.OpenWrite(matchPath)))
				{
					file.WriteLine(name);
					file.WriteLine("White: " + white.GetName());
					file.WriteLine("Black: " + black.GetName());
					file.WriteLine();

					file.WriteLine("Starting Board:");
					foreach(String l in Misc.BoardToString(boards[0])) file.WriteLine(l);

					for(int t = 0; t < 60; t++)
					{
						file.WriteLine();

						if(whiteDisqualification.disqualified && whiteDisqualification.move == t)
						{
							file.WriteLine("White has been disqualified in turn " + (t + 1) + ".");
							file.WriteLine("Reason for disqualification: " + whiteDisqualification.reason);
						}
						if(blackDisqualification.disqualified && blackDisqualification.move == t)
						{
							file.WriteLine("Black has been disqualified in turn: " + (t + 1) + ".");
							file.WriteLine("Reason for disqualification: " + blackDisqualification.reason);
						}

						Move move = moves[t];
						file.WriteLine((move.white ? "White" : "Black") + " played: " + move);
						file.WriteLine("Board after turn " + (t + 1) + ":");
						foreach(String l in Misc.BoardToString(boards[t + 1])) file.WriteLine(l);
					}

					int scoreWhite = whiteDisqualification.disqualified ? 0 : boards[60].CountColor(true) - 2;
					int scoreBlack = blackDisqualification.disqualified ? 0 : boards[60].CountColor(false) - 2;
					file.WriteLine();
					file.WriteLine("Score: " + scoreWhite + " - " + scoreBlack);
					file.Close();
				}

				// Write player logs
				if(whiteLog != null) if(whiteLog.Length > 0) using(StreamWriter file = new StreamWriter(File.OpenWrite(whitePath))) file.Write(whiteLog);
				if(blackLog != null) if(blackLog.Length > 0) using(StreamWriter file = new StreamWriter(File.OpenWrite(blackPath))) file.Write(blackLog);
			}
			catch(Exception e)
			{
				Debug.WriteLine("An exception occured while attempting to save " + name + " to file: " + path, true);
				Debug.WriteLine(e.Message, true);
				Debug.WriteLine(e.StackTrace, true);
			}
			Debug.WriteLine(Prefix + "Saved match in text format to file: " + path);
		}

		public void RunMatch()
		{
			Debug.WriteLine(Prefix + "Starting match...");
			Stopwatch matchTime = Stopwatch.StartNew();

			Board board = new Board();
			boards.Add(board);

			PlayerInstance playerW = white.GetInstance(this);
			PlayerInstance playerB = black.GetInstance(this);

			playerW.Start();
			playerB.Start();

			Debug.WriteLine(Prefix + "Instantiated players...");

			playerW.Designate(true);
			playerB.Designate(false);

			for(int t = 0; t < 30; t++) // TODO: get rid of some of the duplicate code
			{
				try
				{
					Move whiteMove = playerW.GetMove();
					if(!Referee.IsLegal(whiteMove, board)) throw new IllegalMove(whiteMove);
					moves.Add(whiteMove);
					board = board.GetBoard(whiteMove);
					boards.Add(board);
					playerB.SendMove(whiteMove);
				}
				catch(DisqualificationException e)
				{
					if(e is Crash)
					{
						Debug.WriteLine(Prefix + "White crashed and has been disqualified.", true);
						whiteDisqualification = new Disqualification(true, moves.Count, "White crashed.");
					}
					else if(e is Timeout)
					{
						Debug.WriteLine(Prefix + "White used too much time and has been disqualified.", true);
						whiteDisqualification = new Disqualification(true, moves.Count, "White exceeded the time limit.");
					}
					else if(e is InvalidMove)
					{
						Debug.WriteLine(Prefix + "White sent an invalid move and has been disqualified.", true);
						whiteDisqualification = new Disqualification(true, moves.Count, "White sent an invalid move: " + (e as InvalidMove).move + ".");
					}
					else if(e is IllegalMove)
					{
						Debug.WriteLine(Prefix + "White made an illegal move and has been disqualified.", true);
						whiteDisqualification = new Disqualification(true, moves.Count, "White attempted to make an illegal move: " + (e as IllegalMove).move + ".");
					}
					playerW.Quit();
					if(playerW is ExePlayerInstance) whiteLog = (playerW as ExePlayerInstance).PlayerLog;
					playerW = new RandomPlayerInstance(board, true);
					Move whiteMove = playerW.GetMove();
					moves.Add(whiteMove);
					board = board.GetBoard(whiteMove);
					boards.Add(board);
					playerB.SendMove(whiteMove);
				}

				try
				{
					Move blackMove = playerB.GetMove();
					if(!Referee.IsLegal(blackMove, board)) throw new IllegalMove(blackMove);
					moves.Add(blackMove);
					board = board.GetBoard(blackMove);
					boards.Add(board);
					playerW.SendMove(blackMove);
				}
				catch(DisqualificationException e)
				{
					if(e is Crash)
					{
						Debug.WriteLine(Prefix + "Black crashed and has been disqualified.", true);
						blackDisqualification = new Disqualification(true, moves.Count, "Black crashed.");
					}
					else if(e is Timeout)
					{
						Debug.WriteLine(Prefix + "Black used too much time and has been disqualified.", true);
						blackDisqualification = new Disqualification(true, moves.Count, "Black exceeded the time limit.");
					}
					else if(e is InvalidMove)
					{
						Debug.WriteLine(Prefix + "Black sent an invalid move and has been disqualified.", true);
						blackDisqualification = new Disqualification(true, moves.Count, "Black sent an invalid move: " + (e as InvalidMove).move + ".");
					}
					else if(e is IllegalMove)
					{
						Debug.WriteLine(Prefix + "Black made an illegal move and has been disqualified.", true);
						blackDisqualification = new Disqualification(true, moves.Count, "Black attempted to make an illegal move: " + (e as IllegalMove).move + ".");
					}
					playerB.Quit();
					if(playerB is ExePlayerInstance) blackLog = (playerB as ExePlayerInstance).PlayerLog;
					playerB = new RandomPlayerInstance(board, false);
					Move blackMove = playerB.GetMove();
					moves.Add(blackMove);
					board = board.GetBoard(blackMove);
					boards.Add(board);
					playerW.SendMove(blackMove);
				}
			}

			playerW.Quit();
			playerB.Quit();

			if(!whiteDisqualification.disqualified) if(playerW is ExePlayerInstance) whiteLog = (playerW as ExePlayerInstance).PlayerLog;
			if(!blackDisqualification.disqualified) if(playerB is ExePlayerInstance) blackLog = (playerB as ExePlayerInstance).PlayerLog;

			matchTime.Stop();
			scoreWhite = whiteDisqualification.disqualified ? 0 : board.CountColor(true) - 2;
			scoreBlack = blackDisqualification.disqualified ? 0 : board.CountColor(false) - 2;
			Debug.WriteLine(Prefix + "Match finished in " + matchTime.ElapsedMilliseconds + " ms. Score: " + scoreWhite + " - " + scoreBlack + ".", true);
		}
	}
}
