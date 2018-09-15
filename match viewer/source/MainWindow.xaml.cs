using System;
using System.Windows;
using System.Windows.Controls;

namespace FlippoMatchViewer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			MatchExplorer.Refresh(MatchList);
		}

		public void OpenMatchClicked(object sender, RoutedEventArgs args)
		{
			MatchExplorer.OpenMatch((sender as Button).Tag as String);
		}

		private void RefreshClicked(object sender, RoutedEventArgs e)
		{
			MatchExplorer.Refresh(MatchList);
		}
	}
}
