using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using XPDFDoc.Helpers;

namespace XPDFDoc.Drawers
{
  public class XText : XShape, IShape
  {
    public Rectangle Shadow;
    public RichTextBox Textbox;
    //public StackPanel Container;

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

      var myFlowDoc = new FlowDocument();
      myFlowDoc.Blocks.Add(new Paragraph(new Run("Paragraph 1")));
      //Container = new StackPanel();
      Textbox = new RichTextBox
      {
        Width = Shadow.Width,
        Height = Shadow.Height,
        Background = StyleHelper.CurrentStyle.Background,
        BorderBrush = StyleHelper.CurrentStyle.Border,
        BorderThickness = new Thickness(StyleHelper.CurrentStyle.BorderSize),
        IsReadOnly = false
      };

      //Container.Children.Add(Textbox);

      Canvas.SetLeft(Textbox, Canvas.GetLeft(Shadow));
      Canvas.SetTop(Textbox, Canvas.GetTop(Shadow));

      Drawer.Page.Children.Add(Textbox);
      Drawer.Page.Children.Remove(Shadow);

      Textbox.Tag = this;

      Textbox.Focus();
      //Textbox.TextChanged += (o, e) => 
     
      Textbox.TextChanged+= delegate(object sender, TextChangedEventArgs args)
      {
        var newWidth = Textbox.Document.GetFormattedText().WidthIncludingTrailingWhitespace + 20;
        if (newWidth > Textbox.ActualWidth)
        {
          Textbox.Width = Textbox.Document.GetFormattedText().WidthIncludingTrailingWhitespace + 20;
        }
        
        var adn = AdornerHelper.GetAdorner(sender);

        if (adn != null)
        {
          adn.OnChange();
        }
      };
      Textbox.PreviewMouseLeftButtonDown += OnSelect;
      Textbox.Focus();

      OwnedControl = Textbox;

      OnDoubleClick += OnDoubleClickEvent;
    }

    public new void OnSelect(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      base.OnSelect(sender,e);

      var adn = AdornerHelper.GetAdorner(sender);

      if (adn != null)
      {
        adn.OnChange();
      }
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

      Drawer.IsEditMode = false;
      base.Finish();
    }

    public new void EndEdit()
    {
      Textbox.IsReadOnly = true;
      Drawer.IsEditMode = false;
      Drawer.Page.Focus();
    }
  }
}
