using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;
using XDrawerLib.Helpers.Adorners;

namespace XDrawerLib.Helpers
{
  public class AdornerHelper
  {
    public Drawer Drawer;

    public AdornerHelper(Drawer drawer)
    {
      Drawer = drawer;
    }
    public void AddAdorner(object sender, object followItem = null)
    {
      Drawer.Selector.FinishSelect();

      if (sender is Line line)
      {
        var al = AdornerLayer.GetAdornerLayer(line);
        var adn = new LineAdorner(line);
        adn.Drawer = Drawer;
        adn.FollowItem = followItem;
        al?.Add(adn);

        Drawer.DrawTool = Tool.MoveResize;
      }
      else
      {
        var al = AdornerLayer.GetAdornerLayer((UIElement)sender);
        var adn = new ResizingAdorner((UIElement)sender);
        adn.Drawer = Drawer;
        al?.Add(adn);

        Drawer.DrawTool = Tool.MoveResize;
      }
    }

    public void RemoveAdorner(object sender)
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

    public void RemoveAllAdorners()
    {
      foreach (var item in Drawer.Objects.Values)
      {
        if (item.OwnedShape != null)
          RemoveAdorner(item.OwnedShape);

        if (item.OwnedControl != null)
        {
          if (item.OwnedControl is List<Border> borders)
          {
            foreach (var b in borders)
            {
              RemoveAdorner(b);
            }
          }
          else
          {
            RemoveAdorner(item.OwnedControl);
          }
        }
      }
    }

    public ResizingAdorner GetAdorner(object sender)
    {
      var element = (UIElement)sender;
      var al = AdornerLayer.GetAdornerLayer(element);

      var toRemoveArray = al?.GetAdorners(element);
      return (ResizingAdorner)toRemoveArray?[0];
    }
  }
}
