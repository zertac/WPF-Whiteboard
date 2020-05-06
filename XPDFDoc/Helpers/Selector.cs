﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = System.Windows.Point;
using Type = XPDFDoc.Drawers.Type;

namespace XPDFDoc.Helpers
{
  public static class Selector
  {
    public static Canvas Canvas;
    public static bool IsDrawing;

    private static Rectangle _rect;
    private static Point _startPoint;

    public static void StartSelect(Point e)
    {
      if (Drawer.DrawType == Type.MoveResize) return;

      _startPoint = e;
      IsDrawing = true;
      _rect = new System.Windows.Shapes.Rectangle();
      _rect.Width = 0;
      _rect.Height = 0;
      _rect.Fill = new SolidColorBrush(Colors.Blue);
      _rect.Opacity = 0.2;
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

      _rect = null;
    }

    public static void CancelSelect()
    {

    }

    public static void DeselectAll()
    {
      foreach (var item in Drawer.Objects.Values)
      {
        if (item.OwnedShape != null)
        {
          item.IsSelected = false;
        }
      }
    }

    public static void EndEdit()
    {
      foreach (var item in Drawer.Objects.Values)
      {
        if (item.OwnedControl != null)
        {
          item.EndEdit();
        }
      }
    }

    private static void FindContainsObjects()
    {
      //var lst = new List<UIElement>();
      DeselectAll();
      foreach (var item in Drawer.Objects.Values)
      {
        var s = IsContains(item);
        if (s)
        {
          //lst.Add(item);
          Identify(item);
        }
      }
      //foreach (UIElement item in Canvas.Children)
      //{
      //  if (item.Uid != "x_selector")
      //  {
      //    var s = IsContains(item);
      //    if (s)
      //    {
      //      lst.Add(item);
      //      Identify(item);
      //    }
      //  }
      //}
    }

    private static void Identify(XShape item)
    {
      item.ToType<XShape>().IsSelected = true;
    }

    private static bool IsContains(XShape o)
    {
      var item = (UIElement)o.OwnedShape;
      if (item == null) return false;

      var xScale = (double)_rect.RenderTransform.GetValue(ScaleTransform.ScaleXProperty);
      var yScale = (double)_rect.RenderTransform.GetValue(ScaleTransform.ScaleYProperty);

      var x1 = Canvas.GetLeft(_rect);
      var y1 = Canvas.GetTop(_rect);

      if (xScale < 0)
      {
        x1 = x1 - _rect.ActualWidth;
      }

      if (yScale < 0)
      {
        y1 = y1 - _rect.ActualHeight;
      }

      var r1 = new Rect(x1, y1, _rect.ActualWidth, _rect.ActualHeight);

      var x2 = Canvas.GetLeft(item);
      var y2 = Canvas.GetTop(item);
      var r2 = new Rect(x2, y2, item.RenderSize.Width, item.RenderSize.Height);

      if (r1.IntersectsWith(r2))
      {
        return true;
      }

      return false;
    }
  }
}
