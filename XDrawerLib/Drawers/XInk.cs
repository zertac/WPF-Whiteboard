using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;
using XDrawerLib.Helpers;

namespace XDrawerLib.Drawers
{
    public class XInk : XShape, IShape
    {
        public InkCanvas Drawing;
        public List<Stroke> Strokes;
        private int _currentIndex = 0;
        private bool IsHighlight;

        public void Create(Point e)
        {
            IsDrawing = true;
            StartPoint = e;

            Drawing = new InkCanvas();
            Drawing.Width = Drawer.Page.ActualWidth;
            Drawing.Height = Drawer.Page.ActualHeight;
            Drawing.Tag = this;
            Drawing.Background = new SolidColorBrush(Colors.Transparent);
            Strokes = new List<Stroke>();

            var drawingAttributes = new DrawingAttributes();
            if (Drawer.DrawTool == Tool.Ink)
            {
                drawingAttributes.Color = Colors.Black;
                drawingAttributes.IgnorePressure = false;
                drawingAttributes.FitToCurve = true;
                drawingAttributes.StylusTip = StylusTip.Ellipse;
                drawingAttributes.Width = 4;
            }
            else if (Drawer.DrawTool == Tool.Highlight)
            {
                drawingAttributes.Color = Color.FromArgb(127, 255, 255, 0);
                drawingAttributes.IgnorePressure = false;
                drawingAttributes.FitToCurve = true;
                drawingAttributes.StylusTip = StylusTip.Rectangle;
                drawingAttributes.Width = 4;
                drawingAttributes.Height = 14;
                IsHighlight = true;
                //drawingAttributes.IsHighlighter = true;
            }

            Drawing.DefaultDrawingAttributes = drawingAttributes;
            Drawing.StrokeCollected += Drawing_StrokeCollected;
            Drawing.StrokeErased += Drawing_StrokeErased;
            OwnedControl = new List<Border>();

            Style = new DrawerStyle();

            Drawer.Page.Children.Add(Drawing);
            Drawer.IsObjectCreating = true;
        }

        private void Drawing_StrokeErased(object sender, RoutedEventArgs e)
        {
            ArrangeStrokes();
        }

        private void ArrangeStrokes()
        {
            Strokes.Clear();
            foreach (var s in Drawing.Strokes)
            {
                Strokes.Add(s);
            }

            _currentIndex = Strokes.Count;
        }

        private void Drawing_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            Strokes.Add(e.Stroke);
            _currentIndex = Strokes.Count;
            Console.WriteLine("collected");
        }

        public void Update(Point e)
        {

        }

        public override void Finish()
        {
            if (!IsDrawing || Drawing == null) return;

            IsDrawing = false;

            Drawer.IsObjectCreating = false;
            Drawer.IsDrawEnded = true;
            Drawer.Page.Children.Remove(Drawing);
            Drawer.Objects.Remove(Id);

            var lst = Drawing.Strokes.ToList();

            foreach (var stroke in lst)
            {
                var geometry = stroke.GetGeometry(stroke.DrawingAttributes).GetOutlinedPathGeometry();

                var border = new Border();
                border.Background = new SolidColorBrush(Colors.Transparent);
                border.Width = stroke.GetBounds().Width;
                border.Height = stroke.GetBounds().Height;
                border.MouseLeftButtonDown += OnSelect;
                border.StylusDown += OnErase;
                border.Uid = Guid.NewGuid().ToString();

                var path = new Path();
                path.Data = geometry;
                path.VerticalAlignment = VerticalAlignment.Stretch;
                path.HorizontalAlignment = HorizontalAlignment.Stretch;
                if (!IsHighlight)
                {
                    path.Fill = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    path.Fill = new SolidColorBrush(Color.FromArgb(127, 255, 255, 0));
                }

                path.Stretch = Stretch.Fill;
                border.Tag = this;
                border.Child = path;
                Canvas.SetLeft(border, stroke.GetBounds().Left);
                Canvas.SetTop(border, stroke.GetBounds().Top);

                if (OwnedControl is List<Border> oLst)
                {
                    oLst.Add(border);
                }

                Drawer.Objects.Add(border.Uid, this);
                Drawer.Page.Children.Add(border);

                Drawer.UndoHelper.AddStep(UndoHelper.ActionType.Create, border);
            }
        }

        public void Undo()
        {
            if (_currentIndex == -1) return;
            if (Strokes.Count == 0) return;

            Drawing.Strokes.Clear();

            var lst = Strokes.GetRange(0, _currentIndex).ToList();
            foreach (var s in lst)
            {
                Drawing.Strokes.Add(s);
            }

            if (_currentIndex > 0) _currentIndex--;
        }

        public void Redo()
        {
            if (_currentIndex == -1) return;
            if (Strokes.Count == 0) return;

            if (_currentIndex < Strokes.Count)
            {
                _currentIndex++;
            }

            Drawing.Strokes.Clear();

            var lst = Strokes.GetRange(0, _currentIndex).ToList();
            foreach (var s in lst)
            {
                Drawing.Strokes.Add(s);
            }
        }

        public XInk(Drawer drawer) : base(drawer)
        {
            Drawer = drawer;
        }
    }
}
