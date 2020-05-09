using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XPDFDoc.Drawers;
using XPDFDoc.Helpers;
using Type = XPDFDoc.Drawers.Type;

namespace XPDFDoc
{
  public static class Drawer
  {
    #region PROPERTIES
    private static Type _drawType;

    public static Type DrawType
    {
      get { return _drawType; }
      set
      {
        _drawType = value;

        if (value != Type.Ink && value != Type.MoveResize && _drawType == Type.Ink)
        {
          Selector.FinishDraw();
        }

        if (value == Type.None)
        {
          Selector.EndEditForObject();
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

    public static object ActiveObject;

    public static string CustomShapeData;

    public static void Initialize(Canvas canvas)
    {
      Selector.Canvas = canvas;
      Objects = new Dictionary<string, XShape>();

      _page = canvas;
      _page.MouseLeftButtonDown += _canvas_PreviewMouseLeftButtonDown;
      _page.MouseMove += _canvas_PreviewMouseMove;
      _page.MouseLeftButtonUp += _canvas_PreviewMouseLeftButtonUp;
    }

    private static void _canvas_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      FinishDraw();
    }

    private static void _canvas_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (DrawType != Type.None && DrawType != Type.MoveResize)
      {
        UpdateDraw(e.GetPosition(Page));
      }
      else
      {
        if (DrawType == Type.None)
        {
          Selector.UpdateSelect(e.GetPosition(Page));
        }
      }
    }

    private static void _canvas_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      if (DrawType == Type.None)
      {
        Selector.StartSelect(e.GetPosition(Page));
        Selector.EndEditForObject();
      }
      else if (DrawType == Type.MoveResize)
      {
        Drawer.DrawType = Type.None;
        AdornerHelper.RemoveAllAdorners();
        Selector.EndEditForObject();
      }
      else
      {
        //if (IsEditMode == false)
        //{
        IsEditMode = false;
        if (DrawType != Type.Text)
        {
          Selector.EndEditForObject();
        }
        //}
        StartDraw(e.GetPosition(Page));
      }

      return;
      if (e.ClickCount == 1)
      {
        //if (Objects.Count(x => x.Value.IsSelected) > 0)
        //{
        //  AdornerHelper.RemoveAllAdorners();
        //}

        IsEditMode = false;
        Selector.EndEditForObject();
        StartDraw(e.GetPosition(Page));


      }
      else if (e.ClickCount == 2)
      {
        Drawer.DrawType = Type.None;
        AdornerHelper.RemoveAllAdorners();
        Selector.EndEditForObject();
      }
    }

    private static void StartDraw(Point e)
    {
      if (IsEditMode) return;

      //if (DrawType == Type.None)
      //{
      //  Selector.StartSelect(e);
      //}
      else if (DrawType == Type.Rectangle)
      {
        var o = new XRectangle();
        Objects.Add(o.Id, o);
        Objects.Last().Value.ToType<XRectangle>().Create(e);
      }
      else if (DrawType == Type.Ellipse)
      {
        var o = new XEllipse();
        Objects.Add(o.Id, o);
        Objects.Last().Value.ToType<XEllipse>().Create(e);
      }
      else if (DrawType == Type.Triangle)
      {
        if (IsDrawEnded)
        {
          var o = new XTriangle();
          Objects.Add(o.Id, o);
        }

        Objects.Last().Value.ToType<XTriangle>().AddPoint(e);
      }
      else if (DrawType == Type.Line)
      {
        var o = new XLine();
        Objects.Add(o.Id, o);
        Objects.Last().Value.ToType<XLine>().Create(e);
      }
      else if (DrawType == Type.Text)
      {
        var o = new XText();
        Objects.Add(o.Id, o);
        Objects.Last().Value.ToType<XText>().Create(e);
      }
      else if (DrawType == Type.Ink)
      {
        var o = new XInk();
        Objects.Add(o.Id, o);
        Objects.Last().Value.ToType<XInk>().Create(e);
      }
      else if (DrawType == Type.Arrow)
      {
        var o = new XArrow();
        Objects.Add(o.Id, o);
        Objects.Last().Value.ToType<XArrow>().Create(e);
      }
      else if (DrawType == Type.Custom)
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

      if (DrawType == Type.Rectangle)
      {
        Objects.Last().Value.ToType<XRectangle>().Update(e);
      }
      else if (DrawType == Type.Ellipse)
      {
        Objects.Last().Value.ToType<XEllipse>().Update(e);
      }
      else if (DrawType == Type.Triangle)
      {
        Objects.Last().Value.ToType<XTriangle>().Update(e);
      }
      else if (DrawType == Type.Line)
      {
        Objects.Last().Value.ToType<XLine>().Update(e);
      }
      else if (DrawType == Type.Text)
      {
        Objects.Last().Value.ToType<XText>().Update(e);
      }
      else if (DrawType == Type.Arrow)
      {
        Objects.Last().Value.ToType<XArrow>().Update(e);
      }
      else if (DrawType == Type.Custom)
      {
        Objects.Last().Value.ToType<XCustom>().Update(e);
      }
    }

    public static void FinishDraw()
    {
      //if (IsEditMode) return;

      if (DrawType == Type.None)
      {
        Selector.FinishSelect();
      }
      else if (DrawType == Type.Rectangle)
      {
        Objects.Last().Value.Finish();
      }
      else if (DrawType == Type.Ellipse)
      {
        Objects.Last().Value.Finish();
      }
      else if (DrawType == Type.Triangle)
      {
        //Objects.Last().Value.Finish();
      }
      else if (DrawType == Type.Line)
      {
        Objects.Last().Value.OwnedShape.Tag.ToType<XLine>().Finish();
      }
      else if (DrawType == Type.Text)
      {
        Objects.Last().Value.Edit();
        Objects.Last().Value.Finish();
      }
      else if (DrawType == Type.Arrow)
      {
        Objects.Last().Value.OwnedShape.Tag.ToType<XArrow>().Finish();
      }
      else if (DrawType == Type.Custom)
      {
        Objects.Last().Value.Finish();
      }

      //IsObjectCreating = false;
    }

    public static XShape GetSelectedObject()
    {
      var o = ActiveObject as XShape;
      return o;

    }
  }
}
