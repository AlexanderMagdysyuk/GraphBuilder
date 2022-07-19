using GraphBuilder.Model;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Linq;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;

namespace GraphBuilder.ViewModel
{
    public class PlotViewModel : INotifyPropertyChanged
    {
        public const string XAxisKey = "X";
        public const string YAxisKey = "Y";

        CsvPlotModel _model;
        PlotModel _oxyModel;
        LinearAxis _axisX;
        LinearAxis _axisY;

		public PlotViewModel(CsvPlotModel model, Action dirty)
        {
            _model = model;
            _oxyModel = new PlotModel();
            _axisX = new LinearAxis { Position = AxisPosition.Bottom, Title = _model.AxisX, Key = XAxisKey };
            _axisY = new LinearAxis { Position = AxisPosition.Left, Title = _model.AxisY, Key = YAxisKey };
            _oxyModel.Axes.Add(_axisX);
            _oxyModel.Axes.Add(_axisY);

            // invalidate OxyPlot model on model changes
            _model.Points.CollectionChanged += (_, __) =>
            {
                dirty();
                InvalidateOxyModel();
            };
            _model.Points.PropertyChanged += (_, __) =>
            {
	            dirty();
                InvalidateOxyModel();
            };
            _model.PropertyChanged += (sender, e) =>
            {
                dirty();
                InvalidateOxyModel();
                if (PropertyChanged != null)
                    PropertyChanged(sender, e);
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<PlotPoint> TableData => _model.Points;

        public string AxisX
        {
            get => _model.AxisX;
            set => _model.AxisX = value;
        }

        public string AxisY
        {
            get => _model.AxisY;
            set => _model.AxisY = value;
        }

        public PlotModel OxyViewModel
        {
            get
			{
                InvalidateOxyModel();
                return _oxyModel;
            }
        }

		private void InvalidateOxyModel()
		{
            _axisX.Title = _model.AxisX;
            _axisY.Title = _model.AxisY;

            var series = new LineSeries();
            series.XAxisKey = _axisX.Key;
            series.YAxisKey = _axisY.Key;
            series.MarkerType = MarkerType.Square;
            series.MarkerStroke = OxyColor.FromRgb(0, 0, 0);
            series.Points.AddRange(_model.Points.Select(p => new DataPoint(p.X, p.Y)));

            _oxyModel.Series.Clear();
            _oxyModel.Series.Add(series);
            _oxyModel.InvalidatePlot(true);
        }

        public string ExportSvg()
		{
            var exporter = new SvgExporter { Width = 600, Height = 600 };
			return exporter.ExportToString(_oxyModel);
        }

        public void InvertX()
        {
            var pos = _axisX.StartPosition;
            _axisX.StartPosition = _axisX.EndPosition;
            _axisX.EndPosition = pos;
            _axisY.Position = _axisY.Position == AxisPosition.Left ? AxisPosition.Right : AxisPosition.Left;
            _oxyModel.InvalidatePlot(false);
        }

        public void InvertY()
		{
            var pos = _axisY.StartPosition;
            _axisY.StartPosition = _axisY.EndPosition;
            _axisY.EndPosition = pos;
            _axisX.Position = _axisX.Position == AxisPosition.Bottom ? AxisPosition.Top : AxisPosition.Bottom;
            _oxyModel.InvalidatePlot(false);
        }
    }
}
