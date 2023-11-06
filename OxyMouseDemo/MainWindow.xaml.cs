using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using System;
using System.Linq;
using System.Windows;

namespace OxyMouseDemo
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      Plot.Model = new PlotModel()
      {
        Title = "Mouse Events"
      };

      var data = new LineSeries();
      data.Points.Add(new DataPoint(0, 0));
      data.Points.Add(new DataPoint(1, 1));
      data.Points.Add(new DataPoint(2, -1));
      data.Points.Add(new DataPoint(3, -1));
      data.Points.Add(new DataPoint(4, 0));
      data.Points.Add(new DataPoint(4.01, 0.5));
      data.Points.Add(new DataPoint(5, 0));

      Plot.Model.Series.Add(data);

      Plot.Controller = new PlotController();
      Plot.Controller.BindMouseDown(OxyMouseButton.Left, new DelegatePlotCommand<OxyMouseDownEventArgs>(myPlot_MouseDown));

      Plot.Model.InvalidatePlot(true);
    }

    public void HandleNewArrowPosition(DataPoint dataPoint)
    {
      txtHint.Text = $"Position: {dataPoint.X} / {dataPoint.Y}";
    }

    private void myPlot_MouseDown(IPlotView view, IController controller, OxyMouseDownEventArgs e)
    {
      view.ActualController.AddMouseManipulator(view, new ZoomOrClickManipulator(view, this), e);
    }
  }

  public class ZoomOrClickManipulator : ZoomRectangleManipulator
  {
    private MainWindow _mainWindow;

    public ZoomOrClickManipulator(IPlotView view, MainWindow mainWindow) : base(view)
    {
      // use the reference to the main window, to enable a "Callback";
      _mainWindow = mainWindow ?? throw new System.ArgumentNullException(nameof(mainWindow));
    }

    public override void Started(OxyMouseEventArgs e)
    {
      base.Started(e);
    }

    public override void Delta(OxyMouseEventArgs e)
    {
      base.Delta(e);
    }

    public override void Completed(OxyMouseEventArgs e)
    {
      if (!e.Position.Equals(StartPosition))
      {
        base.Completed(e);
        return;
      }
      var lineSeries = PlotView.ActualModel.Series.First() as LineSeries;

      var nearestPoint = GetNearestXPointWithCalculation(lineSeries, e.Position);
      var mousePoint = lineSeries.InverseTransform(e.Position);

      if (!nearestPoint.HasValue)
      {
        base.Completed(e); return;
      }

      var arrow = PlotView.ActualModel.Annotations.FirstOrDefault(a => a.Tag == "UniqueIdToFindTheElementLater") as ArrowAnnotation;
      if (arrow == null)
      {
        arrow = new ArrowAnnotation();
        arrow.Tag = "UniqueIdToFindTheElementLater";
        PlotView.ActualModel.Annotations.Add(arrow);
      }
      arrow.StartPoint = mousePoint;
      //arrow.EndPoint = nearestPoint.DataPoint;
      arrow.EndPoint = nearestPoint.Value;

      if (!PlotView.ActualModel.Annotations.Contains(arrow))
      {
        PlotView.ActualModel.Annotations.Add(arrow);
      }

      PlotView.ActualModel.InvalidatePlot(false);

      //_mainWindow.HandleNewArrowPosition(nearestPoint.DataPoint);
      _mainWindow.HandleNewArrowPosition(nearestPoint.Value);

      base.Completed(e);
    }

    private DataPoint? GetNearestXPointWithCalculation(LineSeries series, ScreenPoint mouseScreenPoint)
    {
      if (series.Points == null || !series.Points.Any())
      {
        return null;
      }
      if (series.Points.Count == 1)
      {
        return series.Points.First();
      }
      var mousePoint = series.InverseTransform(mouseScreenPoint);
      var leftPoint = series.Points.Last(p => p.X < mousePoint.X);
      var rightPoint = series.Points.First(p => p.X > mousePoint.X);

      return CalculateInterpolatedPoint(leftPoint, rightPoint, mousePoint);
    }

    private DataPoint? CalculateInterpolatedPoint(DataPoint leftPoint, DataPoint rightPoint, DataPoint mousePoint)
    {
      if (leftPoint.X == rightPoint.X) { return new DataPoint(leftPoint.X, mousePoint.Y); } // avoid division by zero
      if (leftPoint.Y == rightPoint.Y) { return new DataPoint(mousePoint.X, leftPoint.Y); } // avoid division by zero
      var slope = (rightPoint.Y - leftPoint.Y) / (rightPoint.X - leftPoint.X);
      if (Math.Abs(slope) > 1) // More than 45° is considered "steep"
      {
        // slope = 1 / slope. But better recalculate for better precision.
        slope = (rightPoint.X - leftPoint.X) / (rightPoint.Y - leftPoint.Y);
        var newX = slope * (mousePoint.Y - leftPoint.Y) + leftPoint.X;
        return new DataPoint(newX, mousePoint.Y);
      }
      else
      {
        var newY = slope * (mousePoint.X - leftPoint.X) + leftPoint.Y;
        return new DataPoint(mousePoint.X, newY);
      }
    }
  }
}
