using GraphBuilder.Model;
using GraphBuilder.ViewModel;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GraphBuilder.View
{
	/// <summary>
	/// OxyPlot edit manipulator logic.
	/// </summary>
	internal class DragDropTrackerManipulator : MouseManipulator
	{
		ObservableCollection<PlotPoint> _point;

		DataPointSeries _currentSeries;
		PlotPoint _currentItem;
		double _startX;
		double _startY;

		public DragDropTrackerManipulator(IPlotView plotView, ObservableCollection<PlotPoint> points) : base(plotView)
		{
			_point = points;
		}

		public override void Started(OxyMouseEventArgs e)
		{
			base.Started(e);
			// started manipulation
			_currentSeries = PlotView?.ActualModel?.Series.FirstOrDefault() as DataPointSeries;
			Delta(e);
		}

		public override void Completed(OxyMouseEventArgs e)
		{
			base.Completed(e);
			e.Handled = true;
			// completed manipulation
			_currentItem = null;
			_currentSeries = null;
			PlotView.InvalidatePlot(true);
			PlotView.HideTracker();
		}

		public override void Delta(OxyMouseEventArgs e)
		{
			base.Delta(e);
			e.Handled = true;
			if (_currentSeries == null)
			{
				PlotView.HideTracker();
				return;
			}

			var actualModel = PlotView.ActualModel;
			if (actualModel == null && !actualModel.PlotArea.Contains(e.Position.X, e.Position.Y))
				return;
			
			// gets model coordinates
			var x = _currentSeries.InverseTransform(e.Position).X;
			var y = _currentSeries.InverseTransform(e.Position).Y;
			double diffX = _startX - x;
			_startX = x;
			double diffY = _startY - y;
			_startY = y;

			if (_currentItem == null)
			{
				var axisX = actualModel.Axes.FirstOrDefault(a => a.Key == PlotViewModel.XAxisKey);
				var axisY = actualModel.Axes.FirstOrDefault(a => a.Key == PlotViewModel.YAxisKey);
				if (axisX != null && axisY != null)
				{
					var tresholdX = Math.Abs(axisX.ActualMinimum - axisX.ActualMaximum) / 100;
					var tresholdY = Math.Abs(axisY.ActualMinimum - axisY.ActualMaximum) / 100;
					// search for current point using model coordinates
					_currentItem = _point.FirstOrDefault(p => (p.X >= x - tresholdX) && (p.Y >= y - tresholdY) && (p.X <= x + tresholdX) && (p.Y <= y + tresholdY));
				}
			}
			else
			{
				// change current point
				_currentItem.X = _currentItem.X - diffX;
				_currentItem.Y = _currentItem.Y - diffY;
				PlotView.InvalidatePlot(true);
			}
		}
	}
}
