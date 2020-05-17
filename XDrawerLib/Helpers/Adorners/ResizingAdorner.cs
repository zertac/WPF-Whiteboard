using System;
using System.ComponentModel;
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
  public class ResizingAdorner : Adorner
  {
    public Drawer Drawer;

    const double THUMB_SIZE = 16;
    const double MINIMAL_SIZE = 20;
    const double MOVE_OFFSET = 20;


    //9 thumbs
    /*                        moveAndRotateThumb
     *                              *
     *                              *
     * topLeftThumb*************topMiddleThumb**************topRightThumb        
     *      *                                                    *
     *      *                                                    *
     *      *                                                    *
     * middleLeftThumb                                     middleRightThumb
     *      *                                                    *
     *      *                                                    *
     *      *                                                    * 
     * bottomLeftThumb*********bottomMiddleThumb**************bottomRightThumb
     * 
     * */
    Thumb _moveThumb,
      _topLeftThumb, 
      _middleLeftThumb,
      _bottomLeftThumb,
      _topMiddleThumb,
      _topRightThumb,
      _middleRightThumb,
      _bottomRightThumb,
      _bottomMiddleThumb;

    Rectangle _thumbRectangle;

    VisualCollection _visualCollection;

    private readonly bool _showResizeBorder;

    public ResizingAdorner(UIElement adorned) : base(adorned)
    {
      SetDefaults();

      _showResizeBorder = !(adorned is RichTextBox);
    }

    private void SetDefaults()
    {
      _visualCollection = new VisualCollection(this);

      _visualCollection.Add(_thumbRectangle = GetResizeRectangle());
      _visualCollection.Add(_moveThumb = GetMoveAndRotateThumb());

      _visualCollection.Add(_topLeftThumb = GetResizeThumb(Cursors.SizeNWSE, HorizontalAlignment.Left, VerticalAlignment.Top));
      _visualCollection.Add(_middleLeftThumb = GetResizeThumb(Cursors.SizeWE, HorizontalAlignment.Left, VerticalAlignment.Center));
      _visualCollection.Add(_bottomLeftThumb = GetResizeThumb(Cursors.SizeNESW, HorizontalAlignment.Left, VerticalAlignment.Bottom));

      _visualCollection.Add(_topRightThumb = GetResizeThumb(Cursors.SizeNESW, HorizontalAlignment.Right, VerticalAlignment.Top));
      _visualCollection.Add(_middleRightThumb = GetResizeThumb(Cursors.SizeWE, HorizontalAlignment.Right, VerticalAlignment.Center));
      _visualCollection.Add(_bottomRightThumb = GetResizeThumb(Cursors.SizeNWSE, HorizontalAlignment.Right, VerticalAlignment.Bottom));

      _visualCollection.Add(_topMiddleThumb = GetResizeThumb(Cursors.SizeNS, HorizontalAlignment.Center, VerticalAlignment.Top));
      _visualCollection.Add(_bottomMiddleThumb = GetResizeThumb(Cursors.SizeNS, HorizontalAlignment.Center, VerticalAlignment.Bottom));
    }

    private Rectangle GetResizeRectangle()
    {
      var rectangle = new Rectangle()
      {
        Width = AdornedElement.RenderSize.Width,
        Height = AdornedElement.RenderSize.Height,
        Fill = Brushes.Transparent,
        Stroke = Brushes.Blue,
        StrokeThickness = (double)1
      };

      return rectangle;
    }

    private Size _firstSize;

    private Thumb GetResizeThumb(Cursor cur, HorizontalAlignment horizontal, VerticalAlignment vertical)
    {
      var thumb = new Thumb()
      {
        Width = THUMB_SIZE,
        Height = THUMB_SIZE,
        HorizontalAlignment = horizontal,
        VerticalAlignment = vertical,
        Cursor = cur,
        Template = new ControlTemplate(typeof(Thumb))
        {
          VisualTree = GetThumbTemple(new SolidColorBrush(Colors.White))
        }
      };
      thumb.DragStarted += Thumb_DragStarted1;
      thumb.DragCompleted += Thumb_DragCompleted1;
      thumb.DragDelta += (s, e) =>
      {
        var element = AdornedElement as FrameworkElement;

        if (element == null)
          return;

        this.ElementResize(element);

        switch (thumb.VerticalAlignment)
        {
          case VerticalAlignment.Bottom:
            if (element.Height + e.VerticalChange > MINIMAL_SIZE)
            {
              element.Height += e.VerticalChange;
              _thumbRectangle.Height += e.VerticalChange;
            }
            break;
          case VerticalAlignment.Top:
            if (element.Height - e.VerticalChange > MINIMAL_SIZE)
            {
              element.Height -= e.VerticalChange;
              _thumbRectangle.Height -= e.VerticalChange;

              Canvas.SetTop(element, Canvas.GetTop(element) + e.VerticalChange);
            }
            break;
        }
        switch (thumb.HorizontalAlignment)
        {
          case HorizontalAlignment.Left:
            if (element.Width - e.HorizontalChange > MINIMAL_SIZE)
            {
              element.Width -= e.HorizontalChange;
              var m = _thumbRectangle.Width - e.HorizontalChange;

              _thumbRectangle.Width = Math.Abs(m);
              Canvas.SetLeft(element, Canvas.GetLeft(element) + e.HorizontalChange);
            }
            break;
          case HorizontalAlignment.Right:
            if (element.Width + e.HorizontalChange > MINIMAL_SIZE)
            {
              element.Width += e.HorizontalChange;
              var m = _thumbRectangle.Width + e.HorizontalChange;
              _thumbRectangle.Width = Math.Abs(m);
            }
            break;
        }

        e.Handled = true;
      };

      return thumb;
    }

    private void Thumb_DragCompleted1(object sender, DragCompletedEventArgs e)
    {
      if (AdornedElement is FrameworkElement element)
      {
        Drawer.UndoHelper.AddStep(UndoHelper.ActionType.Resize, element, new Point(), _firstSize);
      }
    }

    private void Thumb_DragStarted1(object sender, DragStartedEventArgs e)
    {
      _firstSize = AdornedElement.RenderSize;
    }

    private void ElementResize(FrameworkElement frameworkElement)
    {
      if (double.IsNaN(frameworkElement.Width))
        frameworkElement.Width = frameworkElement.RenderSize.Width;
      if (double.IsNaN(frameworkElement.Height))
        frameworkElement.Height = frameworkElement.RenderSize.Height;
    }

    // get Thumb Temple
    private FrameworkElementFactory GetThumbTemple(Brush back)
    {
      back.Opacity = 1;
      var fef = new FrameworkElementFactory(typeof(Ellipse));
      fef.SetValue(Shape.FillProperty, back);
      fef.SetValue(Shape.StrokeProperty, Brushes.Green);
      fef.SetValue(Shape.StrokeThicknessProperty, (double)1);
      return fef;
    }

    private FrameworkElementFactory GetThumbTemple2(Brush back)
    {
      back.Opacity = 0;
      var fef = new FrameworkElementFactory(typeof(Rectangle));
      fef.SetValue(Shape.FillProperty, back);
      return fef;
    }

    private Point _firstPosition;

    private Thumb GetMoveAndRotateThumb()
    {
      var thumb = new Thumb()
      {
        Width = 200,
        Height = 200,
        Cursor = Cursors.Hand,
        Template = new ControlTemplate(typeof(Thumb))
        {
          VisualTree = GetThumbTemple2(GetRectangleBack())
        }
      };
      thumb.DragStarted += Thumb_DragStarted;
      thumb.DragCompleted += Thumb_DragCompleted;
      thumb.DragDelta += (s, e) =>
      {
        var element = AdornedElement as FrameworkElement;
        if (element == null)
          return;

        Canvas.SetLeft(element, Canvas.GetLeft(element) + e.HorizontalChange);
        Canvas.SetTop(element, Canvas.GetTop(element) + e.VerticalChange);
      };

      thumb.MouseDoubleClick += Thumb_MouseDoubleClick;

      return thumb;
    }

    private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
      if (AdornedElement is FrameworkElement element)
      {
        Drawer.UndoHelper.AddStep(UndoHelper.ActionType.Move, element, _firstPosition);
      }
    }

    private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
    {
      _firstPosition = new Point(Canvas.GetLeft(AdornedElement), Canvas.GetTop(AdornedElement));
    }

    private void Thumb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (AdornedElement is FrameworkElement element)
      {
        if (element is RichTextBox)
        {
          _moveThumb.Visibility = Visibility.Hidden;
        }

        element.Tag.ToType<XShape>().OnDoubleClick?.Invoke();
      }
    }

    private Brush GetMoveEllipseBack()
    {
      var lan = "M841.142857 570.514286c0 168.228571-153.6 336.457143-329.142857 336.457143s-329.142857-153.6-329.142857-336.457143c0-182.857143 153.6-336.457143 329.142857-336.457143v117.028571l277.942857-168.228571L512 0v117.028571c-241.371429 0-438.857143 197.485714-438.857143 453.485715S270.628571 1024 512 1024s438.857143-168.228571 438.857143-453.485714h-109.714286z m0 0";
      var converter = TypeDescriptor.GetConverter(typeof(Geometry));
      var geometry = (Geometry)converter.ConvertFrom(lan);
      TileBrush bsh = new DrawingBrush(new GeometryDrawing(Brushes.Transparent, new Pen(Brushes.Black, 2), geometry));
      bsh.Stretch = Stretch.Fill;
      return bsh;
    }

    private Brush GetRectangleBack()
    {
      var lan = "M22,4H2v16h20V4L22,4z";
      var converter = TypeDescriptor.GetConverter(typeof(Geometry));
      var geometry = (Geometry)converter.ConvertFrom(lan);
      TileBrush bsh = new DrawingBrush(new GeometryDrawing(Brushes.Transparent, new Pen(Brushes.Black, 2), geometry));
      bsh.Stretch = Stretch.Fill;
      return bsh;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      var offset = (THUMB_SIZE / 2);
      var sz = new Size(THUMB_SIZE, THUMB_SIZE);

      _topLeftThumb.Arrange(new Rect(new Point(-offset, -offset), sz));
      _topMiddleThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width / 2 - THUMB_SIZE / 2, -offset), sz));
      _topRightThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, -offset), sz));

      _bottomLeftThumb.Arrange(new Rect(new Point(-offset, AdornedElement.RenderSize.Height - offset), sz));
      _bottomMiddleThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width / 2 - THUMB_SIZE / 2, AdornedElement.RenderSize.Height - offset), sz));
      _bottomRightThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, AdornedElement.RenderSize.Height - offset), sz));

      _middleLeftThumb.Arrange(new Rect(new Point(-offset, AdornedElement.RenderSize.Height / 2 - THUMB_SIZE / 2), sz));
      _middleRightThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, AdornedElement.RenderSize.Height / 2 - THUMB_SIZE / 2), sz));

      if (_showResizeBorder)
      {
        _thumbRectangle.Arrange(new Rect(new Point(-offset, -offset), new Size(Width = AdornedElement.RenderSize.Width + THUMB_SIZE, Height = AdornedElement.RenderSize.Height + THUMB_SIZE)));
      }

      _moveThumb.Width = finalSize.Width;
      _moveThumb.Height = finalSize.Height;

      var r = new Rect(0, 0, finalSize.Width - offset, finalSize.Height - offset);
      _moveThumb.Arrange(r);

      return finalSize;
    }

    protected override Visual GetVisualChild(int index)
    {
      return _visualCollection[index];
    }

    protected override int VisualChildrenCount => _visualCollection.Count;


    public void OnChange()
    {
     
    }
  }
}