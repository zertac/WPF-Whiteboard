using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using XDrawerLib.Drawers;
using Point = System.Windows.Point;
using XText = XDrawerLib.Drawers.XText;

namespace XDrawerLib.Helpers
{
  public static class Selector
  {
    public static Canvas Canvas;
    public static bool IsDrawing;

    private static Rectangle _rect;
    private static Point _startPoint;

    public static void StartSelect(Point e)
    {
      if (Drawer.DrawTool == Tool.MoveResize) return;

      DeselectAll();

      _startPoint = e;
      IsDrawing = true;
      _rect = new Rectangle();
      _rect.Width = 0;
      _rect.Height = 0;
      _rect.Fill = StyleHelper.SelectionStyle.Background;
      _rect.Opacity = StyleHelper.SelectionStyle.Opacity;
      _rect.Stroke = StyleHelper.SelectionStyle.Border;
      _rect.Uid = "x_selector";

      Canvas.SetLeft(_rect, e.X);
      Canvas.SetTop(_rect, e.Y);
      Canvas.Children.Add(_rect);
    }

    public static void UpdateSelect(Point e)
    {
      if (!IsDrawing) return;

      var diffX = e.X - _startPoint.X;
      var diffY = e.Y - _startPoint.Y;
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

      _rect.RenderTransform = new ScaleTransform(scaleX, scaleY);

      _rect.Width = Math.Abs(diffX);
      _rect.Height = Math.Abs(diffY);
    }

    public static void FinishSelect()
    {
      IsDrawing = false;
      FindContainsObjects();
      Canvas.Children.Remove(_rect);

      //Drawer.CancelDrawing();

      _rect = null;
    }

    public static void CancelSelect()
    {

    }

    public static void EndDrawing()
    {
      foreach (var o in Drawer.Objects)
      {
        if (o.Value.IsDrawing)
        {
          if (Drawer.DrawTool != Tool.Ink)
            o.Value.Finish();
        }
      }
    }

    public static void DeleteSelected()
    {
      if (Drawer.ActiveObject == null) return;

      var o = Drawer.ActiveObject.Tag.ToType<XShape>();

      if (o.OwnedShape != null)
      {
        Drawer.Page.Children.Remove(o.OwnedShape);
        ArrangeObjects();
        UndoHelper.AddStep(UndoHelper.ActionType.Delete, o.OwnedShape);
      }

      if (o.OwnedControl != null)
      {
        var ctrl = (UIElement)o.OwnedShape;
        if (o.OwnedControl is List<Border> borders)
        {
          foreach (var b in borders)
          {
            if (ctrl.Uid == b.Uid)
            {
              Drawer.Page.Children.Remove(b);
              Drawer.Objects.Remove(b.Uid);
              UndoHelper.AddStep(UndoHelper.ActionType.Delete, b);
              break;
            }
          }
        }
        else
        {
          Drawer.Page.Children.Remove((UIElement)o.OwnedControl);
          Drawer.Objects.Remove(o.Id);
          UndoHelper.AddStep(UndoHelper.ActionType.Delete, (FrameworkElement)o.OwnedControl);
        }
      }

      if (o.FollowItem != null)
      {
        Drawer.Page.Children.Remove(o.FollowItem);
      }


      return;



      if (o.OwnedShape != null)
      {
        Drawer.Page.Children.Remove(o.OwnedShape);
        Drawer.Objects.Remove(o.Id);
        UndoHelper.AddStep(UndoHelper.ActionType.Delete, o.OwnedShape);
      }

      if (o.OwnedControl != null)
      {
        if (o.OwnedControl is List<Border> borders)
        {
          foreach (var b in borders)
          {
            if (Drawer.ActiveObject.Uid == b.Uid)
            {
              Drawer.Page.Children.Remove(b);
              Drawer.Objects.Remove(b.Uid);
              UndoHelper.AddStep(UndoHelper.ActionType.Delete, b);
              break;
            }
          }
        }
        else
        {
          Drawer.Page.Children.Remove((UIElement)o.OwnedControl);
          Drawer.Objects.Remove(o.Id);
          UndoHelper.AddStep(UndoHelper.ActionType.Delete, (FrameworkElement)o.OwnedControl);
        }
      }
    }

    public static void ArrangeObjects()
    {
      Drawer.Objects = new Dictionary<string, XShape>();
      foreach (FrameworkElement c in Drawer.Page.Children)
      {
        var obj = c.Tag;
        if (c.Tag != null)
        {
          Drawer.Objects.Add(c.Tag.ToType<XShape>().Id, c.Tag.ToType<XShape>());
        }
      }
    }

    public static void DeleteObject(FrameworkElement ctrl)
    {
      var o = ctrl.Tag.ToType<XShape>();

      if (o.OwnedShape != null)
      {
        var a = (UIElement)ctrl;
        Drawer.Page.Children.Remove(a);
        ArrangeObjects();
      }

      if (o.OwnedControl != null)
      {
        if (o.OwnedControl is List<Border> borders)
        {
          foreach (var b in borders)
          {
            if (ctrl.Uid == b.Uid)
            {
              Drawer.Page.Children.Remove(b);
              Drawer.Objects.Remove(b.Uid);
              break;
            }
          }
        }
        else
        {
          Drawer.Page.Children.Remove((UIElement)o.OwnedControl);
          Drawer.Objects.Remove(o.Id);
        }
      }

      if (o.FollowItem != null)
      {
        Drawer.Page.Children.Remove(o.FollowItem);
      }
    }

    public static void DeselectAll()
    {
      foreach (var item in Drawer.Objects.Values)
      {
        if (item.OwnedShape != null || item.OwnedControl != null)
        {
          item.IsSelected = false;
        }
      }
    }

    public static void DeleteAll()
    {
      foreach (var item in Drawer.Objects.Values)
      {
        if (item.OwnedShape != null)
        {
          DeleteObject(item.OwnedShape);
        }

        if (item.OwnedControl != null)
        {
          DeleteObject((FrameworkElement)item.OwnedControl);
        }
      }
    }

    public static void EndEditForObject()
    {
      foreach (var item in Drawer.Objects.Values)
      {
        if (item.OwnedControl != null && item.OwnedControl is RichTextBox txt)
        {
          txt.Tag.ToType<XText>().EndEdit();
        }
      }
    }

    public static void FinishDraw()
    {
      var lst = Drawer.Objects.ToList();

      foreach (KeyValuePair<string, XShape> item in lst)
      {
        if (item.Value.OwnedControl != null)
        {
          if (item.Value.OwnedControl is List<Border> b)
          {
            (item.Value as XInk)?.Finish();
          }
        }
      }
    }

    private static void FindContainsObjects()
    {
      DeselectAll();
      foreach (var item in Drawer.Objects.Values)
      {
        if (item.OwnedShape != null)
        {
          var s = IsContains(item.OwnedShape);
          if (s)
          {
            Identify(item);
          }
        }
        else if (item.OwnedControl != null)
        {
          if (item.OwnedControl is List<Border> inks)
          {
            foreach (var ink in inks)
            {
              var s = IsContains(ink);
              if (s)
              {
                Identify(item);
              }
            }
          }
          else
          {
            var s = IsContains((UIElement)item.OwnedControl);
            if (s)
            {
              Identify(item);
            }
          }
        }
      }
    }

    private static void Identify(XShape item)
    {
      item.ToType<XShape>().IsSelected = true;
    }

    private static bool IsContains(UIElement item)
    {
      if (item == null) return false;
      if (_rect == null) return false;

      var xScale = (double)_rect.RenderTransform.GetValue(ScaleTransform.ScaleXProperty);
      var yScale = (double)_rect.RenderTransform.GetValue(ScaleTransform.ScaleYProperty);

      var x1 = Canvas.GetLeft(_rect);
      var y1 = Canvas.GetTop(_rect);

      if (xScale < 0)
      {
        x1 -= _rect.ActualWidth;
      }

      if (yScale < 0)
      {
        y1 -= _rect.ActualHeight;
      }

      var r1 = new Rect(x1, y1, _rect.ActualWidth, _rect.ActualHeight);

      var x2 = Canvas.GetLeft(item);
      var y2 = Canvas.GetTop(item);
      var r2 = new Rect(x2, y2, item.RenderSize.Width, item.RenderSize.Height);

      if (r1.IntersectsWith(r2) && r1.Width > 0 && r1.Height > 0)
      {
        return true;
      }

      return false;
    }
  }
}
