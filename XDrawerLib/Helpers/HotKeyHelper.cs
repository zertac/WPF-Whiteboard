using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace XDrawerLib.Helpers
{
  public static class HotKeyHelper
  {
    public static Dictionary<KeyFunction, HotKey> Shortcuts;

    static HotKeyHelper()
    {
      Shortcuts = new Dictionary<KeyFunction, HotKey>();
      SetDefaultKeys();
    }

    private static void SetDefaultKeys()
    {
      Shortcuts.Add(KeyFunction.Ink, new HotKey { PrimaryKey = Key.LeftCtrl, SecondaryKey = Key.D });
      Shortcuts.Add(KeyFunction.Line, new HotKey { PrimaryKey = Key.LeftCtrl, SecondaryKey = Key.L });
      Shortcuts.Add(KeyFunction.Rectangle, new HotKey { PrimaryKey = Key.LeftCtrl, SecondaryKey = Key.R });
      Shortcuts.Add(KeyFunction.Ellipse, new HotKey { PrimaryKey = Key.LeftCtrl, SecondaryKey = Key.E });
      Shortcuts.Add(KeyFunction.Text, new HotKey { PrimaryKey = Key.LeftCtrl, SecondaryKey = Key.T });
      Shortcuts.Add(KeyFunction.Triangle, new HotKey { PrimaryKey = Key.LeftCtrl, SecondaryKey = Key.N });
      Shortcuts.Add(KeyFunction.Arrow, new HotKey { PrimaryKey = Key.LeftCtrl, SecondaryKey = Key.P });
      Shortcuts.Add(KeyFunction.Custom, new HotKey { PrimaryKey = Key.LeftCtrl, SecondaryKey = Key.H });
      Shortcuts.Add(KeyFunction.PreserveSize, new HotKey { PrimaryKey = Key.LeftShift });
      Shortcuts.Add(KeyFunction.Cancel, new HotKey { PrimaryKey = Key.Escape, SecondaryKey = Key.None });
      Shortcuts.Add(KeyFunction.Delete, new HotKey { PrimaryKey = Key.T });
      Shortcuts.Add(KeyFunction.Undo, new HotKey { PrimaryKey = Key.LeftCtrl, SecondaryKey = Key.Z });
      Shortcuts.Add(KeyFunction.Redo, new HotKey { PrimaryKey = Key.LeftCtrl, SecondaryKey = Key.Y });
    }

    public static void ExecuteShortcut()
    {
      var tmp = Shortcuts.Where(x => Keyboard.IsKeyDown(x.Value.PrimaryKey)).ToList();
      var founded = tmp.Count(x =>
        (x.Value.SecondaryKey != Key.None && Keyboard.IsKeyDown(x.Value.SecondaryKey)) ||
        x.Value.SecondaryKey == Key.None) > 0;

      if (founded)
      {
        var function = tmp.Single(x =>
          (x.Value.SecondaryKey != Key.None && Keyboard.IsKeyDown(x.Value.SecondaryKey)) ||
          x.Value.SecondaryKey == Key.None);

        if (function.Key == KeyFunction.Ink)
        {
          Drawer.DrawTool = Tool.Ink;
        }
        else if (function.Key == KeyFunction.Line)
        {
          Drawer.DrawTool = Tool.Line;
        }
        else if (function.Key == KeyFunction.Rectangle)
        {
          Drawer.DrawTool = Tool.Rectangle;
        }
        else if (function.Key == KeyFunction.Ellipse)
        {
          Drawer.DrawTool = Tool.Ellipse;
        }
        else if (function.Key == KeyFunction.Text)
        {
          Drawer.DrawTool = Tool.Text;
        }
        else if (function.Key == KeyFunction.Triangle)
        {
          Drawer.DrawTool = Tool.Triangle;
        }
        else if (function.Key == KeyFunction.Arrow)
        {
          Drawer.DrawTool = Tool.Arrow;
        }
        else if (function.Key == KeyFunction.Custom)
        {
          Drawer.DrawTool = Tool.Custom;
        }
        else if (function.Key == KeyFunction.PreserveSize)
        {

        }
        else if (function.Key == KeyFunction.Cancel)
        {

        }
        else if (function.Key == KeyFunction.Delete)
        {
          Selector.DeleteSelected();
        }
        else if (function.Key == KeyFunction.Undo)
        {
          UndoHelper.Undo();
        }
        else if (function.Key == KeyFunction.Redo)
        {
          UndoHelper.Redo();
        }
      }
    }
  }
}
