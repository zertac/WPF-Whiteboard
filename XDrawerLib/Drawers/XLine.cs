using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using XDrawerLib.Helpers;

namespace XDrawerLib.Drawers
{
    public class XLine : XShape, IShape
    {
        public Line Drawing;

        public void Create(Point e)
        {
            IsDrawing = true;
            StartPoint = e;

            Drawing = new Line();
            Drawing.X1 = e.X;
            Drawing.Y1 = e.Y;
            Drawing.X2 = e.X;
            Drawing.Y2 = e.Y;
            Drawing.Stroke = new SolidColorBrush(Colors.Black);
            Drawing.StrokeThickness = 2;
            Drawing.Opacity = 0.2;
            Drawing.Tag = this;
            OwnedShape = Drawing;

            Style = new DrawerStyle(StyleHelper.CurrentStyle);
            Drawing.MouseLeftButtonDown += OnSelect;
            Drawing.StylusDown += OnErase;
            Drawer.Page.Children.Add(Drawing);
            Drawer.IsObjectCreating = true;
        }

        public void Update(Point e)
        {
            if (!IsDrawing) return;

            Drawing.X2 = e.X;
            Drawing.Y2 = e.Y;
        }

        public new void Finish()
        {
            base.Finish();

            if (Drawing.X1 == Drawing.X2 && Drawing.Y1 == Drawing.Y2)
            {
                base.Cancel();
            }
        }
    }
}
