using OxyPlot;
using System.Windows.Controls;
using GraphBuilder.ViewModel;

namespace GraphBuilder.View
{
	/// <summary>
	/// Interaction logic for OxyPlotView.xaml
	/// </summary>
	public partial class OxyPlotView : UserControl
	{
		public OxyPlotView()
		{
			InitializeComponent();
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var pc = Plot.ActualController as PlotController;

			pc.UnbindMouseDown(OxyMouseButton.Left);
			pc.UnbindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Control);
			pc.UnbindMouseDown(OxyMouseButton.Left, OxyModifierKeys.Shift);
			// subscribe to d&d possibility
			pc.BindMouseDown(OxyMouseButton.Left, new DelegatePlotCommand<OxyMouseDownEventArgs>((view, controller, args) =>
			{
				var viewModel = (PlotViewModel)DataContext;
				controller.AddMouseManipulator(view, new DragDropTrackerManipulator(view, viewModel.TableData), args);
			}));
		}
	}
}
