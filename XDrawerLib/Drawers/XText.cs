﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using XDrawerLib.Helpers;

namespace XDrawerLib.Drawers
{
  public class XText : XShape, IShape
  {
    public Rectangle Shadow;
    public RichTextBox Textbox;

    public void Create(Point e)
    {
      IsCustom = true;
      IsDrawing = true;

      StartPoint = e;

      Shadow = new Rectangle();
      Shadow.Fill = new SolidColorBrush(Colors.Transparent);
      Shadow.Stroke = new SolidColorBrush(Colors.Black);
      Shadow.StrokeThickness = 3;
      Shadow.Width = 0;
      Shadow.Height = 0;
      Shadow.Opacity = 0.2;

      Canvas.SetLeft(Shadow, e.X);
      Canvas.SetTop(Shadow, e.Y);

      Style = new DrawerStyle(StyleHelper.CurrentStyle);
      Drawer.Page.Children.Add(Shadow);
      Drawer.IsObjectCreating = true;
    }

    public void Update(Point e)
    {
      if (!IsDrawing) return;

      Shadow.Width = Math.Abs(StartPoint.X - e.X);
      Shadow.Height = Math.Abs(StartPoint.Y - e.Y);
    }

    public override void Edit()
    {
      IsDrawing = false;
      Drawer.IsEditMode = true;

      //var myFlowDoc = new FlowDocument();
      //myFlowDoc.Blocks.Add(new Paragraph(new Run("Paragraph 1")));

      Textbox = new RichTextBox
      {
        Width = Shadow.Width,
        Height = Shadow.Height,
        Background = StyleHelper.CurrentStyle.Background,
        BorderBrush = StyleHelper.CurrentStyle.Border,
        BorderThickness = new Thickness(StyleHelper.CurrentStyle.BorderSize),
        IsReadOnly = false
      };

      Textbox.Document.GetFormattedText();

      Textbox.Uid = Guid.NewGuid().ToString();

      Canvas.SetLeft(Textbox, Canvas.GetLeft(Shadow));
      Canvas.SetTop(Textbox, Canvas.GetTop(Shadow));

      Drawer.Page.Children.Add(Textbox);
      Drawer.Page.Children.Remove(Shadow);

      Textbox.Tag = this;
      Textbox.Focus();

      Textbox.TextChanged += delegate (object sender, TextChangedEventArgs args)
       {
         var newWidth = Textbox.Document.GetFormattedText().WidthIncludingTrailingWhitespace + 20;
         if (newWidth > Textbox.ActualWidth)
         {
           Textbox.Width = Textbox.Document.GetFormattedText().WidthIncludingTrailingWhitespace + 20;
         }

         var adn = Drawer.AdornerHelper.GetAdorner(sender);
         adn?.OnChange();
       };

      Textbox.PreviewMouseLeftButtonDown += OnSelect;
      Textbox.StylusDown += OnErase;
      Textbox.Focus();

      OwnedControl = Textbox;
      OnDoubleClick += OnDoubleClickEvent;
    }

    public new void OnSelect(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      base.OnSelect(sender, e);

      var adn = Drawer.AdornerHelper.GetAdorner(sender);

      adn?.OnChange();
    }

    public void OnDoubleClickEvent()
    {
      Drawer.IsEditMode = true;
      Textbox.IsReadOnly = false;
      Textbox.Focus();
    }

    public new void Finish()
    {
      Shadow.Opacity = 1;

      //Drawer.IsEditMode = false;
      base.Finish();
    }

    public new void EndEdit()
    {
      Textbox.IsReadOnly = true;
      Drawer.IsEditMode = false;
      Drawer.Page.Focus();
    }

    public void Undo()
    {
      Textbox.Undo();
    }

    public void Redo()
    {
      Textbox.Redo();
    }

    public XText(Drawer drawer) : base(drawer)
    {
      Drawer = drawer;
    }
  }
}
