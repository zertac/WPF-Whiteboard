using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace XDrawerLib.Helpers
{
  public class HotKeyHelper
  {
    public Dictionary<KeyFunction, HotKey> Shortcuts;
    public Drawer Drawer;

    public HotKeyHelper(Drawer drawer)
    {
      Drawer = drawer;

      Shortcuts = new Dictionary<KeyFunction, HotKey>();
      SetDefaultKeys();
    }

    private void SetDefaultKeys()
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
      Shortcuts.Add(KeyFunction.Cancel, new HotKey { PrimaryKey = Key.Escape });
      Shortcuts.Add(KeyFunction.Delete, new HotKey { PrimaryKey = Key.T });
      Shortcuts.Add(KeyFunction.Undo, new HotKey { PrimaryKey = Key.LeftCtrl, SecondaryKey = Key.Z });
      Shortcuts.Add(KeyFunction.Redo, new HotKey { PrimaryKey = Key.LeftCtrl, SecondaryKey = Key.Y });
    }

    public bool IsPreserveSize()
    {
      if (Keyboard.IsKeyDown(Shortcuts[KeyFunction.PreserveSize].PrimaryKey))
      {
        return true;
      }

      return false;
    }

    public void ExecuteShortcut()
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
        else if (function.Key == KeyFunction.Cancel)
        {
          Drawer.CancelDrawing();
        }
        else if (function.Key == KeyFunction.Delete)
        {
          Drawer.Selector.DeleteSelected();
        }
        else if (function.Key == KeyFunction.Undo)
        {
          Drawer.UndoHelper.Undo();
        }
        else if (function.Key == KeyFunction.Redo)
        {
          Drawer.UndoHelper.Redo();
        }
      }
    }

    public void Bind(KeyFunction function, Key primaryKey, Key secondaryKey = Key.None)
    {
      if (Shortcuts.ContainsKey(function))
      {
        if (primaryKey != Key.None)
        {
          Shortcuts[function].PrimaryKey = primaryKey;
        }

        if (secondaryKey != Key.None)
        {
          Shortcuts[function].SecondaryKey = secondaryKey;
        }
      }
    }
  }
}
