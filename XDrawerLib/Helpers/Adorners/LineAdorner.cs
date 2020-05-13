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
    private readonly Thumb _startThumb;
    private readonly Thumb _endThumb;
    private Line _selectedLine;
    private readonly VisualCollection _visualChildren;

    public object FollowItem;

    private FrameworkElementFactory GetThumbTemple(Brush back)
    {
      back.Opacity = 1;
      var fef = new FrameworkElementFactory(typeof(Ellipse));
      fef.SetValue(Shape.FillProperty, back);
      fef.SetValue(Shape.StrokeProperty, Brushes.Green);
      fef.SetValue(Shape.StrokeThicknessProperty, (double)1);
      return fef;
    }

    private Point _firstPosition1;
    private Point _firstPosition2;

    public LineAdorner(UIElement adornedElement) : base(adornedElement)
    {
      _visualChildren = new VisualCollection(this);

      _startThumb = new Thumb()
      {
        Width = 16,
        Height = 16,
        Cursor = Cursors.Hand,
        Template = new ControlTemplate(typeof(Thumb))
        {
          VisualTree = GetThumbTemple(new SolidColorBrush(Colors.White))
        }
      };
      _endThumb = new Thumb()
      {
        Width = 16,
        Height = 16,
        Cursor = Cursors.Hand,
        Template = new ControlTemplate(typeof(Thumb))
        {
          VisualTree = GetThumbTemple(new SolidColorBrush(Colors.White))
        }
      };

      _startThumb.DragStarted += _startThumb_DragStarted;
      _startThumb.DragCompleted += _startThumb_DragCompleted;
      _startThumb.DragDelta += StartDragDelta;
      _endThumb.DragDelta += EndDragDelta;

      _visualChildren.Add(_startThumb);
      _visualChildren.Add(_endThumb);

      _selectedLine = AdornedElement as Line;
    }

    private void _startThumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
      if (AdornedElement is Line element)
      {
        UndoHelper.AddStep(UndoHelper.ActionType.Move,
          element,
          _firstPosition1,
          element.RenderSize, null, _firstPosition2);
      }
    }

    private void _startThumb_DragStarted(object sender, DragStartedEventArgs e)
    {
      var element = AdornedElement as Line;

      _firstPosition1 = new Point(element.X1, element.Y1);
      _firstPosition2 = new Point(element.X2, element.Y2);
    }

    private void StartDragDelta(object sender, DragDeltaEventArgs e)
    {
      var position = Mouse.GetPosition(this);

      _selectedLine.X1 = position.X;
      _selectedLine.Y1 = position.Y;
    }

    // Event for the Thumb End Point
    private void EndDragDelta(object sender, DragDeltaEventArgs e)
    {
      var position = Mouse.GetPosition(this);

      _selectedLine.X2 = position.X;
      _selectedLine.Y2 = position.Y;
    }

    protected override int VisualChildrenCount { get { return _visualChildren.Count; } }
    protected override Visual GetVisualChild(int index) { return _visualChildren[index]; }

    protected override void OnRender(DrawingContext drawingContext)
    {
      if (AdornedElement is Line)
      {
        _selectedLine = AdornedElement as Line;
        var point = new Point(_selectedLine.X1, _selectedLine.Y1);
        var point1 = new Point(_selectedLine.X2, _selectedLine.Y2);
      }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      _selectedLine = AdornedElement as Line;

      if (_selectedLine != null)
      {
        var startRect = new Rect(_selectedLine.X1 - (_startThumb.Width / 2), _selectedLine.Y1 - (_startThumb.Width / 2), _startThumb.Width, _startThumb.Height);
        _startThumb.Arrange(startRect);
      }

      if (_selectedLine != null)
      {
        var endRect = new Rect(_selectedLine.X2 - (_endThumb.Width / 2), _selectedLine.Y2 - (_endThumb.Height / 2), _endThumb.Width, _endThumb.Height);
        _endThumb.Arrange(endRect);
      }

      if (FollowItem != null)
      {
        var f = (XArrow)FollowItem;
        f.SetArrowPosition();
      }

      return finalSize;
    }
  }
}
