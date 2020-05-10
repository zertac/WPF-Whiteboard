using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using XDrawerLib.Helpers;

namespace XDrawerLib.Drawers
{
  public class XEllipse : XShape, IShape
  {
    public static Ellipse Drawing;

    public void Create(Point e)
    {
      IsDrawing = true;
      StartPoint = e;

      Drawing = new Ellipse();
      Drawing.Fill = StyleHelper.CurrentStyle.Background;
      Drawing.Stroke = StyleHelper.CurrentStyle.Border;
      Drawing.StrokeThickness = StyleHelper.CurrentStyle.BorderSize;
      Drawing.Width = 0;
      Drawing.Height = 0;
      Drawing.Opacity = 0.2;
      Drawing.Tag = this;

      OwnedShape = Drawing;

      Style = new DrawerStyle(StyleHelper.CurrentStyle);
      Drawing.MouseLeftButtonDown += base.OnSelect;

      Canvas.SetLeft(Drawing, e.X);
      Canvas.SetTop(Drawing, e.Y);

      Drawer.Page.Children.Add(Drawing);
      Drawer.IsObjectCreating = true;
    }

    public void Update(Point e)
    {
      if (!IsDrawing) return;

      //var state = Keyboard.IsKeyDown(Key.LeftCtrl);

      //if (state)
      //{
      //  Drawing.Width = Math.Abs(StartPoint.X - e.X);
      //  Drawing.Height = Drawing.Width;
      //}
      //else
      //{
      //  Drawing.Width = Math.Abs(StartPoint.X - e.X);
      //  Drawing.Height = Math.Abs(StartPoint.Y - e.Y);
      //}

      var diffX = e.X - StartPoint.X;
      var diffY = e.Y - StartPoint.Y;
      var scaleX = 1;
      var scaleY = 1;

      if (diffX < 0)
      {
        scaleX = -1;
      }

      if (diffY < 0)
      {
        scaleY = -1;
      }

      Drawing.RenderTransform = new ScaleTransform(scaleX, scaleY);

      Drawing.Width = Math.Abs(diffX);
      Drawing.Height = Math.Abs(diffY);
    }
  }
}