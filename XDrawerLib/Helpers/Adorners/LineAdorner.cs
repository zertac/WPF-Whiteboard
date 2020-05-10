using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using XDrawerLib.Drawers;

namespace XDrawerLib.Helpers.Adorners
{
  public class LineAdorner : Adorner
  {
    private Thumb startThumb;
    private Thumb endThumb;
    private Line selectedLine;
    private VisualCollection visualChildren;

    public object FollowItem;

    private FrameworkElementFactory GetThumbTemple(Brush back)
    {
      back.Opacity = 1;
      var fef = new FrameworkElementFactory(typeof(Ellipse));
      fef.SetValue(Ellipse.FillProperty, back);
      fef.SetValue(Ellipse.StrokeProperty, Brushes.Green);
      fef.SetValue(Ellipse.StrokeThicknessProperty, (double)1);
      return fef;
    }

    public LineAdorner(UIElement adornedElement) : base(adornedElement)
    {
      visualChildren = new VisualCollection(this);

      startThumb = new Thumb()
      {
        //Background = Brushes.Red,
        Width = 16,
        Height = 16,
        Cursor = Cursors.Hand,
        Template = new ControlTemplate(typeof(Thumb))
        {
          VisualTree = GetThumbTemple(new SolidColorBrush(Colors.White))
        }
      };
      endThumb = new Thumb()
      {
        //Background = Brushes.Red,
        Width = 16,
        Height = 16,
        Cursor = Cursors.Hand,
        Template = new ControlTemplate(typeof(Thumb))
        {
          VisualTree = GetThumbTemple(new SolidColorBrush(Colors.White))
        }
      };

      startThumb.DragDelta += StartDragDelta;
      endThumb.DragDelta += EndDragDelta;

      visualChildren.Add(startThumb);
      visualChildren.Add(endThumb);

      selectedLine = AdornedElement as Line;
    }

    private void StartDragDelta(object sender, DragDeltaEventArgs e)
    {
      Point position = Mouse.GetPosition(this);

      selectedLine.X1 = position.X;
      selectedLine.Y1 = position.Y;
    }

    // Event for the Thumb End Point
    private void EndDragDelta(object sender, DragDeltaEventArgs e)
    {
      Point position = Mouse.GetPosition(this);

      selectedLine.X2 = position.X;
      selectedLine.Y2 = position.Y;
    }

    protected override int VisualChildrenCount { get { return visualChildren.Count; } }
    protected override Visual GetVisualChild(int index) { return visualChildren[index]; }

    protected override void OnRender(DrawingContext drawingContext)
    {
      if (AdornedElement is Line)
      {
        selectedLine = AdornedElement as Line;
        new Point(selectedLine.X1, selectedLine.Y1);
        new Point(selectedLine.X2, selectedLine.Y2);
      }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      selectedLine = AdornedElement as Line;

      double left = Math.Min(selectedLine.X1, selectedLine.X2);
      double top = Math.Min(selectedLine.Y1, selectedLine.Y2);

      var startRect = new Rect(selectedLine.X1 - (startThumb.Width / 2), selectedLine.Y1 - (startThumb.Width / 2), startThumb.Width, startThumb.Height);
      startThumb.Arrange(startRect);

      var endRect = new Rect(selectedLine.X2 - (endThumb.Width / 2), selectedLine.Y2 - (endThumb.Height / 2), endThumb.Width, endThumb.Height);
      endThumb.Arrange(endRect);

      if (FollowItem != null)
      {
        var f = (XArrow)FollowItem;
        f.SetArrowPosition();
      }

      return finalSize;
    }
  }
}
