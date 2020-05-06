﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using XPDFDoc.Drawers;
using XPDFDoc.Helpers;
using Type = XPDFDoc.Drawers.Type;

namespace XPDFDoc
{
  public class XShape
  {
    protected static object Instance;
    internal bool IsDrawing;
    internal Point StartPoint;
    public Shape OwnedShape;
    public object OwnedControl;
    public string Id;
    public bool IsCustom;

    private bool _isSelected;
    public Action OnDoubleClick;
    private DrawerStyle _style;

    public DrawerStyle Style
    {
      get => _style;
      set
      {
        _style = value;

        if (!(OwnedShape is Line))
        {
          OwnedShape.Fill = _style.Background;
        }

        OwnedShape.Stroke = _style.Border;
        OwnedShape.StrokeThickness = _style.BorderSize;
        OwnedShape.Opacity = _style.Opacity;
      }
    }

    public XShape()
    {
      Id = Guid.NewGuid().ToString();

      AdornerHelper.RemoveAllAdorners();

      Selector.DeselectAll();
    }

    public bool IsSelected
    {
      get => _isSelected;
      set
      {
        _isSelected = value;

        if (value)
        {
          if (OwnedShape != null)
          {
            OwnedShape.Stroke = new SolidColorBrush(Colors.Aqua);
            OwnedShape.StrokeThickness = 3;
          }


          if (OwnedControl != null)
          {
            if (OwnedControl is RichTextBox o)
            {
              o.BorderBrush = new SolidColorBrush(Colors.Aqua);
              o.BorderThickness = new Thickness(3);
            }
          }
        }
        else
        {
          Style = _style;
        }
      }
    }

    public void OnSelect(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      if (Drawer.DrawType != Type.None && Drawer.DrawType != Type.MoveResize) return;

      if (Drawer.IsEditMode) return;

      AdornerHelper.RemoveAllAdorners();
      Selector.DeselectAll();

      IsSelected = true;

      AdornerHelper.AddAdorner(sender);
    }

    internal static T Init<T>() where T : new()
    {
      Instance = new T();
      return (T)Instance;
    }

    public void Finish()
    {
      IsDrawing = false;

      if (OwnedShape != null)
        OwnedShape.Opacity = StyleHelper.CurrentStyle.Opacity;

      Drawer.DrawType = Drawer.ContinuousDraw ? Drawer.DrawType : Type.None;
      Drawer.IsObjectCreating = false;
      Drawer.IsDrawEnded = true;
    }

    public void Cancel()
    {
      Drawer.Page.Children.Remove(OwnedShape);
    }

    public virtual void Edit()
    {

    }

    public virtual void EndEdit()
    {

    }
  }
}