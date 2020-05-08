using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using Svg;

namespace XPDFDoc.Helpers
{
  public static class InkHelper
  {
    public static void Create(InkCanvas cnv, Canvas main)
    {
      var svg = new SvgDocument();
      var colorServer = new SvgColourServer(System.Drawing.Color.Black);

      var group = new SvgGroup { Fill = colorServer, Stroke = colorServer };
      svg.Children.Add(group);

      foreach (var stroke in cnv.Strokes)
      {
        var geometry = stroke.GetGeometry(stroke.DrawingAttributes).GetOutlinedPathGeometry();

        var border = new Border();
        border.Background = new SolidColorBrush(Colors.CornflowerBlue);
        border.Width = 500;
        border.Height = 500;

        var path = new Path();
        path.Data = geometry;
        path.Width = 100;
        path.Height = 100;
        path.Fill = new SolidColorBrush(Colors.Black);
        path.Stretch = Stretch.Fill;

        border.Child = path;

        main.Children.Add(border);
        cnv.Background = new SolidColorBrush(Colors.Transparent);
      }
    }
  }
}
