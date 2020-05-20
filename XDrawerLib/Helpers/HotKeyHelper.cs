using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
      Shortcuts.Add(KeyFunction.Delete, new HotKey { PrimaryKey = Key.LeftAlt, SecondaryKey = Key.T });
      Shortcuts.Add(KeyFunction.Undo, new HotKey { PrimaryKey = Key.LeftCtrl, SecondaryKey = Key.Z });
      Shortcuts.Add(KeyFunction.Redo, new HotKey { PrimaryKey = Key.LeftCtrl, SecondaryKey = Key.Y });
      Shortcuts.Add(KeyFunction.Selection, new HotKey { PrimaryKey = Key.LeftCtrl, SecondaryKey = Key.Q });
      Shortcuts.Add(KeyFunction.None, new HotKey { PrimaryKey = Key.LeftCtrl });
      Shortcuts.Add(KeyFunction.Pan, new HotKey { PrimaryKey = Key.LeftAlt });
      Shortcuts.Add(KeyFunction.SelectAll, new HotKey { PrimaryKey = Key.LeftAlt, SecondaryKey = Key.B });
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
      Console.WriteLine("pressed");
      var tmp = Shortcuts.Where(x => Keyboard.IsKeyDown(x.Value.PrimaryKey)).ToList();
      var founded = tmp.Count(x =>
        (x.Value.SecondaryKey != Key.None && Keyboard.IsKeyDown(x.Value.SecondaryKey)) ||
        x.Value.SecondaryKey == Key.None) > 0;

      var total = tmp.Count(x =>
        (x.Value.SecondaryKey != Key.None && Keyboard.IsKeyDown(x.Value.SecondaryKey)) ||
        x.Value.SecondaryKey == Key.None);


      if (founded)
      {
       
        var function = tmp.FirstOrDefault(x =>
          (x.Value.SecondaryKey != Key.None && Keyboard.IsKeyDown(x.Value.SecondaryKey)) ||
          x.Value.SecondaryKey == Key.None);

        if (function.Key == KeyFunction.None)
        {
          Drawer.DrawTool = Tool.None;
        }
        else if (function.Key == KeyFunction.Selection)
        {
          Drawer.DrawTool = Tool.Selection;
          Console.WriteLine("selection");
        }
        if (function.Key == KeyFunction.Pan)
        {
          Drawer.DrawTool = Tool.None;
          Console.WriteLine("pan");
        }
        else if (function.Key == KeyFunction.Ink)
        {
          Drawer.DrawTool = Tool.Ink;
          Console.WriteLine("ink");
        }
        else if (function.Key == KeyFunction.Line)
        {
          Drawer.DrawTool = Tool.Line;
          Console.WriteLine("line");
        }
        else if (function.Key == KeyFunction.Rectangle)
        {
          Drawer.DrawTool = Tool.Rectangle;
          Console.WriteLine("rectangle");
        }
        else if (function.Key == KeyFunction.Ellipse)
        {
          Drawer.DrawTool = Tool.Ellipse;
          Console.WriteLine("ellipse");
        }
        else if (function.Key == KeyFunction.Text)
        {
          Drawer.DrawTool = Tool.Text;
          Console.WriteLine("text");
        }
        else if (function.Key == KeyFunction.Triangle)
        {
          Drawer.DrawTool = Tool.Triangle;
          Console.WriteLine("triange");
        }
        else if (function.Key == KeyFunction.Arrow)
        {
          Drawer.DrawTool = Tool.Arrow;
          Console.WriteLine("arrow");
        }
        else if (function.Key == KeyFunction.Custom)
        {
          Drawer.DrawTool = Tool.Custom;
          Console.WriteLine("custom");
        }
        else if (function.Key == KeyFunction.Cancel)
        {
          Drawer.CancelDrawing();
          Console.WriteLine("cancel");
        }
        else if (function.Key == KeyFunction.Delete)
        {
          Drawer.Selector.DeleteSelected();
          Console.WriteLine("delete");
        }
        else if (function.Key == KeyFunction.Undo)
        {
          Drawer.UndoHelper.Undo();
          Console.WriteLine("undo");
        }
        else if (function.Key == KeyFunction.Redo)
        {
          Drawer.UndoHelper.Redo();
          Console.WriteLine("redo");
        }
        else if (function.Key == KeyFunction.SelectAll)
        {
          Drawer.Selector.SelectAll();
          Console.WriteLine("select all");
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
