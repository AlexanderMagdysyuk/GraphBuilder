using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using GraphBuilder.Model;
using GraphBuilder.ViewModel;
using System.ComponentModel;

namespace GraphBuilder
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		CsvPlotModel _model = new CsvPlotModel();
		bool _isDirty;

		public MainWindow()
		{
			InitializeComponent();
			Title = GraphBuilder.Resources.MainTitle;
			DataContext = new PlotViewModel(_model, () => IsDirty = true);
			CurrentDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			ResetData();
			IsDirty = false;
		}

		private bool IsDirty
		{
			get => _isDirty;
			set
			{
				_isDirty = value;
				Title = GraphBuilder.Resources.MainTitle + " - " + CurrentFile + (value ? "*" : "");
			}
		}

		private string CurrentFile { get; set; } = GraphBuilder.Resources.UntitledFileName;

		private string CurrentDir { get; set; }

		private void OnNewClick(object sender, RoutedEventArgs e)
		{
			ResetData();
		}

		private void OnOpenClick(object sender, RoutedEventArgs e)
		{
			if (IsDirty)
			{
				if (SaveChanges(true) != MessageBoxResult.Cancel)
				{
					OpenData();
					IsDirty = false;
				}
			}
			else
			{
				OpenData();
				IsDirty = false;
			}
		}

		private void OnSaveClick(object sender, RoutedEventArgs e)
		{
			if (SaveChanges(false) == MessageBoxResult.Yes)
				IsDirty = false;
		}

		private void OnExitClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void OnCopyClick(object sender, RoutedEventArgs e)
		{
			// save to Excel-like format
			string text = string.Empty;
			try
			{
				text = _model.Save("\t");
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, GraphBuilder.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (string.IsNullOrEmpty(text))
				return;
			Clipboard.SetText(text);
		}

		private void OnPasteClick(object sender, RoutedEventArgs e)
		{
			// parse from Excel
			var text = Clipboard.GetText();
			if (string.IsNullOrEmpty(text))
				return;
			try
			{
				_model.Load(text);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, GraphBuilder.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void OnInvertXClick(object sender, RoutedEventArgs e)
		{
			((PlotViewModel)DataContext).InvertX();
		}

		private void OnInvertYClick(object sender, RoutedEventArgs e)
		{
			((PlotViewModel)DataContext).InvertY();
		}

		private void OnExportSvgClick(object sender, RoutedEventArgs e)
		{
			var saveDialog = new SaveFileDialog();
			saveDialog.DefaultExt = ".svg";
			saveDialog.Filter = GraphBuilder.Resources.SvgDialogFilter;
			saveDialog.InitialDirectory = CurrentDir;
			if (saveDialog.ShowDialog(this) == true)
			{
				CurrentDir = Path.GetDirectoryName(saveDialog.FileName);
				try
				{
					File.WriteAllText(saveDialog.FileName, ((PlotViewModel)DataContext).ExportSvg());
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, ex.Message, GraphBuilder.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void OnAboutClick(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(this, GraphBuilder.Resources.AboutMessage, GraphBuilder.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Information);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (IsDirty)
				e.Cancel = SaveChanges(true) == MessageBoxResult.Cancel;
			base.OnClosing(e);
		}

		private void ResetData()
		{
			try
			{
				_model.Load(GraphBuilder.Resources.EmptyData);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, ex.Message, GraphBuilder.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			CurrentFile = GraphBuilder.Resources.UntitledFileName;
			IsDirty = false;
		}

		private void OpenData()
		{
			var openDialog = new OpenFileDialog();
			openDialog.DefaultExt = ".csv";
			openDialog.Filter = GraphBuilder.Resources.CsvDialogFilter;
			openDialog.InitialDirectory = CurrentDir;
			if (openDialog.ShowDialog(this) == true)
			{
				CurrentFile = Path.GetFileName(openDialog.FileName);
				CurrentDir = Path.GetDirectoryName(openDialog.FileName);
				try
				{
					_model.Load(File.ReadAllText(openDialog.FileName));
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, ex.Message, GraphBuilder.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private MessageBoxResult SaveChanges(bool withAsk)
		{
			var result = MessageBoxResult.Yes;
			if (withAsk)
				result = MessageBox.Show(this, GraphBuilder.Resources.SaveChangesMessage, GraphBuilder.Resources.MainTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				var saveDialog = new SaveFileDialog();
				saveDialog.DefaultExt = ".csv";
				saveDialog.Filter = GraphBuilder.Resources.CsvDialogFilter;
				saveDialog.InitialDirectory = CurrentDir;
				if (saveDialog.ShowDialog(this) == true)
				{
					CurrentFile = Path.GetFileName(saveDialog.FileName);
					CurrentDir = Path.GetDirectoryName(saveDialog.FileName);
					try
					{
						File.WriteAllText(saveDialog.FileName, _model.Save(","));
					}
					catch (Exception ex)
					{
						MessageBox.Show(this, ex.Message, GraphBuilder.Resources.MainTitle, MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
				else if (withAsk)
					return SaveChanges(withAsk);
				else
					result = MessageBoxResult.No;
			}
			return result;
		}
	}
}
