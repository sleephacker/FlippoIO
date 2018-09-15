using System.Windows;

namespace FlippoMatchViewer
{
	/// <summary>
	/// Interaction logic for MatchWindow.xaml
	/// </summary>
	public partial class MatchWindow : Window
	{
		public MatchWindow()
		{
			InitializeComponent();
		}

		private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
		{
			double h = MainGrid.RowDefinitions[0].ActualHeight;
			BoardGrid.RowDefinitions[0].Height = new GridLength(h);
			BoardGrid.ColumnDefinitions[0].Width = new GridLength(h);
		}
	}
}
