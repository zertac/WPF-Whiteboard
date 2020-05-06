using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using XPDFDoc.Drawers;
using XPDFDoc.Helpers;

namespace XPDFDoc
{
  public class XRectangle : XShape, IShape
  {
    public Rectangle Drawing;

    public void Create(Point e)
    {
      IsDrawing = true;
      StartPoint = e;

      Drawing = new Rectangle();
      Drawing.Fill = StyleHelper.CurrentStyle.Background;
      Drawing.Stroke = StyleHelper.CurrentStyle.Border;
      Drawing.StrokeThickness = StyleHelper.CurrentStyle.BorderSize;
      Drawing.Width = 0;
      Drawing.Height = 0;
      Drawing.Opacity = 0.2;
      Drawing.Tag = this;

      OwnedShape = Drawing;

      Style = new DrawerStyle(StyleHelper.CurrentStyle);
      Drawing.PreviewMouseLeftButtonDown += base.OnSelect;

      Canvas.SetLeft(Drawing, e.X);
      Canvas.SetTop(Drawing, e.Y);

      Drawer.Page.Children.Add(Drawing);
      Drawer.IsObjectCreated = true;
    }

    public void Update(Point e)
    {
      if (!IsDrawing) return;

      Drawing.Width = Math.Abs(StartPoint.X - e.X);
      Drawing.Height = Math.Abs(StartPoint.Y - e.Y);
    }
  }
}
