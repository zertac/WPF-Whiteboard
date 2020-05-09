using System;
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
    internal bool FollowItem;
    public DrawerStyle Style
    {
      get => _style;
      set
      {
        _style = value;

        //if (!(OwnedShape is Line))
        //{
        //  if (OwnedShape == null) return;
        //  OwnedShape.Fill = _style.Background;
        //}

        if (OwnedShape != null)
        {
          OwnedShape.Stroke = _style.Border;
          OwnedShape.StrokeThickness = _style.BorderSize;
          OwnedShape.Opacity = _style.Opacity;
        }

        if (OwnedControl != null)
        {
          if (OwnedControl is RichTextBox txt)
          {
            txt.BorderBrush = _style.Border;
            txt.BorderThickness = new Thickness(_style.BorderSize);
            txt.Opacity = _style.Opacity;
          }
        }
      }
    }

    public XShape()
    {
      Id = Guid.NewGuid().ToString();

      AdornerHelper.RemoveAllAdorners();

      Selector.DeselectAll();

      Drawer.ActiveObject = this;
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
        e.Handled = true;
      }

      if (Drawer.DrawType != Type.None && Drawer.DrawType != Type.MoveResize) return;

      if (Drawer.IsEditMode) return;

      AdornerHelper.RemoveAllAdorners();
      Selector.DeselectAll();

      IsSelected = true;

      if (FollowItem)
      {
        AdornerHelper.AddAdorner(sender, this);
      }
      else
      {
        AdornerHelper.AddAdorner(sender);
      }

      Drawer.ActiveObject = this;
    }

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

      Drawer.DrawType = Drawer.ContinuousDraw ? Drawer.DrawType : Type.None;
      Drawer.IsObjectCreating = false;
      Drawer.IsDrawEnded = true;

      Style = Style;
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

    public void SetTextStyle(DependencyProperty property, object value)
    {
      var txt = OwnedControl as RichTextBox;

      var ts = txt.Selection;
      ts.ApplyPropertyValue(property, value);
      txt.Focus();
    }
  }
}