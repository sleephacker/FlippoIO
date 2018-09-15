using System;
using System.IO;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;
using System.Threading;
using System.Globalization;

namespace FlippoMatchViewer
{
	public class Match
	{
		// match data
		private String matchName;
		private String whiteName;
		private String blackName;
		private int whiteScore;
		private int blackScore;
		private bool scoresAreKnown = false;
		private double whiteTime = double.NaN;
		private double blackTime = double.NaN;
		private Disqualification whiteDisqualification = Disqualification.GetNone();
		private Disqualification blackDisqualification = Disqualification.GetNone();
		private List<Board> boards = new List<Board>();
		private List<Move> moves = new List<Move>();

		private String whiteLog;
		private String blackLog;

		// viewing state
		private int turn = 0;
		private MatchWindow window;
		private Thread playThread;
		private bool supressPause = false;

		public Match(String path)
		{
			try
			{
				using(StreamReader file = new StreamReader(File.OpenRead(path + "\\match")))
				{
					while(!file.EndOfStream)
					{
						String[] line = file.ReadLine().Split(':');
						if(line.Length > 2)
							line = new String[] { line[0], String.Join(":", line.Where((s, i) => i > 0)) };
						switch(line[0])
						{
							case "Name":
								if(line.Length != 2) throw new Exception("Missing arguments for Name");
								matchName = line[1];
								break;
							case "White":
								if(line.Length != 2) throw new Exception("Missing arguments for White");
								whiteName = line[1];
								break;
							case "Black":
								if(line.Length != 2) throw new Exception("Missing arguments for Black");
								blackName = line[1];
								break;
							case "Disqualification":
								{
									if(line.Length != 2) throw new Exception("Missing arguments for Disqualification");
									String[] args = line[1].Split(',');
									bool white = bool.Parse(args[0]);
									Disqualification d = new Disqualification(args.Where((s, i) => i > 0).ToArray(), file);
									if(white) whiteDisqualification = d;
									else blackDisqualification = d;
									break;
								}
							case "Board":
								boards.Add(new Board(file));
								break;
							case "Move":
								if(line.Length != 2) throw new Exception("Missing arguments for Move");
								moves.Add(new Move(line[1].Split(',')));
								break;
							case "Score":
								{
									if(line.Length != 2) throw new Exception("Missing arguments for Score");
									String[] args = line[1].Split(',');
									if(args.Length != 2) throw new Exception("Incorrect number of arguments for Score");
									whiteScore = int.Parse(args[0], CultureInfo.InvariantCulture);
									blackScore = int.Parse(args[1], CultureInfo.InvariantCulture);
									scoresAreKnown = true;
									break;
								}
							case "Time":
								{
									if(line.Length != 2) throw new Exception("Missing arguments for Time");
									String[] args = line[1].Split(',');
									if(args.Length != 2) throw new Exception("Incorrect number of arguments for Time");
									bool white = bool.Parse(args[0]);
									double time = double.Parse(args[1], CultureInfo.InvariantCulture);
									if(white) whiteTime = time;
									else blackTime = time;
									break;
								}
							default:
								{
									String original = String.Join(":", line);
									if(original.Length > 0)
										throw new Exception("Unrecognized line: " + original);
									break;
								}
						}
					}
					if(matchName == null) throw new Exception("Failed to reconstruct the match from its file, match name is missing.");
					if(whiteName == null) throw new Exception("Failed to reconstruct the match from its file, name of white player is missing.");
					if(blackName == null) throw new Exception("Failed to reconstruct the match from its file, name of black player is missing.");
					if(!scoresAreKnown) throw new Exception("Failed to reconstruct the match from its file, scores are missing.");
					if(boards.Count != 61) throw new Exception("Failed to reconstruct the match from its file, incorrect number of boards.");
					if(moves.Count != 60) throw new Exception("Failed to reconstruct the match from its file, incorrect number of moves.");
				}

				if(File.Exists(path + "\\white.txt"))
					using(StreamReader file = new StreamReader(File.OpenRead(path + "\\white.txt"))) whiteLog = file.ReadToEnd();
				if(File.Exists(path + "\\black.txt"))
					using(StreamReader file = new StreamReader(File.OpenRead(path + "\\black.txt"))) blackLog = file.ReadToEnd();

				// Match has been loaded succesfully, display window
				window = new MatchWindow();
				window.Title = matchName;
				window.BoardCanvas.renderer = RenderBoard;
				window.PlayButton.Click += PlayButton_Click;
				window.ForwardButton.Click += ForwardButton_Click;
				window.BackButton.Click += BackButton_Click;
				window.MoveList.SelectionChanged += MoveList_SelectionChanged;
				window.ScoreTextBlock.Text = whiteScore + " - " + blackScore;

				String matchLog = "";
				if(whiteDisqualification.disqualified)
					matchLog += whiteName + " was disqualified in move " + (whiteDisqualification.move + 1) + ", reason for disqualification:" + Environment.NewLine
						+ whiteDisqualification.reason + Environment.NewLine;
				if(blackDisqualification.disqualified)
					matchLog += blackName + " was disqualified in move " + (blackDisqualification.move + 1) + ", reason for disqualification:" + Environment.NewLine
						+ blackDisqualification.reason + Environment.NewLine;
				if(!double.IsNaN(whiteTime))
					matchLog += whiteName + " used " + String.Format("{0:0.0}", whiteTime) + " ms." + Environment.NewLine;
				if(!double.IsNaN(blackTime))
					matchLog += blackName + " used " + String.Format("{0:0.0}", blackTime) + " ms." + Environment.NewLine;
				window.MatchLog.Text = matchLog;
				if(whiteLog != null)
				{
					window.WhiteLog.Text = whiteLog;
					window.WhiteTab.Header = whiteName;
					window.WhiteTab.Visibility = Visibility.Visible;
				}
				if(blackLog != null)
				{
					window.BlackLog.Text = blackLog;
					window.BlackTab.Header = blackName;
					window.BlackTab.Visibility = Visibility.Visible;
				}

				List<MoveItem> moveItems = new List<MoveItem>();
				moveItems.Add(new MoveItem { MoveText = "0: Start" });
				for(int m = 0; m < moves.Count; m++)
					moveItems.Add(new MoveItem { MoveText = (m + 1) + ": " + (char)(moves[m].x + 'A') + (moves[m].y + 1) });
				window.MoveList.ItemsSource = moveItems;

				UpdateMoves();
				UpdateNotification();
				window.Show();
			}
			catch(Exception e)
			{
				MessageBox.Show("An exception occured while trying to read the match file:" + Environment.NewLine
					+ e.Message + Environment.NewLine
					+ e.StackTrace, "Exception");
			}
		}

		private void RenderBoard(DrawingContext context, Size size) => boards[turn].Render(context, size);

		private void UpdateNotification()
		{
			if(whiteDisqualification.disqualified && (whiteDisqualification.move + 1) == turn)
			{
				window.NotificationTextBlock.Text = whiteName + " was disqualified";
				window.NotificationBorder.Visibility = Visibility.Visible;
			}
			else if(blackDisqualification.disqualified && (blackDisqualification.move + 1) == turn)
			{
				window.NotificationTextBlock.Text = blackName + " was disqualified";
				window.NotificationBorder.Visibility = Visibility.Visible;
			}
			else
				window.NotificationBorder.Visibility = Visibility.Hidden;
		}

		private void UpdateMoves() => window.MoveList.SelectedIndex = turn;

		private void Play()
		{
			if(playThread != null)
				playThread.Abort();
			(playThread = new Thread(new ThreadStart(PlayThread))).Start();
			window.PlayButton.Content = "Pause";
		}

		private void Pause()
		{
			if(supressPause) return;
			if(playThread != null)
				playThread.Abort();
			playThread = null;
			window.PlayButton.Content = "Play";
		}

		private void PlayThread()
		{
			while(turn < 60)
			{
				turn++;
				supressPause = true;
				window.Dispatcher.Invoke(() =>
				{
					window.BoardCanvas.InvalidateVisual();
					UpdateMoves();
					UpdateNotification();
				});
				supressPause = false;
				Thread.Sleep(500);
			}
			playThread = null;
			window.Dispatcher.Invoke(() =>
			{
				window.PlayButton.Content = "Play";
			});
		}

		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			Pause();
			if(turn > 0) turn--;
			window.Dispatcher.Invoke(() =>
			{
				window.BoardCanvas.InvalidateVisual();
				UpdateMoves();
				UpdateNotification();
			});
		}

		private void PlayButton_Click(object sender, RoutedEventArgs e)
		{
			if(playThread == null)
				Play();
			else
				Pause();
		}

		private void ForwardButton_Click(object sender, RoutedEventArgs e)
		{
			Pause();
			if(turn < 60) turn++;
			window.Dispatcher.Invoke(() =>
			{
				window.BoardCanvas.InvalidateVisual();
				UpdateMoves();
				UpdateNotification();
			});
		}

		private void MoveList_SelectionChanged(object sender, RoutedEventArgs e)
		{
			Pause();
			turn = window.MoveList.SelectedIndex;
			window.BoardCanvas.InvalidateVisual();
			UpdateMoves();
			UpdateNotification();
		}
	}

	public class MoveItem
	{
		public String MoveText { get; set; }
	}
}
