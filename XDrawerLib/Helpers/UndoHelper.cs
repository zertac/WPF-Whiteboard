using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using XDrawerLib.Drawers;

namespace XDrawerLib.Helpers
{
  public static class UndoHelper
  {
    private static int _currentIndex;

    public enum ActionType
    {
      Create,
      Delete,
      Move,
      SetStyle,
      Resize
    }

    public static List<DrawAction> Steps;

    static UndoHelper()
    {
      Steps = new List<DrawAction>();
    }

    public static void AddStep(ActionType type, FrameworkElement ctrl, Point preLocation = new Point(), Size preSize = new Size(), DrawerStyle preStyle = null)
    {
      var o = ctrl.Tag.ToType<XShape>();

      var action = new DrawAction();
      action.Type = type;
      action.NextProps = new NextProps();
      if (o.OwnedShape != null)
      {
        action.NextProps.Location = new Point(Canvas.GetLeft(o.OwnedShape), Canvas.GetTop(o.OwnedShape));
        action.NextProps.Size = new Size(o.OwnedShape.ActualWidth, o.OwnedShape.Height);
      }

      if (o.OwnedControl != null)
      {
        if (o.OwnedControl is List<Border> borders)
        {
          foreach (var b in borders)
          {
            var control = b;

            action.NextProps.Location = new Point(Canvas.GetLeft(control), Canvas.GetTop(control));
            action.NextProps.Size = new Size(control.ActualWidth, control.Height);
          }
        }
        else
        {
          var control = (FrameworkElement)o.OwnedControl;

          action.NextProps.Location = new Point(Canvas.GetLeft(control), Canvas.GetTop(control));
          action.NextProps.Size = new Size(control.ActualWidth, control.Height);
        }
      }

      action.NextProps.Style = new DrawerStyle(o.Style);
      action.PreviousProps = new PreviousProps();
      action.PreviousProps.Location = preLocation;
      action.PreviousProps.Size = preSize;
      action.PreviousProps.Style = preStyle;

      action.Object = ctrl;

      Steps.Add(action);

      _currentIndex = Steps.Count - 1;
    }

    public static void Undo()
    {
      if (_currentIndex == -1) return;

      var lastAction = Steps[_currentIndex];

      if (lastAction.Type == ActionType.Create)
      {
        Selector.DeleteObject(lastAction.Object);
      }
      else if (lastAction.Type == ActionType.Move)
      {
        lastAction.Object.Tag.ToType<XShape>().SetPosition(lastAction.PreviousProps.Location);
      }
      else if (lastAction.Type == ActionType.SetStyle)
      {
        lastAction.Object.Tag.ToType<XShape>().Style = lastAction.PreviousProps.Style;
      }
      else if (lastAction.Type == ActionType.Resize)
      {
        lastAction.Object.Tag.ToType<XShape>().OwnedShape.Width = lastAction.PreviousProps.Size.Width;
        lastAction.Object.Tag.ToType<XShape>().OwnedShape.Height = lastAction.PreviousProps.Size.Height;
      }

      if (_currentIndex > -1) _currentIndex--;
    }

    public static void Redo()
    {
      if (_currentIndex == Steps.Count - 1) return;

      if (_currentIndex + 1 < Steps.Count)
      {
        _currentIndex++;
      }

      var lastAction = Steps[_currentIndex];

      if (lastAction.Type == ActionType.Create)
      {
        var shape = lastAction.Object.Tag.ToType<XShape>();
        if (shape.OwnedShape != null)
        {
          Drawer.Page.Children.Add(shape.OwnedShape);
          Drawer.Objects.Add(shape.Id, shape);
        }

        if (shape.OwnedControl != null)
        {
          if (shape.OwnedControl is List<Border> borders)
          {
            foreach (var b in borders)
            {
              if (b.Uid == lastAction.Object.Uid)
              {
                Drawer.Page.Children.Add(b);
                Drawer.Objects.Add(b.Uid, shape);
                break;
              }
            }
          }
          else
          {
            Drawer.Page.Children.Add((FrameworkElement)shape.OwnedControl);
            Drawer.Objects.Add(shape.Id, shape);
          }
        }
      }
      else if (lastAction.Type == ActionType.Move)
      {
        lastAction.Object.Tag.ToType<XShape>().SetPosition(lastAction.NextProps.Location);
      }
      else if (lastAction.Type == ActionType.SetStyle)
      {
        lastAction.Object.Tag.ToType<XShape>().Style = lastAction.NextProps.Style;
      }
      else if (lastAction.Type == ActionType.Resize)
      {
        lastAction.Object.Tag.ToType<XShape>().OwnedShape.Width = lastAction.NextProps.Size.Width;
        lastAction.Object.Tag.ToType<XShape>().OwnedShape.Height = lastAction.NextProps.Size.Height;
      }
    }
  }

  public class DrawAction
  {
    public UndoHelper.ActionType Type { get; set; }
    public PreviousProps PreviousProps { get; set; }
    public NextProps NextProps { get; set; }
    public FrameworkElement Object { get; set; }
  }

  public class PreviousProps
  {
    public Point Location { get; set; }
    public Size Size { get; set; }
    public DrawerStyle Style { get; set; }
  }

  public class NextProps
  {
    public Point Location { get; set; }
    public Size Size { get; set; }
    public DrawerStyle Style { get; set; }
  }
}