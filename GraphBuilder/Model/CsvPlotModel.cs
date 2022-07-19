using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CsvHelper.Configuration;
using System.ComponentModel;

namespace GraphBuilder.Model
{
	public sealed class PlotPoint : INotifyPropertyChanged
	{
		double _x;
		double _y;

		public PlotPoint()
		{
			// do not remove - required for WPF DataGrid
		}

		public PlotPoint(double x, double y)
		{
			_x = x;
			_y = y;
		}

		public double X
		{
			get => _x;
			set
			{
				_x = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("X"));
			}
		}

		public double Y
		{
			get => _y;
			set
			{
				_y = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("Y"));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public override bool Equals(object? other)
		{
			var p = other as PlotPoint;
			if (p == null) return false;
			return p.X == X && p.Y == Y;
		}

		public override int GetHashCode()
		{
			return (int)X ^ (int)Y;
		}
	}

	public sealed class CsvPlotModel : INotifyPropertyChanged
	{
		string _axisX;
		string _axisY;

		public CsvPlotModel()
		{
			Points = new PointsCollection();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void Load(string data)
		{
			AxisX = Resources.AxisXText;
			AxisY = Resources.AxisYText;
			Points.Clear();

			var config = new CsvConfiguration(CultureInfo.InvariantCulture);
			config.DetectDelimiter = true;
			var reader = new CsvHelper.CsvReader(new StringReader(data), config);
			reader.Read();
			AxisX = reader.GetField(0);
			AxisY = reader.GetField(1);
			while (reader.Read())
				Points.Add(new PlotPoint(float.Parse(reader.GetField(0), CultureInfo.InvariantCulture), float.Parse(reader.GetField(1), CultureInfo.InvariantCulture)));
		}

		public string Save(string delimiter)
		{
			var builder = new StringBuilder();
			var config = new CsvConfiguration(CultureInfo.InvariantCulture);
			config.Delimiter = delimiter;
			var writer = new CsvHelper.CsvWriter(new StringWriter(builder), config);
			writer.WriteRecord(new KeyValuePair<string, string>(AxisX, AxisY));
			writer.NextRecord();
			foreach (var point in Points)
			{
				writer.WriteRecord(point);
				writer.NextRecord();
			}
			writer.Flush();
			return builder.ToString();
		}

		public string AxisX
		{
			get => _axisX;
			set
			{
				_axisX = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("AxisX"));
			}
		}

		public string AxisY
		{
			get => _axisY;
			set
			{
				_axisY = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("AxisY"));
			}
		}

		public PointsCollection Points { get; private set; }

		public sealed class PointsCollection : ObservableCollection<PlotPoint>
		{
			protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
			{
				if (e.OldItems != null)
					foreach (PlotPoint p in e.OldItems)
						p.PropertyChanged -= OnPropertyChanged;
				if (e.NewItems != null)
					foreach (PlotPoint p in e.NewItems)
						p.PropertyChanged += OnPropertyChanged;
				base.OnCollectionChanged(e);
			}
			
			public event PropertyChangedEventHandler PropertyChanged;

			private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				if (PropertyChanged != null)
					PropertyChanged(sender, e);
			}
		}
	}
}
