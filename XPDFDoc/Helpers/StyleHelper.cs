using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using XPDFDoc.Drawers;

namespace XPDFDoc.Helpers
{
  public static class StyleHelper
  {
    public static DrawerStyle SelectionStyle;
    public static DrawerStyle CurrentStyle;

    static StyleHelper()
    {
      if (SelectionStyle == null)
      {
        SelectionStyle = new DrawerStyle();
        SelectionStyle.Background = new SolidColorBrush(Colors.Blue);
        SelectionStyle.Border = new SolidColorBrush(Colors.DarkBlue);
        SelectionStyle.BorderSize = 1;
        SelectionStyle.Opacity = 0.2;
      }

      if (CurrentStyle == null)
      {
        CurrentStyle = new DrawerStyle();
        CurrentStyle.Background = new SolidColorBrush(Colors.Transparent);
        CurrentStyle.Border = new SolidColorBrush(Colors.Black);
        CurrentStyle.BorderSize = 2;
        CurrentStyle.FontSize = 16;
        CurrentStyle.Opacity = 1;
      }
    }
  }
}
