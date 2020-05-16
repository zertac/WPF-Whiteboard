using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;
using Svg;
using XDrawerLib.Helpers;

namespace XDrawerLib.Drawers
{
  public class XInk : XShape, IShape
  {
    public InkCanvas Drawing;

    public void Create(Point e)
    {
      IsDrawing = true;
      StartPoint = e;

      Drawing = new InkCanvas();
      Drawing.Width = Drawer.Page.ActualWidth;
      Drawing.Height = Drawer.Page.ActualHeight;
      Drawing.Tag = this;
      Drawing.Background = new SolidColorBrush(Colors.Transparent);

      var drawingAttributes = new DrawingAttributes();
      drawingAttributes.Color = Colors.Black;
      drawingAttributes.IgnorePressure = false;
      drawingAttributes.FitToCurve = true;
      drawingAttributes.StylusTip = StylusTip.Ellipse;
      drawingAttributes.Width = 4;

      Drawing.DefaultDrawingAttributes = drawingAttributes;

      OwnedControl = new List<Border>();

      Style = new DrawerStyle();

      Drawer.Page.Children.Add(Drawing);
      Drawer.IsObjectCreating = true;
    }

    public void Update(Point e)
    {

    }

    public override void Finish()
    {
      if (!IsDrawing || Drawing == null) return;

      IsDrawing = false;

      Drawer.IsObjectCreating = false;
      Drawer.IsDrawEnded = true;
      //Drawer.Objects.Remove(Id);

      var svg = new SvgDocument();
      var colorServer = new SvgColourServer(System.Drawing.Color.Black);

      var group = new SvgGroup { Fill = colorServer, Stroke = colorServer };
      svg.Children.Add(group);

      var lst = Drawing.Strokes.ToList();

      Drawer.Page.Children.Remove(Drawing);

      foreach (var stroke in lst)
      {
        var geometry = stroke.GetGeometry(stroke.DrawingAttributes).GetOutlinedPathGeometry();

        var border = new Border();
        border.Background = new SolidColorBrush(Colors.Transparent);
        border.Width = stroke.GetBounds().Width;
        border.Height = stroke.GetBounds().Height;
        border.MouseLeftButtonDown += OnSelect;
        border.StylusDown += OnErase;
        border.Uid = Guid.NewGuid().ToString();

        var path = new Path();
        path.Data = geometry;
        path.VerticalAlignment = VerticalAlignment.Stretch;
        path.HorizontalAlignment = HorizontalAlignment.Stretch;
        path.Fill = new SolidColorBrush(Colors.Black);
        path.Stretch = Stretch.Fill;
        border.Tag = this;
        border.Child = path;
        Canvas.SetLeft(border, stroke.GetBounds().Left);
        Canvas.SetTop(border, stroke.GetBounds().Top);

        if (OwnedControl is List<Border> oLst)
        {
          oLst.Add(border);
        }

        Drawer.Objects.Add(border.Uid, this);
        Drawer.Page.Children.Add(border);

        UndoHelper.AddStep(UndoHelper.ActionType.Create, border);
      }

      Drawing = null;
    }
  }
}
