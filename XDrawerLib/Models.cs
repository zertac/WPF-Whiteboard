using System.Windows.Input;
using System.Windows.Media;

namespace XDrawerLib
{
  public enum Tool
  {
    None,
    Selection,
    Ink,
    Line,
    Rectangle,
    Ellipse,
    Text,
    MoveResize,
    Triangle,
    Arrow,
    Custom
  }

  public enum KeyFunction
  {
    None,
    Selection,
    Pan,
    Ink,
    Line,
    Rectangle,
    Ellipse,
    Text,
    Triangle,
    Arrow,
    Custom,
    PreserveSize,
    Cancel,
    Delete,
    Undo,
    Redo
  }

  public class DrawerStyle
  {
    public Brush Background { get; set; }
    public Brush Border { get; set; }
    public double BorderSize { get; set; }
    public double FontSize { get; set; }
    public double Opacity { get; set; }

    public DrawerStyle(DrawerStyle style)
    {
      this.Background = style.Background;
      this.Border = style.Border;
      this.BorderSize = style.BorderSize;
      this.FontSize = style.FontSize;
      this.Opacity = style.Opacity;
    }

    public DrawerStyle()
    {

    }
  }

  public class HotKey
  {
    public Key PrimaryKey { get; set; }
    public Key SecondaryKey { get; set; }
  }
}