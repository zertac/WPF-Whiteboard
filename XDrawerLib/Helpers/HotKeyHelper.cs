using System.Collections.Generic;
using System.Windows.Input;

namespace XDrawerLib.Helpers
{
  public static class HotKeyHelper
  {
    public static Dictionary<KeyFunction, HotKey> Shortcuts;

    static HotKeyHelper()
    {
      Shortcuts = new Dictionary<KeyFunction, HotKey>();
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
      Shortcuts.Add(KeyFunction.PreserveSize, new HotKey { PrimaryKey = Key.LeftCtrl });
      Shortcuts.Add(KeyFunction.Cancel, new HotKey { PrimaryKey = Key.Escape });
    }

    public static void ExecuteShortcut()
    {
      if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.D))
      {
        Drawer.DrawTool = Tool.Ink;
      }
    }
  }
}
