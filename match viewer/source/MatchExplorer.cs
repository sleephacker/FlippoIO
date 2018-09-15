﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace FlippoMatchViewer // TODO: use ENTER key and double click instead of "Open" button
{
	public static class MatchExplorer
	{
		public static readonly String LogDirectory = "logs";

		public static void Refresh(ListBox matchList)
		{
			List<MatchItem> matches = new List<MatchItem>();
			String[] matchPaths = Directory.GetDirectories(LogDirectory);
			foreach(String path in matchPaths)
				matches.Add(new MatchItem { MatchName = path.Split('\\').Last() });
			matchList.ItemsSource = matches;
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
