using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlippoMatchViewer
{
	/// <summary>
	/// Interaction logic for CustomCanvas.xaml
	/// </summary>
	public partial class CustomCanvas : UserControl
	{
		public delegate void Renderer(DrawingContext context, Size size);

		public Renderer renderer;

		public CustomCanvas()
		{
			InitializeComponent();
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			if(renderer != null) renderer(drawingContext, RenderSize);
			else
			{
				Brush b = Brushes.Magenta;
				Pen p = new Pen(Brushes.Lime, 1);
				drawingContext.DrawRectangle(b, p, new Rect(new Point(0, 0), RenderSize));
			}
		}
	}
}
