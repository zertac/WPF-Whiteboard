using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using XPDFDoc.Helpers;

namespace XPDFDoc.Drawers
{
  public class XCustom : XShape, IShape
  {
    public Path Drawing;

    public void Create(Point e, string data)
    {
      IsDrawing = true;
      StartPoint = e;

      Drawing = new Path();
      Drawing.Data = Geometry.Parse(data);
      Drawing.Fill = StyleHelper.CurrentStyle.Background;
      Drawing.Stroke = StyleHelper.CurrentStyle.Border;
      Drawing.StrokeThickness = StyleHelper.CurrentStyle.BorderSize;
      Drawing.Width = 0;
      Drawing.Height = 0;
      Drawing.Opacity = 0.2;
      Drawing.Stretch = Stretch.Fill;
      Drawing.Tag = this;

      OwnedShape = Drawing;

      Style = new DrawerStyle(StyleHelper.CurrentStyle);
      Drawing.PreviewMouseLeftButtonDown += base.OnSelect;

      Canvas.SetLeft(Drawing, e.X);
      Canvas.SetTop(Drawing, e.Y);

      Drawer.Page.Children.Add(Drawing);
      Drawer.IsObjectCreating = true;
    }

    public void Create(Point e)
    {
      
    }

    public void Update(Point e)
    {
      if (!IsDrawing) return;

      Drawing.Width = Math.Abs(StartPoint.X - e.X);
      Drawing.Height = Math.Abs(StartPoint.Y - e.Y);
    }
  }
}
