using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace FlippoMatchViewer
{
	public class Board
	{
		private Tile[,] tiles = new Tile[8, 8];
		public Tile this[int x, int y] => tiles[x, y]; // read-only public indexer

		public Board(StreamReader file)
		{
			for(int x = 0; x < 8; x++)
			{
				String line = file.ReadLine();
				int count = line.Count((c) => c == 'W' || c == 'B' || c == '.');
				if(count != 8 || line.Length != 8)
					throw new Exception("Invalid board format.");
				for(int y = 0; y < 8; y++)
					tiles[x, y] = new Tile(line[y] != '.', line[y] == 'W');
			}
		}

		public void Render(DrawingContext context, Size size)
		{
			Pen black = new Pen(Brushes.Black, 0.5);
			Pen gray = new Pen(Brushes.Gray, 1);
			Brush background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
			double scale = Math.Min(size.Width, size.Height);
			double unit = scale / 8;

			context.DrawRectangle(background, null, new Rect(0, 0, scale, scale));
			for(int y = 1; y < 8; y++)
				context.DrawLine(black, new Point(y * unit, 0), new Point(y * unit, scale));
			for(int x = 1; x < 8; x++)
				context.DrawLine(black, new Point(0, x * unit), new Point(scale, x * unit));

			for(int x = 0; x < 8; x++)
				for(int y = 0; y < 8; y++)
					if(tiles[x, y].occupied)
					{
						Point center = new Point(y * unit + 0.5 * unit, x * unit + 0.5 * unit);
						double radius = (0.8 * unit) / 2;
						context.DrawEllipse(tiles[x, y].white ? Brushes.White : Brushes.Black, gray, center, radius, radius);
					}
		}
	}
}
