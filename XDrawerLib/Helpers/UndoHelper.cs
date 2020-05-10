using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using XDrawerLib.Drawers;

namespace XDrawerLib.Helpers
{
  public static class UndoHelper
  {
    private static int CurrentIndex;

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

    public static void AddStep(ActionType type, XShape o, Point preLocation = new Point(), Size preSize = new Size(), DrawerStyle preStyle = null)
    {
      var action = new DrawAction();
      action.Type = type;
      action.NextProps = new NextProps();
      action.NextProps.Location = new Point(Canvas.GetLeft(o.OwnedShape), Canvas.GetTop(o.OwnedShape));
      action.NextProps.Size = new Size(o.OwnedShape.ActualWidth, o.OwnedShape.Height);
      action.NextProps.Style = new DrawerStyle(o.Style);

      action.PreviousProps = new PreviousProps();
      action.PreviousProps.Location = preLocation;
      action.PreviousProps.Size = preSize;
      action.PreviousProps.Style = preStyle;

      action.Object = o;

      Steps.Add(action);

      CurrentIndex = Steps.Count - 1;
    }

    public static void Undo()
    {
      if (CurrentIndex == -1) return;

      var lastAction = Steps[CurrentIndex];

      if (lastAction.Type == ActionType.Create)
      {
        Selector.DeleteObject(lastAction.Object);
      }
      else if (lastAction.Type == ActionType.Move)
      {
        lastAction.Object.SetPosition(lastAction.PreviousProps.Location);
      }
      else if (lastAction.Type == ActionType.SetStyle)
      {
        lastAction.Object.Style = lastAction.PreviousProps.Style;
      }
      else if (lastAction.Type == ActionType.Resize)
      {
        lastAction.Object.OwnedShape.Width = lastAction.PreviousProps.Size.Width;
        lastAction.Object.OwnedShape.Height = lastAction.PreviousProps.Size.Height;
      }

      if (CurrentIndex > -1) CurrentIndex--;
    }

    public static void Redo()
    {
      if (CurrentIndex == Steps.Count - 1) return;

      if (CurrentIndex + 1 < Steps.Count)
      {
        CurrentIndex++;
      }

      var lastAction = Steps[CurrentIndex];

      if (lastAction.Type == ActionType.Create)
      {
        Drawer.Page.Children.Add(lastAction.Object.OwnedShape);
        Drawer.Objects.Add(lastAction.Object.Id, lastAction.Object);
      }
      else if (lastAction.Type == ActionType.Move)
      {
        lastAction.Object.SetPosition(lastAction.NextProps.Location);
      }
      else if (lastAction.Type == ActionType.SetStyle)
      {
        lastAction.Object.Style = lastAction.NextProps.Style;
      }
      else if (lastAction.Type == ActionType.Resize)
      {
        lastAction.Object.OwnedShape.Width = lastAction.NextProps.Size.Width;
        lastAction.Object.OwnedShape.Height = lastAction.NextProps.Size.Height;
      }
    }
  }

  public class DrawAction
  {
    public UndoHelper.ActionType Type { get; set; }
    public PreviousProps PreviousProps { get; set; }
    public NextProps NextProps { get; set; }
    public XShape Object { get; set; }
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