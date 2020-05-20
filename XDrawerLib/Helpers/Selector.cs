using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using XDrawerLib.Drawers;
using Point = System.Windows.Point;
using XText = XDrawerLib.Drawers.XText;

namespace XDrawerLib.Helpers
{
    public class Selector
    {
        public Canvas Canvas;
        public bool IsDrawing;
        public Drawer Drawer;

        private static Rectangle _rect;
        private static Point _startPoint;

        public Selector(Drawer drawer)
        {
            Drawer = drawer;
        }

        public void StartSelect(Point e)
        {
            if (Drawer.DrawTool == Tool.MoveResize) return;

            DeselectAll();

            _startPoint = e;
            IsDrawing = true;
            _rect = new Rectangle();
            _rect.Width = 0;
            _rect.Height = 0;
            _rect.Fill = StyleHelper.SelectionStyle.Background;
            _rect.Opacity = StyleHelper.SelectionStyle.Opacity;
            _rect.Stroke = StyleHelper.SelectionStyle.Border;
            _rect.Uid = "x_selector";

            Canvas.SetLeft(_rect, e.X);
            Canvas.SetTop(_rect, e.Y);
            Canvas.Children.Add(_rect);
        }

        public void UpdateSelect(Point e)
        {
            if (!IsDrawing) return;

            var diffX = e.X - _startPoint.X;
            var diffY = e.Y - _startPoint.Y;
            var scaleX = 1;
            var scaleY = 1;

            if (diffX < 0)
            {
                scaleX = -1;
            }

            if (diffY < 0)
            {
                scaleY = -1;
            }

            _rect.RenderTransform = new ScaleTransform(scaleX, scaleY);

            _rect.Width = Math.Abs(diffX);
            _rect.Height = Math.Abs(diffY);
        }

        public void FinishSelect()
        {
            IsDrawing = false;
            FindContainsObjects();
            Canvas.Children.Remove(_rect);

            _rect = null;
        }

        public void CancelSelect()
        {

        }

        public void EndDrawing()
        {
            foreach (var o in Drawer.Objects)
            {
                if (o.Value.IsDrawing)
                {
                    if (Drawer.DrawTool != Tool.Ink)
                        o.Value.Finish();
                }
            }
        }

        public void SelectAll()
        {
            foreach (var o in Drawer.Objects.Values)
            {
                Identify(o);
            }
        }

        public void DeleteSelected()
        {

            var objList = Drawer.Objects.Where(x => x.Value.IsSelected).ToList();

            foreach (KeyValuePair<string, XShape> o in objList)
            {
                if (o.Value.OwnedShape != null)
                {
                    Drawer.Page.Children.Remove(o.Value.OwnedShape);
                    Drawer.UndoHelper.AddStep(UndoHelper.ActionType.Delete, o.Value.OwnedShape);
                }

                if (o.Value.OwnedControl != null)
                {
                    if (o.Value.OwnedControl is List<Border> borders)
                    {
                        foreach (var b in borders)
                        {
                            if (Drawer.ActiveObject.Uid == b.Uid)
                            {
                                Drawer.Page.Children.Remove(b);
                                Drawer.Objects.Remove(b.Uid);
                                Drawer.UndoHelper.AddStep(UndoHelper.ActionType.Delete, b);
                                break;
                            }
                        }
                    }
                    else
                    {
                        Drawer.Page.Children.Remove((UIElement)o.Value.OwnedControl);
                        Drawer.Objects.Remove(o.Value.Id);
                        Drawer.UndoHelper.AddStep(UndoHelper.ActionType.Delete, (FrameworkElement)o.Value.OwnedControl);
                    }
                }

                if (o.Value.FollowItem != null)
                {
                    Drawer.Page.Children.Remove(o.Value.FollowItem);
                }

                ArrangeObjects();
            }
        }

        public void ArrangeObjects()
        {
            Drawer.Objects = new Dictionary<string, XShape>();
            foreach (FrameworkElement c in Drawer.Page.Children)
            {
                var obj = c.Tag;
                if (c.Tag != null)
                {
                    if (c.Tag.ToType<XShape>().OwnedControl is List<Border> borders)
                    {
                        foreach (var b in borders)
                        {
                            if (b.Uid == c.Uid)
                            {
                                Drawer.Objects.Add(b.Uid, c.Tag.ToType<XShape>());
                                break;
                            }
                        }
                    }
                    else
                    {
                        Drawer.Objects.Add(c.Tag.ToType<XShape>().Id, c.Tag.ToType<XShape>());
                    }
                }
            }
        }

        public void DeleteObject(FrameworkElement ctrl)
        {
            var o = ctrl.Tag.ToType<XShape>();

            if (o.OwnedShape != null)
            {
                var a = (UIElement)ctrl;
                Drawer.Page.Children.Remove(a);
            }

            if (o.OwnedControl != null)
            {
                if (o.OwnedControl is List<Border> borders)
                {
                    foreach (var b in borders)
                    {
                        if (ctrl.Uid == b.Uid)
                        {
                            Drawer.Page.Children.Remove(b);
                            Drawer.Objects.Remove(b.Uid);
                            break;
                        }
                    }
                }
                else
                {
                    Drawer.Page.Children.Remove((UIElement)o.OwnedControl);
                    Drawer.Objects.Remove(o.Id);
                }
            }

            if (o.FollowItem != null)
            {
                Drawer.Page.Children.Remove(o.FollowItem);
            }

            ArrangeObjects();
        }

        public void DeselectAll()
        {
            foreach (var item in Drawer.Objects.Values)
            {
                if (item.OwnedShape != null || item.OwnedControl != null)
                {
                    item.IsSelected = false;
                }
            }
        }

        public void DeleteAll()
        {
            foreach (var item in Drawer.Objects.Values)
            {
                if (item.OwnedShape != null)
                {
                    DeleteObject(item.OwnedShape);
                }

                if (item.OwnedControl != null)
                {
                    DeleteObject((FrameworkElement)item.OwnedControl);
                }
            }
        }

        public void EndEditForObject()
        {
            foreach (var item in Drawer.Objects.Values)
            {
                if (item.OwnedControl != null && item.OwnedControl is RichTextBox txt)
                {
                    txt.Tag.ToType<XText>().EndEdit();
                }
            }
        }

        public void FinishDraw()
        {
            var lst = Drawer.Objects.ToList();

            foreach (KeyValuePair<string, XShape> item in lst)
            {
                if (item.Value.OwnedControl != null)
                {
                    if (item.Value.OwnedControl is List<Border> b)
                    {
                        (item.Value as XInk)?.Finish();
                    }
                }
            }
        }

        private void FindContainsObjects()
        {
            DeselectAll();

            foreach (var item in Drawer.Objects.Values)
            {
                if (item.OwnedShape != null)
                {
                    var s = IsContains(item.OwnedShape);
                    if (s)
                    {
                        Identify(item);
                    }
                }
                else if (item.OwnedControl != null)
                {
                    if (item.OwnedControl is List<Border> inks)
                    {
                        foreach (var ink in inks)
                        {
                            var s = IsContains(ink);
                            if (s)
                            {
                                Identify(item);
                            }
                        }
                    }
                    else
                    {
                        var s = IsContains((FrameworkElement)item.OwnedControl);
                        if (s)
                        {
                            Identify(item);
                        }
                    }
                }
            }
        }

        private void Identify(XShape item)
        {
            item.ToType<XShape>().IsSelected = true;
        }

        private bool IsContains(FrameworkElement item)
        {
            if (item == null) return false;
            if (_rect == null) return false;

            if (item.Tag is XLine line)
            {
                var xScale = (double)_rect.RenderTransform.GetValue(ScaleTransform.ScaleXProperty);
                var yScale = (double)_rect.RenderTransform.GetValue(ScaleTransform.ScaleYProperty);
                
                var x1 = Canvas.GetLeft(_rect);
                var y1 = Canvas.GetTop(_rect);

                if (xScale < 0)
                {
                    x1 -= _rect.ActualWidth;
                }

                if (yScale < 0)
                {
                    y1 -= _rect.ActualHeight;
                }

                var r1 = new Rect(x1, y1, _rect.ActualWidth, _rect.ActualHeight);

                var s = LineIntersectsRect(new Point(line.Drawing.X1, line.Drawing.Y1), new Point(line.Drawing.X2, line.Drawing.Y2), r1);
                if (s)
                {
                    return true;
                }

                return false;
            }
            else
            {
                var xScale = (double)_rect.RenderTransform.GetValue(ScaleTransform.ScaleXProperty);
                var yScale = (double)_rect.RenderTransform.GetValue(ScaleTransform.ScaleYProperty);

                var x1 = Canvas.GetLeft(_rect);
                var y1 = Canvas.GetTop(_rect);

                if (xScale < 0)
                {
                    x1 -= _rect.ActualWidth;
                }

                if (yScale < 0)
                {
                    y1 -= _rect.ActualHeight;
                }

                var r1 = new Rect(x1, y1, _rect.ActualWidth, _rect.ActualHeight);

                var x2 = Canvas.GetLeft(item);
                var y2 = Canvas.GetTop(item);
                var r2 = new Rect(x2, y2, item.RenderSize.Width, item.RenderSize.Height);

                if (r1.IntersectsWith(r2) && r1.Width > 0 && r1.Height > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool LineIntersectsRect(Point p1, Point p2, Rect r)
        {
            return LineIntersectsLine(p1, p2, new Point(r.X, r.Y), new Point(r.X + r.Width, r.Y)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y), new Point(r.X + r.Width, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y + r.Height), new Point(r.X, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X, r.Y + r.Height), new Point(r.X, r.Y)) ||
                   (r.Contains(p1) && r.Contains(p2));
        }

        private bool LineIntersectsLine(Point l1p1, Point l1p2, Point l2p1, Point l2p2)
        {
            double q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            double d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

            if (d == 0)
            {
                return false;
            }

            double r = q / d;

            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            double s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }
    }
}
