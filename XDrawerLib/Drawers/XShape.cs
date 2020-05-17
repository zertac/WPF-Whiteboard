using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using XDrawerLib.Helpers;

namespace XDrawerLib.Drawers
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
    internal FrameworkElement FollowItem;
    internal ScaleTransform Inverse;
    public Drawer Drawer;

    public DrawerStyle Style
    {
      get => _style;
      set
      {
        _style = value;

        if (OwnedShape != null)
        {
          OwnedShape.Stroke = _style.Border;
          OwnedShape.StrokeThickness = _style.BorderSize;
          OwnedShape.Opacity = _style.Opacity;
          OwnedShape.Fill = _style.Background;
        }

        if (OwnedControl != null)
        {
          if (OwnedControl is RichTextBox txt)
          {
            txt.BorderBrush = _style.Border;
            txt.BorderThickness = new Thickness(_style.BorderSize);
            txt.Opacity = _style.Opacity;
            txt.Background = _style.Background;
          }
        }
      }
    }

    public XShape(Drawer drawer)
    {
      Drawer = drawer;

      Id = Guid.NewGuid().ToString();
      Inverse = new ScaleTransform();

      Console.WriteLine("created :" + Id);

      Drawer.AdornerHelper.RemoveAllAdorners();

      Drawer.Selector.DeselectAll();
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
      if (sender is Polygon && IsDrawing)
      {
        e.Handled = false;
      }
      else
      {
        if (sender is RichTextBox)
        {
          e.Handled = false;
        }
        else
        {
          if (Drawer.DrawTool != Tool.None && Drawer.DrawTool != Tool.MoveResize)
          {
            e.Handled = true;
          }
        }
      }

      if (Drawer.DrawTool != Tool.None && Drawer.DrawTool != Tool.MoveResize) return;

      if (Drawer.IsEditMode) return;

      Drawer.AdornerHelper.RemoveAllAdorners();
      Drawer.Selector.DeselectAll();

      IsSelected = true;

      if (FollowItem != null)
      {
        Drawer.AdornerHelper.AddAdorner(sender, this);
      }
      else
      {
        Drawer.AdornerHelper.AddAdorner(sender);
      }

      if (OwnedShape != null)
      {
        Drawer.ActiveObject = OwnedShape;
      }

      if (OwnedControl != null)
      {
        if (OwnedControl is List<Border> borders)
        {
          var c = (FrameworkElement)sender;

          foreach (var b in borders)
          {
            if (b.Uid == c.Uid)
            {
              Drawer.ActiveObject = b;
              break;
            }
          }
        }
        else
        {
          Drawer.ActiveObject = (FrameworkElement)OwnedControl;
        }
      }
    }

    internal void OnErase(object sender, System.Windows.Input.StylusEventArgs e)
    {
      if (e.Inverted)
      {
        Drawer.Selector.DeleteObject(OwnedShape);
      }
    }

    //internal void OnEraseTest(object sender, MouseEventArgs e)
    //{
    //  if (e.RightButton == MouseButtonState.Pressed)
    //  {
    //    Console.WriteLine("removed");
    //    Selector.DeleteObject(OwnedShape);
    //  }
    //}

    internal static T Init<T>() where T : new()
    {
      Instance = new T();
      return (T)Instance;
    }

    public virtual void Finish()
    {
      IsDrawing = false;

      if (OwnedShape != null)
        OwnedShape.Opacity = StyleHelper.CurrentStyle.Opacity;

      Drawer.DrawTool = Drawer.ContinuousDraw ? Drawer.DrawTool : Tool.None;
      Drawer.IsObjectCreating = false;
      Drawer.IsDrawEnded = true;

      Style = Style;

      if (OwnedShape != null)
      {
        Drawer.UndoHelper.AddStep(UndoHelper.ActionType.Create, OwnedShape);
      }

      if (OwnedControl != null)
      {
        Drawer.UndoHelper.AddStep(UndoHelper.ActionType.Create, (FrameworkElement)OwnedControl);
      }

      if (Inverse.ScaleX < 1)
      {
        if (OwnedShape != null)
        {
          OwnedShape.RenderTransform = new ScaleTransform(1, 1);

          var x = Canvas.GetLeft(OwnedShape) - OwnedShape.ActualWidth;

          Canvas.SetLeft(OwnedShape, x);
        }
      }

      if (Inverse.ScaleY < 1)
      {
        if (OwnedShape != null)
        {
          OwnedShape.RenderTransform = new ScaleTransform(1, 1);

          var y = Canvas.GetTop(OwnedShape) - OwnedShape.ActualHeight;

          Canvas.SetTop(OwnedShape, y);
        }
      }
    }

    public void Cancel()
    {
      Drawer.IsObjectCreating = false;

      if (OwnedShape != null)
      {
        Drawer.Selector.DeleteObject(OwnedShape);
      }

      if (OwnedControl != null)
      {
        Drawer.Selector.DeleteObject((FrameworkElement)OwnedControl);
      }
    }

    public virtual void Edit()
    {

    }

    public virtual void EndEdit()
    {

    }

    public void SetTextStyle(DependencyProperty property, object value)
    {
      if (OwnedControl is RichTextBox txt)
      {
        var ts = txt.Selection;
        ts.ApplyPropertyValue(property, value);
        txt.Focus();
      }
    }

    public void SetPosition(Point position, Point position2 = new Point())
    {
      if (OwnedShape != null)
      {
        if (OwnedShape is Line line)
        {
          line.X1 = position.X;
          line.Y1 = position.Y;
          line.X2 = position2.X;
          line.Y2 = position2.Y;
        }
        else
        {
          Canvas.SetLeft(OwnedShape, position.X);
          Canvas.SetTop(OwnedShape, position.Y);
        }

        if (FollowItem != null)
        {
          OwnedShape.Tag.ToType<XArrow>().SetArrowPosition();
        }
      }

      if (OwnedControl != null)
      {
        Canvas.SetLeft((UIElement)OwnedControl, position.X);
        Canvas.SetTop((UIElement)OwnedControl, position.Y);
      }
    }
  }
}