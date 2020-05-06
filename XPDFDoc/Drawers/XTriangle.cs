using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using XPDFDoc.Drawers;
using XPDFDoc.Helpers;

namespace XPDFDoc
{
  public class XTriangle : XShape, IShape
  {
    public Polygon Drawing;
    private int _addedPoint;
    private Line _shadowLine;

    public bool IsFinished;

    public void Create(Point e)
    {
      IsDrawing = true;
      StartPoint = e;

      Drawing = new Polygon();
      Drawing.Fill = StyleHelper.CurrentStyle.Background;
      Drawing.Stroke = StyleHelper.CurrentStyle.Border;
      Drawing.StrokeThickness = StyleHelper.CurrentStyle.BorderSize;
      Drawing.Opacity = 0.2;
      Drawing.Points = new PointCollection();
      Drawing.Points.Add(new Point(0, 0));
      Drawing.Points.Add(new Point(0, 0));
      Drawing.Points.Add(new Point(0, 0));
      Drawing.Tag = this;

      OwnedShape = Drawing;

      Style = new DrawerStyle(StyleHelper.CurrentStyle);
      Drawing.MouseLeftButtonDown += OnSelect;

      Canvas.SetLeft(Drawing, e.X);
      Canvas.SetTop(Drawing, e.Y);

      Drawer.Page.Children.Add(Drawing);
    }

    public void Update(Point e)
    {
      switch (_addedPoint)
      {
        case 1:
          _shadowLine.X2 = e.X;
          _shadowLine.Y2 = e.Y;
          break;
        case 2:
          {
            var p = new Point();
            p.X = e.X - StartPoint.X;
            p.Y = e.Y - StartPoint.Y;

            Drawing.Points[1] = p;
            break;
          }
      }
    }

    public new void Finish()
    {
      if (_addedPoint < 3) return;

      IsDrawing = false;
      OwnedShape.Opacity = StyleHelper.CurrentStyle.Opacity;

      Drawer.DrawType = Drawer.ContinuousDraw ? Drawer.DrawType : Drawers.Type.None;

      Drawer.Page.Children.Remove(_shadowLine);

      FindMinPoints();

      IsFinished = true;

      Drawing.Stretch = Stretch.Fill;
    }

    public new void Cancel()
    {
      Drawer.Page.Children.Remove(_shadowLine);
      Drawer.Page.Children.Remove(Drawing);
      Instance = null;
    }

    public void AddPoint(Point e)
    {
      if (Drawing == null)
      {
        Create(e);

        var p = new Point { X = 0, Y = 0 };

        Drawing.Points[2] = p;

        _shadowLine = new Line();
        _shadowLine.Stroke = new SolidColorBrush(Colors.Black);
        _shadowLine.X1 = StartPoint.X;
        _shadowLine.Y1 = StartPoint.Y;
        _shadowLine.Opacity = 0.2;

        Canvas.SetLeft(_shadowLine, 0);
        Canvas.SetTop(_shadowLine, 0);

        Drawer.Page.Children.Add(_shadowLine);

        _addedPoint = 1;
      }
      else
      {
        if (IsDrawing == false) return;

        var p = new Point();
        p.X = e.X - StartPoint.X;
        p.Y = e.Y - StartPoint.Y;

        if (_addedPoint == 1)
        {
          Drawing.Points[0] = p;
          _addedPoint = 2;
        }
        else if (_addedPoint == 2)
        {
          Drawing.Points[1] = p;
          _addedPoint = 3;

          Finish();
        }
      }
    }

    private void FindMinPoints()
    {
      var minX = Drawing.Points.Min(x => x.X);
      var minY = Drawing.Points.Min(x => x.Y);

      if (minX < 0)
      {
        var diffX = Math.Abs(minX);
        Canvas.SetLeft(Drawing, Canvas.GetLeft(Drawing) - diffX);

        for (var i = 0; i < Drawing.Points.Count; i++)
        {
          Drawing.Points[i] = new Point(Drawing.Points[i].X + diffX, Drawing.Points[i].Y);
        }
      }

      if (minY < 0)
      {
        var diffY = Math.Abs(minY);
        Canvas.SetTop(Drawing, Canvas.GetTop(Drawing) - diffY);

        for (var i = 0; i < Drawing.Points.Count; i++)
        {
          Drawing.Points[i] = new Point(Drawing.Points[i].X, Drawing.Points[i].Y + diffY);
        }
      }
    }
  }
}
