using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using XPDFDoc.Helpers;

namespace XPDFDoc
{
  public class ResizingAdorner : Adorner
  {
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
    Thumb moveThumb, topLeftThumb, middleLeftThumb, bottomLeftThumb, topMiddleThumb, topRightThumb, middleRightThumb, bottomRightThumb, bottomMiddleThumb;

    Rectangle thumbRectangle;

    VisualCollection visualCollection;

    private UIElement mainAdorner;

    public ResizingAdorner(UIElement adorned) : base(adorned)
    {
      SetDefaults();
    }

    private void SetDefaults()
    {
      visualCollection = new VisualCollection(this);

      visualCollection.Add(thumbRectangle = GeteResizeRectangle());
      visualCollection.Add(moveThumb = GetMoveAndRotateThumb());

      visualCollection.Add(topLeftThumb = GetResizeThumb(Cursors.SizeNWSE, HorizontalAlignment.Left, VerticalAlignment.Top));
      visualCollection.Add(middleLeftThumb = GetResizeThumb(Cursors.SizeWE, HorizontalAlignment.Left, VerticalAlignment.Center));
      visualCollection.Add(bottomLeftThumb = GetResizeThumb(Cursors.SizeNESW, HorizontalAlignment.Left, VerticalAlignment.Bottom));

      visualCollection.Add(topRightThumb = GetResizeThumb(Cursors.SizeNESW, HorizontalAlignment.Right, VerticalAlignment.Top));
      visualCollection.Add(middleRightThumb = GetResizeThumb(Cursors.SizeWE, HorizontalAlignment.Right, VerticalAlignment.Center));
      visualCollection.Add(bottomRightThumb = GetResizeThumb(Cursors.SizeNWSE, HorizontalAlignment.Right, VerticalAlignment.Bottom));

      visualCollection.Add(topMiddleThumb = GetResizeThumb(Cursors.SizeNS, HorizontalAlignment.Center, VerticalAlignment.Top));
      visualCollection.Add(bottomMiddleThumb = GetResizeThumb(Cursors.SizeNS, HorizontalAlignment.Center, VerticalAlignment.Bottom));
    }

    private Rectangle GeteResizeRectangle()
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


    private Thumb GetResizeThumb(Cursor cur, HorizontalAlignment horizontal, VerticalAlignment vertical)
    {
      var thumb = new Thumb()
      {
        //Background = Brushes.Red,
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
              thumbRectangle.Height += e.VerticalChange;
            }
            break;

          //case VerticalAlignment.Center:
          //    if ()
          //    {

          //    }
          //    break;
          case VerticalAlignment.Top:
            if (element.Height - e.VerticalChange > MINIMAL_SIZE)
            {
              element.Height -= e.VerticalChange;
              thumbRectangle.Height -= e.VerticalChange;

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
              thumbRectangle.Width -= e.HorizontalChange;
              Canvas.SetLeft(element, Canvas.GetLeft(element) + e.HorizontalChange);
            }
            break;
          //case HorizontalAlignment.Center:
          //    if (element.Width + e.HorizontalChange > MINIMAL_SIZE)
          //    {
          //        element.Width += e.HorizontalChange;
          //    }
          //    break;
          case HorizontalAlignment.Right:
            if (element.Width + e.HorizontalChange > MINIMAL_SIZE)
            {
              element.Width += e.HorizontalChange;
              thumbRectangle.Width += e.HorizontalChange;
            }
            break;
        }

        e.Handled = true;
      };

      return thumb;
    }

    private void ElementResize(FrameworkElement frameworkElement)
    {
      if (Double.IsNaN(frameworkElement.Width))
        frameworkElement.Width = frameworkElement.RenderSize.Width;
      if (Double.IsNaN(frameworkElement.Height))
        frameworkElement.Height = frameworkElement.RenderSize.Height;
    }

    // get Thumb Temple
    private FrameworkElementFactory GetThumbTemple(Brush back)
    {
      back.Opacity = 1;
      var fef = new FrameworkElementFactory(typeof(Ellipse));
      fef.SetValue(Ellipse.FillProperty, back);
      fef.SetValue(Ellipse.StrokeProperty, Brushes.Green);
      fef.SetValue(Ellipse.StrokeThicknessProperty, (double)1);
      return fef;
    }

    private FrameworkElementFactory GetThumbTemple2(Brush back)
    {
      back.Opacity = 0;
      var fef = new FrameworkElementFactory(typeof(Rectangle));
      fef.SetValue(Ellipse.FillProperty, back);
      return fef;
    }

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

    private void Thumb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (AdornedElement is FrameworkElement element)
      {
        //moveThumb.Visibility = Visibility.Hidden;

        element.Tag.ToType<XShape>().OnDoubleClick?.Invoke();
      }
    }

    private Brush GetMoveEllipseBack()
    {
      string lan = "M841.142857 570.514286c0 168.228571-153.6 336.457143-329.142857 336.457143s-329.142857-153.6-329.142857-336.457143c0-182.857143 153.6-336.457143 329.142857-336.457143v117.028571l277.942857-168.228571L512 0v117.028571c-241.371429 0-438.857143 197.485714-438.857143 453.485715S270.628571 1024 512 1024s438.857143-168.228571 438.857143-453.485714h-109.714286z m0 0";
      var converter = TypeDescriptor.GetConverter(typeof(Geometry));
      var geometry = (Geometry)converter.ConvertFrom(lan);
      TileBrush bsh = new DrawingBrush(new GeometryDrawing(Brushes.Transparent, new Pen(Brushes.Black, 2), geometry));
      bsh.Stretch = Stretch.Fill;
      return bsh;
    }

    private Brush GetRectangleBack()
    {
      string lan = "M22,4H2v16h20V4L22,4z";
      var converter = TypeDescriptor.GetConverter(typeof(Geometry));
      var geometry = (Geometry)converter.ConvertFrom(lan);
      TileBrush bsh = new DrawingBrush(new GeometryDrawing(Brushes.Transparent, new Pen(Brushes.Black, 2), geometry));
      bsh.Stretch = Stretch.Fill;
      return bsh;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      double offset = (THUMB_SIZE / 2);
      Size sz = new Size(THUMB_SIZE, THUMB_SIZE);

      topLeftThumb.Arrange(new Rect(new Point(-offset, -offset), sz));
      topMiddleThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width / 2 - THUMB_SIZE / 2, -offset), sz));
      topRightThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, -offset), sz));

      bottomLeftThumb.Arrange(new Rect(new Point(-offset, AdornedElement.RenderSize.Height - offset), sz));
      bottomMiddleThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width / 2 - THUMB_SIZE / 2, AdornedElement.RenderSize.Height - offset), sz));
      bottomRightThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, AdornedElement.RenderSize.Height - offset), sz));

      middleLeftThumb.Arrange(new Rect(new Point(-offset, AdornedElement.RenderSize.Height / 2 - THUMB_SIZE / 2), sz));
      middleRightThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, AdornedElement.RenderSize.Height / 2 - THUMB_SIZE / 2), sz));



      // rectMove.Arrange(new Rect(new Point(-offset, -offset), new Size(Width = AdornedElement.RenderSize.Width + THUMB_SIZE, Height = AdornedElement.RenderSize.Height + THUMB_SIZE)));
      //thumbRectangle.Arrange(new Rect(new Point(-offset, -offset), new Size(Width = AdornedElement.RenderSize.Width + THUMB_SIZE, Height = AdornedElement.RenderSize.Height + THUMB_SIZE)));

      moveThumb.Width = finalSize.Width;
      moveThumb.Height = finalSize.Height;


      var r = new Rect(0, 0, finalSize.Width - offset, finalSize.Height - offset);
      moveThumb.Arrange(r);

      return finalSize;
    }

    protected override Visual GetVisualChild(int index)
    {
      return visualCollection[index];
    }

    protected override int VisualChildrenCount
    {
      get
      {
        return visualCollection.Count;
      }
    }


    public void OnChange()
    {
      //var element = AdornedElement as FrameworkElement;
      //var r = new Size(element.ActualWidth, element.ActualHeight);
      //ArrangeOverride(r);
    }
  }
}