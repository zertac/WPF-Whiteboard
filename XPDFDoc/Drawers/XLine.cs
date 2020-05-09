using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using XPDFDoc.Helpers;

namespace XPDFDoc.Drawers
{
  public class XLine : XShape, IShape
  {
    public Line Drawing;

    public void Create(Point e)
    {
      IsDrawing = true;
      StartPoint = e;

      Drawing = new Line();
      Drawing.X1 = e.X;
      Drawing.Y1 = e.Y;
      Drawing.Stroke = new SolidColorBrush(Colors.Black);
      Drawing.StrokeThickness = 2;
      Drawing.Opacity = 0.2;
      Drawing.Tag = this;
      OwnedShape = Drawing;

      Style = new DrawerStyle(StyleHelper.CurrentStyle);
      Drawing.MouseLeftButtonDown += base.OnSelect;

      Drawer.Page.Children.Add(Drawing);
      Drawer.IsObjectCreating = true;
    }

    public void Update(Point e)
    {
      if (!IsDrawing) return;

      Drawing.X2 = e.X;
      Drawing.Y2 = e.Y;
    }
  }
}
