using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using XDrawerLib.Drawers;

namespace XDrawerLib.Helpers
{
    public class Drawer
    {
        public Selector Selector;
        public HotKeyHelper HotKeyHelper;
        public UndoHelper UndoHelper;
        public AdornerHelper AdornerHelper;

        #region PROPERTIES
        private Tool _drawTool;

        public Tool DrawTool
        {
            get { return _drawTool; }
            set
            {
                //if (_drawTool == Tool.Ink && value != Tool.Ink)
                //{
                //    Selector.FinishDraw();
                //}

                //if (_drawTool == Tool.Highlight && value != Tool.Highlight)
                //{
                //    Selector.FinishDraw();
                //}

                _drawTool = value;

                if ((value != Tool.Ink && value != Tool.Highlight && value != Tool.MoveResize) || _drawTool == Tool.Ink || _drawTool == Tool.Highlight)
                {
                    Selector.FinishDraw();
                }

                if (value == Tool.Selection || value == Tool.None)
                {
                    Selector.EndEditForObject();
                }

                if (value == Tool.Ink || value == Tool.Highlight)
                {
                    var o = new XInk(this);
                    Objects.Add(o.Id, o);
                    Objects.Last().Value.ToType<XInk>().Create(new Point());
                }
            }
        }

        private Canvas _page;

        public Canvas Page
        {
            get { return _page; }
        }
        #endregion

        public bool ContinuousDraw;
        public Dictionary<string, XShape> Objects;
        public bool IsEditMode;

        public bool IsObjectCreating;
        public bool IsDrawEnded = true;

        public FrameworkElement ActiveObject;

        public string CustomShapeData;

        public Drawer()
        {

        }

        public Drawer Instance;

        public Drawer(Canvas canvas)
        {
            Instance = this;

            Selector = new Selector(this);
            HotKeyHelper = new HotKeyHelper(this);
            UndoHelper = new UndoHelper(this);
            AdornerHelper = new AdornerHelper(this);

            Initialize(canvas);
        }

        private void Initialize(Canvas canvas)
        {
            Selector.Canvas = canvas;
            Objects = new Dictionary<string, XShape>();

            _page = canvas;
            _page.PreviewMouseLeftButtonDown += _canvas_PreviewMouseLeftButtonDown;
            _page.MouseMove += _canvas_PreviewMouseMove;
            _page.PreviewMouseLeftButtonUp += _canvas_PreviewMouseLeftButtonUp;
            var window = Application.Current.MainWindow;
            if (window != null)
            {
                window.KeyDown += P_PreviewKeyDown;
            }
        }


        private void P_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HotKeyHelper.ExecuteShortcut();
        }

        private void _canvas_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FinishDraw();
        }

        private void _canvas_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (DrawTool == Tool.None) return;

            if (DrawTool != Tool.Selection && DrawTool != Tool.MoveResize)
            {
                UpdateDraw(e.GetPosition(Page));
            }
            else
            {
                if (DrawTool == Tool.Selection)
                {
                    Selector.UpdateSelect(e.GetPosition(Page));
                }
            }
        }

        private void _canvas_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ActiveObject = null;

            if (DrawTool == Tool.None) return;

            if (e.StylusDevice != null && e.StylusDevice.Inverted)
            {
                return;
            }

            if (DrawTool == Tool.Selection)
            {
                Selector.StartSelect(e.GetPosition(Page));
                Selector.EndEditForObject();
            }
            else if (DrawTool == Tool.MoveResize)
            {
                DrawTool = Tool.Selection;
                AdornerHelper.RemoveAllAdorners();
                Selector.EndEditForObject();
            }
            else
            {
                IsEditMode = false;
                if (DrawTool != Tool.Text)
                {
                    Selector.EndEditForObject();
                }

                StartDraw(e.GetPosition(Page));
            }
        }

        private void StartDraw(Point e)
        {
            if (IsEditMode) return;

            Selector.FinishSelect();

            if (DrawTool != Tool.Triangle)
            {
                Selector.EndDrawing();
            }

            if (DrawTool == Tool.Rectangle)
            {
                var o = new XRectangle(this);
                Objects.Add(o.Id, o);
                Objects.Last().Value.ToType<XRectangle>().Create(e);
            }
            else if (DrawTool == Tool.Ellipse)
            {
                var o = new XEllipse(this);
                Objects.Add(o.Id, o);
                Objects.Last().Value.ToType<XEllipse>().Create(e);
            }
            else if (DrawTool == Tool.Triangle)
            {
                if (IsDrawEnded)
                {
                    var o = new XTriangle(this);
                    Objects.Add(o.Id, o);
                }

                Objects.Last().Value.ToType<XTriangle>().AddPoint(e);
            }
            else if (DrawTool == Tool.Line)
            {
                var o = new XLine(this);
                Objects.Add(o.Id, o);
                Objects.Last().Value.ToType<XLine>().Create(e);
            }
            else if (DrawTool == Tool.Text)
            {
                var o = new XText(this);
                Objects.Add(o.Id, o);
                Objects.Last().Value.ToType<XText>().Create(e);
            }
            else if (DrawTool == Tool.Ink)
            {
                //var o = new XInk();
                //Objects.Add(o.Id, o);
                //Objects.Last().Value.ToType<XInk>().Create(e);
            }
            else if (DrawTool == Tool.Arrow)
            {
                var o = new XArrow(this);
                Objects.Add(o.Id, o);
                Objects.Last().Value.ToType<XArrow>().Create(e);
            }
            else if (DrawTool == Tool.Custom)
            {
                var o = new XCustom(this);
                Objects.Add(o.Id, o);
                Objects.Last().Value.ToType<XCustom>().Create(e, CustomShapeData);
            }
        }

        public void UpdateDraw(Point e)
        {
            if (IsEditMode) return;
            if (!IsObjectCreating) return;

            if (DrawTool == Tool.Rectangle)
            {
                Objects.Last().Value.ToType<XRectangle>().Update(e);
            }
            else if (DrawTool == Tool.Ellipse)
            {
                Objects.Last().Value.ToType<XEllipse>().Update(e);
            }
            else if (DrawTool == Tool.Triangle)
            {
                Objects.Last().Value.ToType<XTriangle>().Update(e);
            }
            else if (DrawTool == Tool.Line)
            {
                Objects.Last().Value.ToType<XLine>().Update(e);
            }
            else if (DrawTool == Tool.Text)
            {
                Objects.Last().Value.ToType<XText>().Update(e);
            }
            else if (DrawTool == Tool.Arrow)
            {
                Objects.Last().Value.ToType<XArrow>().Update(e);
            }
            else if (DrawTool == Tool.Custom)
            {
                Objects.Last().Value.ToType<XCustom>().Update(e);
            }
        }

        public void FinishDraw()
        {
            //if (IsEditMode) return;
            if (!IsObjectCreating && Selector.IsDrawing == false) return;

            if (DrawTool == Tool.Selection)
            {
                Selector.FinishSelect();
            }
            else if (DrawTool == Tool.Rectangle)
            {
                Objects.Last().Value.Finish();
            }
            else if (DrawTool == Tool.Ellipse)
            {
                Objects.Last().Value.Finish();
            }
            else if (DrawTool == Tool.Triangle)
            {

            }
            else if (DrawTool == Tool.Line)
            {
                Objects.Last().Value.OwnedShape.Tag.ToType<XLine>().Finish();
            }
            else if (DrawTool == Tool.Text)
            {
                if (!(Objects.Last().Value is XText txt)) return;

                if (IsEditMode == false)
                {
                    txt.Edit();
                    txt.Finish();
                }
                else
                {
                    DrawTool = Tool.Selection;
                }
            }
            else if (DrawTool == Tool.Arrow)
            {
                Objects.Last().Value.OwnedShape.Tag.ToType<XArrow>().Finish();
            }
            else if (DrawTool == Tool.Custom)
            {
                Objects.Last().Value.Finish();
            }
        }

        public FrameworkElement GetSelectedObject()
        {
            var o = ActiveObject.Tag.ToType<XShape>().OwnedShape == null ? (FrameworkElement)ActiveObject.Tag.ToType<XShape>().OwnedControl : ActiveObject.Tag.ToType<XShape>().OwnedShape;
            return o;
        }

        public XShape GetSelectedShape()
        {
            return ActiveObject.Tag.ToType<XShape>();
        }

        public int GetShapeCount()
        {
            return Objects.Count;
        }

        public void CancelDrawing()
        {
            if (Objects.Count > 0)
            {
                Objects.Last().Value.Cancel();
            }

            if (Selector.IsDrawing)
            {
                List<FrameworkElement> lst = new List<FrameworkElement>();
                foreach (FrameworkElement c in Page.Children)
                {
                    if (c.Uid == "x_selector")
                    {
                        lst.Add(c);
                    }
                }

                for (int i = 0; i < lst.Count; i++)
                {
                    Page.Children.Remove(lst[i]);
                }
            }
        }

        public void CleanPage()
        {
            Selector.FinishDraw();
            Selector.DeleteAll();
            DrawTool = _drawTool;
        }

        public void SetBackground(Brush brush)
        {
            foreach (var o in Objects)
            {
                if (o.Value.IsSelected)
                {
                    var style = new DrawerStyle();
                    style.Background = brush;
                    style.BorderSize = o.Value.Style.BorderSize;
                    style.Border = o.Value.Style.Border;
                    style.FontSize = o.Value.Style.FontSize;
                    style.Opacity = o.Value.Style.Opacity;

                    o.Value.Style = style;
                }
            }

            if (ActiveObject != null)
            {
                var style = new DrawerStyle();
                style.Background = brush;
                style.BorderSize = ActiveObject.Tag.ToType<XShape>().Style.BorderSize;
                style.Border = ActiveObject.Tag.ToType<XShape>().Style.Border;
                style.FontSize = ActiveObject.Tag.ToType<XShape>().Style.FontSize;
                style.Opacity = ActiveObject.Tag.ToType<XShape>().Style.Opacity;

                ActiveObject.Tag.ToType<XShape>().Style = style;
            }
        }

        public void SetBorder(Brush brush)
        {
            foreach (var o in Objects)
            {
                if (o.Value.IsSelected)
                {
                    var style = new DrawerStyle();
                    style.Background = o.Value.Style.Background;
                    style.BorderSize = o.Value.Style.BorderSize;
                    style.Border = brush;
                    style.FontSize = o.Value.Style.FontSize;
                    style.Opacity = o.Value.Style.Opacity;

                    o.Value.Style = style;
                }
            }

            if (ActiveObject != null)
            {
                var style = new DrawerStyle();
                style.Background = ActiveObject.Tag.ToType<XShape>().Style.Background;
                style.BorderSize = ActiveObject.Tag.ToType<XShape>().Style.BorderSize;
                style.Border = brush;
                style.FontSize = ActiveObject.Tag.ToType<XShape>().Style.FontSize;
                style.Opacity = ActiveObject.Tag.ToType<XShape>().Style.Opacity;

                ActiveObject.Tag.ToType<XShape>().Style = style;
            }
        }

        public void SetBorderSize(double value)
        {
            foreach (var o in Objects)
            {
                if (o.Value.IsSelected)
                {
                    var status = true;
                    if (value < 0)
                    {
                        if (o.Value.Style.BorderSize < 1)
                        {
                            status = false;
                        }
                    }
                    else if (value > 0)
                    {
                        if (o.Value.Style.BorderSize > 20)
                        {
                            status = false;
                        }
                    }

                    if (status)
                    {
                        var style = new DrawerStyle();
                        style.Background = o.Value.Style.Background;
                        style.BorderSize = o.Value.Style.BorderSize + value;
                        style.Border = o.Value.Style.Border;
                        style.FontSize = o.Value.Style.FontSize;
                        style.Opacity = o.Value.Style.Opacity;

                        o.Value.Style = style;
                    }
                }
            }

            if (ActiveObject != null)
            {
                var status = true;
                if (value < 0)
                {
                    if (ActiveObject.Tag.ToType<XShape>().Style.BorderSize < 1)
                    {
                        status = false;
                    }
                }
                else if (value > 0)
                {
                    if (ActiveObject.Tag.ToType<XShape>().Style.BorderSize > 20)
                    {
                        status = false;
                    }
                }

                if (status)
                {
                    var style = new DrawerStyle();
                    style.Background = ActiveObject.Tag.ToType<XShape>().Style.Background;
                    style.BorderSize = ActiveObject.Tag.ToType<XShape>().Style.BorderSize + value;
                    style.Border = ActiveObject.Tag.ToType<XShape>().Style.Border;
                    style.FontSize = ActiveObject.Tag.ToType<XShape>().Style.FontSize;
                    style.Opacity = ActiveObject.Tag.ToType<XShape>().Style.Opacity;

                    ActiveObject.Tag.ToType<XShape>().Style = style;
                }
            }
        }

        public void SetFontSize(double value)
        {
            foreach (var o in Objects)
            {
                if (o.Value.IsSelected)
                {
                    var status = true;
                    if (value < 0)
                    {
                        if (o.Value.Style.FontSize < 1)
                        {
                            status = false;
                        }
                    }
                    else if (value > 0)
                    {
                        if (o.Value.Style.FontSize > 60)
                        {
                            status = false;
                        }
                    }

                    if (o.Value.Style.FontSize + value < 1)
                    {
                        status = false;
                    }

                    if (status)
                    {
                        var style = new DrawerStyle();
                        style.Background = o.Value.Style.Background;
                        style.BorderSize = o.Value.Style.BorderSize;
                        style.Border = o.Value.Style.Border;
                        style.FontSize = o.Value.Style.FontSize + value;
                        style.Opacity = o.Value.Style.Opacity;

                        o.Value.Style = style;
                    }
                }
            }

            if (ActiveObject != null)
            {
                var status = true;
                if (value < 0)
                {
                    if (ActiveObject.Tag.ToType<XShape>().Style.FontSize < 1)
                    {
                        status = false;
                    }
                }
                else if (value > 0)
                {
                    if (ActiveObject.Tag.ToType<XShape>().Style.FontSize > 60)
                    {
                        status = false;
                    }
                }

                if (ActiveObject.Tag.ToType<XShape>().Style.FontSize + value < 1)
                {
                    status = false;
                }

                if (status)
                {
                    var style = new DrawerStyle();
                    style.Background = ActiveObject.Tag.ToType<XShape>().Style.Background;
                    style.BorderSize = ActiveObject.Tag.ToType<XShape>().Style.BorderSize;
                    style.Border = ActiveObject.Tag.ToType<XShape>().Style.Border;
                    style.FontSize = ActiveObject.Tag.ToType<XShape>().Style.FontSize + value;
                    style.Opacity = ActiveObject.Tag.ToType<XShape>().Style.Opacity;

                    ActiveObject.Tag.ToType<XShape>().Style = style;
                }
            }
        }

        public void SetOpacity(double value)
        {
            foreach (var o in Objects)
            {
                if (o.Value.IsSelected)
                {
                    var status = true;
                    if (value < 0)
                    {
                        if (o.Value.Style.Opacity < 1)
                        {
                            status = false;
                        }
                    }
                    else if (value > 0)
                    {
                        if (o.Value.Style.Opacity > 1)
                        {
                            status = false;
                        }
                    }

                    if (status)
                    {
                        var style = new DrawerStyle();
                        style.Background = o.Value.Style.Background;
                        style.BorderSize = o.Value.Style.BorderSize;
                        style.Border = o.Value.Style.Border;
                        style.FontSize = o.Value.Style.FontSize;
                        style.Opacity = value;

                        o.Value.Style = style;
                    }
                }
            }

            if (ActiveObject != null)
            {

                var status = true;
                if (value < 0)
                {
                    if (ActiveObject.Tag.ToType<XShape>().Style.Opacity < 1)
                    {
                        status = false;
                    }
                }
                else if (value > 0)
                {
                    if (ActiveObject.Tag.ToType<XShape>().Style.Opacity > 1)
                    {
                        status = false;
                    }
                }

                if (status)
                {
                    var style = new DrawerStyle();
                    style.Background = ActiveObject.Tag.ToType<XShape>().Style.Background;
                    style.BorderSize = ActiveObject.Tag.ToType<XShape>().Style.BorderSize;
                    style.Border = ActiveObject.Tag.ToType<XShape>().Style.Border;
                    style.FontSize = ActiveObject.Tag.ToType<XShape>().Style.FontSize;
                    style.Opacity = value;

                    ActiveObject.Tag.ToType<XShape>().Style = style;
                }
            }
        }
    }
}
