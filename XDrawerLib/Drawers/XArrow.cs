using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using XDrawerLib.Helpers;

namespace XDrawerLib.Drawers
{
    public class XArrow : XShape, IShape
    {
        public Line Drawing;
        public Border Arrow;

        public void Create(Point e)
        {
            IsDrawing = true;
            StartPoint = e;


            Drawing = new Line();
            OwnedShape = Drawing;

            Style = new DrawerStyle(StyleHelper.CurrentStyle);

            Drawing.X1 = e.X;
            Drawing.Y1 = e.Y;
            Drawing.X2 = e.X;
            Drawing.Y2 = e.Y;
            Drawing.Opacity = 0.2;
            Drawing.Tag = this;

            Drawing.PreviewMouseLeftButtonDown += base.OnSelect;

            Arrow = new Border();
            Arrow.Background = Style.Background;
            Arrow.Width = 32;
            Arrow.Height = 32;
            Arrow.Opacity = 0.2;
            Arrow.RenderTransformOrigin = new Point(0.5, 0.5);

            var draw = new Path();
            draw.Data = Geometry.Parse("M4 2L4 22L21.3125 12Z");
            draw.VerticalAlignment = VerticalAlignment.Stretch;
            draw.HorizontalAlignment = HorizontalAlignment.Stretch;
            draw.Stretch = Stretch.Fill;
            draw.Fill = new SolidColorBrush(Colors.Black);
            
            Canvas.SetLeft(Arrow, e.X);
            Canvas.SetTop(Arrow, e.Y);
            
            Arrow.Child = draw;
            Arrow.PreviewMouseLeftButtonDown += Arrow_MouseLeftButtonDown;
            Drawing.PreviewStylusMove += OnErase;
            Arrow.PreviewStylusMove += OnErase;

            Drawer.Page.Children.Add(Arrow);

            Drawer.Page.Children.Add(Drawing);
            Drawer.IsObjectCreating = true;
            FollowItem = Arrow;
        }

        private void Arrow_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnSelect(Drawing, e);
        }

        public void Update(Point e)
        {
            if (!IsDrawing) return;

            Drawing.X2 = e.X;
            Drawing.Y2 = e.Y;

            SetArrowPosition();
        }

        public new void Finish()
        {
            base.Finish();

            SetArrowPosition();

            Arrow.Opacity = 1;

            if (Drawing.X1 == Drawing.X2 && Drawing.Y1 == Drawing.Y2)
            {
                base.Cancel();
            }
        }

        public void SetArrowPosition()
        {
            var a = Angle(Drawing.X1, Drawing.Y1, Drawing.X2, Drawing.Y2);

            var x = Drawing.X2 - 16;
            var y = Drawing.Y2 - 16;

            var rt = new RotateTransform();
            rt.Angle = -(a);
            rt.CenterX = 0.5;
            rt.CenterY = 0.5;
            Arrow.RenderTransform = rt;

            Canvas.SetTop(Arrow, y);
            Canvas.SetLeft(Arrow, x);
        }

        private double Angle(double cx, double cy, double ex, double ey)
        {
            var dy = ey - cy;
            var dx = ex - cx;
            var theta = Math.Atan2(dy, dx); // range (-PI, PI]
            theta *= 360 / (2 * Math.PI); // rads to degs, range (-180, 180]

            if (cy < ey)
            {
                theta -= 360;
            }

            return Math.Abs(theta);
        }
    }
}
