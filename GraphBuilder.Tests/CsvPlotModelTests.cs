using GraphBuilder.Model;

namespace GraphBuilder.Tests
{
	[TestFixture]
	public static class Tests
	{
		static CsvPlotModel _model;

		[SetUp]
		public static void Setup()
		{
			_model = new CsvPlotModel();
		}

		[TestCase(",")]
		[TestCase("\t")]
		public static void LoadTest(string delimiter)
		{
			_model.Load(string.Format("X{0}Y\r\n0{0}9\r\n9{0}0\r\n", delimiter));
			Assert.AreEqual("X", _model.AxisX);
			Assert.AreEqual("Y", _model.AxisY);
			Assert.AreEqual(2, _model.Points.Count);
			Assert.AreEqual(new PlotPoint(0, 9), _model.Points[0]);
			Assert.AreEqual(new PlotPoint(9, 0), _model.Points[1]);
		}

		[TestCase(",")]
		[TestCase("\t")]
		public static void SaveTest(string delimiter)
		{
			_model.AxisX = "X";
			_model.AxisY = "Y";
			_model.Points.Add(new PlotPoint(0, 9));
			_model.Points.Add(new PlotPoint(9, 0));
			Assert.AreEqual(string.Format("X{0}Y\r\n0{0}9\r\n9{0}0\r\n", delimiter), _model.Save(delimiter));
		}
	}
}
