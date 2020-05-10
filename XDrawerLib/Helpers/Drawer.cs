using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XDrawerLib.Drawers;

namespace XDrawerLib.Helpers
{
  public static class Drawer
  {
    #region PROPERTIES
    private static Tool _drawTool;

    public static Tool DrawTool
    {
      get { return _drawTool; }
      set
      {
        _drawTool = value;

        if ((value != Tool.Ink && value != Tool.MoveResize) || _drawTool == Tool.Ink)
        {
          Selector.FinishDraw();
        }

        if (value == Tool.None)
        {
          Selector.EndEditForObject();
        }

        if (value == Tool.Ink)
        {
          var o = new XInk();
          Objects.Add(o.Id, o);
          Objects.Last().Value.ToType<XInk>().Create(new Point());
        }
      }
    }

    private static Canvas _page;

    public static Canvas Page
    {
      get { return _page; }
    }
    #endregion

    public static bool ContinuousDraw;
    public static Dictionary<string, XShape> Objects;
    public static bool IsEditMode;

    public static bool IsObjectCreating;
    public static bool IsDrawEnded = true;

    public static FrameworkElement ActiveObject;

    public static string CustomShapeData;

    public static void Initialize(Canvas canvas)
    {
      Selector.Canvas = canvas;
      Objects = new Dictionary<string, XShape>();

      _page = canvas;
      _page.PreviewMouseLeftButtonDown += _canvas_PreviewMouseLeftButtonDown;
      _page.MouseMove += _canvas_PreviewMouseMove;
      _page.PreviewMouseLeftButtonUp += _canvas_PreviewMouseLeftButtonUp;

      if ((_page.Parent as Grid)?.Parent is Window p) p.PreviewKeyDown += P_PreviewKeyDown;
    }

    private static void P_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      HotKeyHelper.ExecuteShortcut();
    }

    private static void _canvas_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      FinishDraw();
    }

    private static void _canvas_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (DrawTool != Tool.None && DrawTool != Tool.MoveResize)
      {
        UpdateDraw(e.GetPosition(Page));
      }
      else
      {
        if (DrawTool == Tool.None)
        {
          Selector.UpdateSelect(e.GetPosition(Page));
        }
      }
    }

    private static void _canvas_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      if (DrawTool == Tool.None)
      {
        Selector.StartSelect(e.GetPosition(Page));
        Selector.EndEditForObject();
      }
      else if (DrawTool == Tool.MoveResize)
      {
        Drawer.DrawTool = Tool.None;
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

    private static void StartDraw(Point e)
    {
      if (IsEditMode) return;

      if (DrawTool == Tool.Rectangle)
      {
        var o = new XRectangle();
        Objects.Add(o.Id, o);
        Objects.Last().Value.ToType<XRectangle>().Create(e);
      }
      else if (DrawTool == Tool.Ellipse)
      {
        var o = new XEllipse();
        Objects.Add(o.Id, o);
        Objects.Last().Value.ToType<XEllipse>().Create(e);
      }
      else if (DrawTool == Tool.Triangle)
      {
        if (IsDrawEnded)
        {
          var o = new XTriangle();
          Objects.Add(o.Id, o);
        }

        Objects.Last().Value.ToType<XTriangle>().AddPoint(e);
      }
      else if (DrawTool == Tool.Line)
      {
        var o = new XLine();
        Objects.Add(o.Id, o);
        Objects.Last().Value.ToType<XLine>().Create(e);
      }
      else if (DrawTool == Tool.Text)
      {
        var o = new XText();
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
        var o = new XArrow();
        Objects.Add(o.Id, o);
        Objects.Last().Value.ToType<XArrow>().Create(e);
      }
      else if (DrawTool == Tool.Custom)
      {
        var o = new XCustom();
        Objects.Add(o.Id, o);
        Objects.Last().Value.ToType<XCustom>().Create(e, CustomShapeData);
      }
    }

    public static void UpdateDraw(Point e)
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

    public static void FinishDraw()
    {
      //if (IsEditMode) return;

      if (DrawTool == Tool.None)
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
        Objects.Last().Value.Edit();
        Objects.Last().Value.Finish();
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

    public static FrameworkElement GetSelectedObject()
    {
      var o = ActiveObject.Tag.ToType<XShape>().OwnedShape == null ? ActiveObject.Tag.ToType<XShape>().OwnedShape : (FrameworkElement)ActiveObject.Tag.ToType<XShape>().OwnedControl;
      return o;
    }
  }
}
