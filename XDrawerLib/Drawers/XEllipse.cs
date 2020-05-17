﻿using System;
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
      Drawing.Uid = Guid.NewGuid().ToString();

      OwnedShape = Drawing;

      Style = new DrawerStyle(StyleHelper.CurrentStyle);
      Drawing.MouseLeftButtonDown += base.OnSelect;
      Drawing.StylusDown += OnErase;

      Canvas.SetLeft(Drawing, e.X);
      Canvas.SetTop(Drawing, e.Y);

      Drawer.Page.Children.Add(Drawing);
      Drawer.IsObjectCreating = true;
    }

    public void Update(Point e)
    {
      if (!IsDrawing) return;

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

      Inverse.ScaleX = scaleX;
      Inverse.ScaleY = scaleY;

      Drawing.RenderTransform = new ScaleTransform(scaleX, scaleY);

      if (Drawer.HotKeyHelper.IsPreserveSize())
      {
        Drawing.Width = Math.Abs(diffX);
        Drawing.Height = Drawing.Width;
      }
      else
      {
        Drawing.Width = Math.Abs(diffX);
        Drawing.Height = Math.Abs(diffY);
      }
    }

    public XEllipse(Drawer drawer) : base(drawer)
    {
      Drawer = drawer;
    }
  }
}