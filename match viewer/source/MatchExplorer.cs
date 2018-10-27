using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FlippoMatchViewer // TODO: use ENTER key and double click instead of "Open" button
{
	public static class MatchExplorer
	{
		public static readonly String LogDirectory = "logs";

		public static void Refresh(ListBox matchList)
		{
			try
			{
				List<MatchItem> matches = new List<MatchItem>();
				String[] matchPaths = Directory.GetDirectories(LogDirectory);
				foreach(String path in matchPaths)
					matches.Add(new MatchItem { MatchName = path.Split('\\').Last() });
				matchList.ItemsSource = matches;
			}
			catch(DirectoryNotFoundException)
			{
				matchList.ItemsSource = new List<MatchItem>(); // No directory = no logs to display
			}
		}

		public static void OpenMatch(String matchName)
		{
			new Match(LogDirectory + "\\" + matchName);
		}
	}

	public class MatchItem
	{
		public String MatchName { get; set; }
	}
}
