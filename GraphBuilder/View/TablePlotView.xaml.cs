using GraphBuilder.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace GraphBuilder.View
{
	/// <summary>
	/// Interaction logic for TablePlotView.xaml
	/// </summary>
	public partial class TablePlotView : UserControl
	{
		public TablePlotView()
		{
			InitializeComponent();
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var viewModel = (PlotViewModel)DataContext;
			viewModel.PropertyChanged += (_, __) =>
			{
				AxisX = viewModel.AxisX;
				AxisY = viewModel.AxisY;
			};
		}

		public static readonly DependencyProperty AxisXProperty =
            DependencyProperty.Register("AxisX", typeof(string), typeof(TablePlotView),
            new PropertyMetadata(GraphBuilder.Resources.AxisXText, OnAxisX));

        private static void OnAxisX(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
			var view = (TablePlotView)d;
			var viewModel = (PlotViewModel)view.DataContext;
			viewModel.AxisX = (string)e.NewValue;
		}

		public string AxisX
		{
            get { return (string)GetValue(AxisXProperty); }
            set { SetValue(AxisXProperty, value); }
        }

		public static readonly DependencyProperty AxisYProperty =
			DependencyProperty.Register("AxisY", typeof(string), typeof(TablePlotView),
			new PropertyMetadata(GraphBuilder.Resources.AxisYText, OnAxisY));

		private static void OnAxisY(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var view = (TablePlotView)d;
			var viewModel = (PlotViewModel)view.DataContext;
			viewModel.AxisY = (string)e.NewValue;
		}

		public string AxisY
		{
			get { return (string)GetValue(AxisYProperty); }
			set { SetValue(AxisYProperty, value); }
		}
	}
}
