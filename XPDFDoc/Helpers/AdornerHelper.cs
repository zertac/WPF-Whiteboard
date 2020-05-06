using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;
using Type = XPDFDoc.Drawers.Type;

namespace XPDFDoc.Helpers
{
  public static class AdornerHelper
  {
    public static void AddAdorner(object sender)
    {
      if (sender is Line line)
      {
        var al = AdornerLayer.GetAdornerLayer(line);
        var adn = new LineAdorner(line);

        al?.Add(adn);

        Drawer.DrawType = Type.MoveResize;
      }
      else
      {
        var al = AdornerLayer.GetAdornerLayer((UIElement)sender);
        var adn = new ResizingAdorner((UIElement)sender);

        al?.Add(adn);

        Drawer.DrawType = Type.MoveResize;
      }
    }

    public static void RemoveAdorner(object sender)
    {
      var element = (UIElement)sender;
      var al = AdornerLayer.GetAdornerLayer(element);

      var toRemoveArray = al?.GetAdorners(element);
      if (toRemoveArray != null)
      {
        if (toRemoveArray[0] is LineAdorner)
        {
          var toRemove = (LineAdorner)toRemoveArray[0];

          al.Remove(toRemove);
        }
        else
        {
          var toRemove = (ResizingAdorner)toRemoveArray[0];

          al.Remove(toRemove);
        }
      }
    }

    public static void RemoveAllAdorners()
    {
      foreach (var item in Drawer.Objects.Values)
      {
        if (item.OwnedShape != null)
          RemoveAdorner(item.OwnedShape);

        if (item.OwnedControl != null)
          RemoveAdorner(item.OwnedControl);
      }
    }

    public static ResizingAdorner GetAdorner(object sender)
    {
      var element = (UIElement)sender;
      var al = AdornerLayer.GetAdornerLayer(element);

      var toRemoveArray = al?.GetAdorners(element);
      if (toRemoveArray != null)
      {
        return (ResizingAdorner)toRemoveArray[0];
      }

      return null;
    }
  }
}
