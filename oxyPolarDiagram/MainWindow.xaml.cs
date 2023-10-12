using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Linq;
using System.Windows;

namespace oxyPolarDiagram
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      this.Plot.Model = new OxyPlot.PlotModel() {
        Title = "Polar plot",
        PlotType = PlotType.Polar,
        PlotMargins = new OxyThickness(20, 20, 4, 40),
        PlotAreaBorderThickness = new OxyThickness(0)
      };
      Axis angle;
      Axis magnitude;
      this.Plot.Model.Axes.Add(angle=new OxyPlot.Axes.AngleAxis
      {
        Key = "Angle",
        Title = "Angle",
        Minimum = 0,
        Maximum = Math.PI * 2,
        MajorStep = Math.PI / 6,
        MinorStep = Math.PI / 12,
        FormatAsFractions = true,
        FractionUnit = Math.PI,
        FractionUnitSymbol = "π"
      });
      this.Plot.Model.Axes.Add(magnitude=new OxyPlot.Axes.MagnitudeAxis()
      {
        Key = "Magnitude",
        Title = "Magnitude",
        MajorGridlineStyle = LineStyle.Solid,
        MajorStep = 1,
        MinorStep = 0.1,
        
      });

      // LineSeries can not show arrows
      var series = new LineSeries();
      series.Points.Add(new DataPoint(0, 0));
      series.Points.Add(new DataPoint(0.5, Math.PI / 2));
      series.MarkerType = MarkerType.Circle;
      this.Plot.Model.Series.Add(series);

      series = new LineSeries();
      series.Points.Add(new DataPoint(0, 0));
      series.Points.Add(new DataPoint(0.7, Math.PI / 3));
      series.MarkerType = MarkerType.Square;
      this.Plot.Model.Series.Add(series);

      series = new LineSeries();
      series.Points.Add(new DataPoint(0, 0));
      series.Points.Add(new DataPoint(0.2, 0));
      series.MarkerType = MarkerType.Diamond;
      this.Plot.Model.Series.Add(series);

      // Annotations are good for arrows.
      this.Plot.Model.Annotations.Add(new ArrowAnnotation() { StartPoint = new DataPoint(0, 0), EndPoint = new DataPoint(0.5, Math.PI) });

      // Refresh
      this.Plot.Model.InvalidatePlot(true);

    }
  }
}
