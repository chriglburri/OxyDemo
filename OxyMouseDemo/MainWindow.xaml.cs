using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using System.Linq;
using System.Runtime.InteropServices;
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
      data.Points.Add(new DataPoint(0, 1));
      data.Points.Add(new DataPoint(1, 1));
      data.Points.Add(new DataPoint(1, -1));
      data.Points.Add(new DataPoint(2, -1));
      data.Points.Add(new DataPoint(2, 0));
      data.Points.Add(new DataPoint(3, 0));

      Plot.Model.Series.Add(data);

      Plot.Controller = new PlotController();
      Plot.Controller.BindMouseDown(OxyMouseButton.Left , new DelegatePlotCommand<OxyMouseDownEventArgs>(myPlot_MouseDown));

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
      var lineSeries = this.PlotView.ActualModel.Series.First() as LineSeries;
      var nearestPoint = lineSeries.GetNearestPoint(e.Position, true); // with interpolation: Finds a point between actual data points
      var mousePoint = lineSeries.InverseTransform(e.Position);

      var arrow = PlotView.ActualModel.Annotations.FirstOrDefault(a => a.Tag == "UniqueIdToFindTheElementLater") as ArrowAnnotation;
      if (arrow == null)
      {
        arrow = new ArrowAnnotation();
        arrow.Tag = "UniqueIdToFindTheElementLater";
        PlotView.ActualModel.Annotations.Add(arrow);
      }
      arrow.StartPoint = mousePoint;
      arrow.EndPoint = nearestPoint.DataPoint;

      if (!PlotView.ActualModel.Annotations.Contains(arrow))
      {
        PlotView.ActualModel.Annotations.Add(arrow);
      }

      PlotView.ActualModel.InvalidatePlot(false);

      _mainWindow.HandleNewArrowPosition(nearestPoint.DataPoint);

      base.Completed(e);

    }
  }
}
