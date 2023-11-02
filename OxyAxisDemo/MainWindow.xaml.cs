using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System.Windows;

namespace OxyAxisDemo
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      Plot1.Model = new PlotModel()
      {
        Title = "Axis without fix"
      };
      Plot2.Model = new PlotModel()
      {
        Title = "Axis fixed"
      };

      AddSeries(Plot1);
      AddSeries(Plot2);

      FixAxis(Plot2);

      Plot1.Model.InvalidatePlot(true);
      Plot2.Model.InvalidatePlot(true);
    }

    private void FixAxis(PlotView plot)
    {
      Axis axis = new LinearAxis { Title = "Y fixed", Position = AxisPosition.Left };

      // Set the MajorStep to 1. Then the labels are shown even on small plots.
      axis.MajorStep = 1;

      // Attention: This has not the same effect as when you change the "MajorStep" on a custom axis
      //plot.ActualModel.DefaultYAxis.MajorStep = 1;

      // I used this to see the difference between the default and custom axis.
      //string jsonCustom = JsonConvert.SerializeObject(axis, Formatting.Indented);
      //string jsonDefault = JsonConvert.SerializeObject(plot.ActualModel.DefaultYAxis, Formatting.Indented);

      plot.ActualModel.Axes.Add(axis);
    }

    private void AddSeries(PlotView plotView)
    {
      var data = new LineSeries();
      data.Points.Add(new DataPoint(0, 1));
      data.Points.Add(new DataPoint(1, 1));
      data.Points.Add(new DataPoint(1, -1));
      data.Points.Add(new DataPoint(2, -1));
      data.Points.Add(new DataPoint(2, 0));
      data.Points.Add(new DataPoint(3, 0));

      plotView.Model.Series.Add(data);
    }
  }
}
