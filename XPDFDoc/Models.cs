using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace XPDFDoc.Drawers
{
  public enum Type
  {
    None,
    Ink,
    Line,
    Rectangle,
    Ellipse,
    Text,
    MoveResize,
    Triangle
  }

  public class DrawerStyle
  {
    public Brush Background { get; set; }
    public Brush Border { get; set; }
    public double BorderSize { get; set; }
    public double FontSize { get; set; }
    public double Opacity { get; set; }

    public DrawerStyle(DrawerStyle style)
    {
      this.Background = style.Background;
      this.Border = style.Border;
      this.BorderSize = style.BorderSize;
      this.FontSize = style.FontSize;
      this.Opacity = style.Opacity;
    }

    public DrawerStyle()
    {

    }
  }
}
