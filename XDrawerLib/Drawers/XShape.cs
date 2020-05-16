using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using XDrawerLib.Helpers;

namespace XDrawerLib.Drawers
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
        internal FrameworkElement FollowItem;
        public DrawerStyle Style
        {
            get => _style;
            set
            {
                _style = value;

                if (OwnedShape != null)
                {
                    OwnedShape.Stroke = _style.Border;
                    OwnedShape.StrokeThickness = _style.BorderSize;
                    OwnedShape.Opacity = _style.Opacity;
                    OwnedShape.Fill = _style.Background;
                }

                if (OwnedControl != null)
                {
                    if (OwnedControl is RichTextBox txt)
                    {
                        txt.BorderBrush = _style.Border;
                        txt.BorderThickness = new Thickness(_style.BorderSize);
                        txt.Opacity = _style.Opacity;
                        txt.Background = _style.Background;
                    }
                }
            }
        }

        public XShape()
        {
            Id = Guid.NewGuid().ToString();

            Console.WriteLine(Id);

            AdornerHelper.RemoveAllAdorners();

            Selector.DeselectAll();
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
                if (sender is RichTextBox)
                {
                    e.Handled = false;
                }
                else
                {
                    if (Drawer.DrawTool != Tool.None && Drawer.DrawTool != Tool.MoveResize)
                    {
                        e.Handled = true;
                    }
                }
            }

            if (Drawer.DrawTool != Tool.None && Drawer.DrawTool != Tool.MoveResize) return;

            if (Drawer.IsEditMode) return;

            AdornerHelper.RemoveAllAdorners();
            Selector.DeselectAll();

            IsSelected = true;

            if (FollowItem != null)
            {
                AdornerHelper.AddAdorner(sender, this);
            }
            else
            {
                AdornerHelper.AddAdorner(sender);
            }

            if (OwnedShape != null)
            {
                Drawer.ActiveObject = OwnedShape;
            }

            if (OwnedControl != null)
            {
                if (OwnedControl is List<Border> borders)
                {
                    var c = (FrameworkElement)sender;

                    foreach (var b in borders)
                    {
                        if (b.Uid == c.Uid)
                        {
                            Drawer.ActiveObject = b;
                            break;
                        }
                    }
                }
                else
                {
                    Drawer.ActiveObject = (FrameworkElement)OwnedControl;
                }
            }
        }

        internal void OnErase(object sender, System.Windows.Input.StylusEventArgs e)
        {
            if (e.Inverted)
            {
                Console.WriteLine("removed");
                Selector.DeleteObject(OwnedShape);
            }
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

            Drawer.DrawTool = Drawer.ContinuousDraw ? Drawer.DrawTool : Tool.None;
            Drawer.IsObjectCreating = false;
            Drawer.IsDrawEnded = true;

            Style = Style;

            if (OwnedShape != null)
            {
                UndoHelper.AddStep(UndoHelper.ActionType.Create, OwnedShape);
            }

            if (OwnedControl != null)
            {
                UndoHelper.AddStep(UndoHelper.ActionType.Create, (FrameworkElement)OwnedControl);
            }

        }

        public void Cancel()
        {
            Drawer.IsObjectCreating = false;

            if (OwnedShape != null)
            {
                Selector.DeleteObject(OwnedShape);
            }

            if (OwnedControl != null)
            {
                Selector.DeleteObject((FrameworkElement)OwnedControl);
            }
        }

        public virtual void Edit()
        {

        }

        public virtual void EndEdit()
        {

        }

        public void SetTextStyle(DependencyProperty property, object value)
        {
            if (OwnedControl is RichTextBox txt)
            {
                var ts = txt.Selection;
                ts.ApplyPropertyValue(property, value);
                txt.Focus();
            }
        }

        public void SetPosition(Point position, Point position2 = new Point())
        {
            if (OwnedShape != null)
            {
                if (OwnedShape is Line line)
                {
                    line.X1 = position.X;
                    line.Y1 = position.Y;
                    line.X2 = position2.X;
                    line.Y2 = position2.Y;
                }
                else
                {
                    Canvas.SetLeft(OwnedShape, position.X);
                    Canvas.SetTop(OwnedShape, position.Y);
                }
            }

            if (OwnedControl != null)
            {
                Canvas.SetLeft((UIElement)OwnedControl, position.X);
                Canvas.SetTop((UIElement)OwnedControl, position.Y);
            }
        }
    }
}