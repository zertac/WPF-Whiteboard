using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using XDrawerLib;
using XDrawerLib.Drawers;
using XDrawerLib.Helpers;
using XDrawerLib.Helpers.Adorners;

namespace XDrawer.Whiteboard
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public Drawer Drawer;
    public MainWindow()
    {
      InitializeComponent();

      this.Loaded += MainWindow_Loaded;

      Drawer = new Drawer(MainCanvas);
      Drawer.ContinuousDraw = true;

      BtnNone.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.DrawTool = Tool.None;
      };

      BtnRectangle.Click += delegate (object sender, RoutedEventArgs args)
       {
         Drawer.DrawTool = Tool.Rectangle;
       };

      BtnEllipse.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.DrawTool = Tool.Ellipse;
      };

      BtnTriangle.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.DrawTool = Tool.Triangle;
      };

      BtnLine.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.DrawTool = Tool.Line;
      };

      BtnStyle.Click += delegate (object sender, RoutedEventArgs args)
      {
        var style = new DrawerStyle();
        style.Border = new SolidColorBrush(Colors.Black);
        style.Background = new SolidColorBrush(Colors.Red);
        style.Opacity = 1;
        style.BorderSize = 3;

        Drawer.UndoHelper.AddStep(UndoHelper.ActionType.SetStyle, Drawer.GetSelectedObject(), new Point(), new Size(), Drawer.GetSelectedObject().Tag.ToType<XShape>().Style);

        Drawer.GetSelectedObject().Tag.ToType<XShape>().Style = style;
      };

      BtnText.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.DrawTool = Tool.Text;
      };

      ChkCont.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.ContinuousDraw = ChkCont.IsChecked == true ? true : false;
      };

      BtnObjectCount.Click += delegate (object sender, RoutedEventArgs args)
      {
        MessageBox.Show(Drawer.Objects.Count.ToString());
      };

      BtnInk.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.DrawTool = Tool.Ink;
      };

      BtnArrow.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.DrawTool = Tool.Arrow;
      };

      BtnCustom.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.CustomShapeData =
          "M16 2.59375L15.28125 3.28125L2.28125 16.28125L3.71875 17.71875L5 16.4375L5 28L14 28L14 18L18 18L18 28L27 28L27 16.4375L28.28125 17.71875L29.71875 16.28125L16.71875 3.28125 Z M 16 5.4375L25 14.4375L25 26L20 26L20 16L12 16L12 26L7 26L7 14.4375Z";
        Drawer.DrawTool = Tool.Custom;
      };

      BtnClean.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.CleanPage();
      };

      BtnBind.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.HotKeyHelper.Bind(KeyFunction.Delete, Key.J);
      };
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {

    }

    //private void Btn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    //{
    //  var svg = new SvgDocument();
    //  var colorServer = new SvgColourServer(System.Drawing.Color.Black);

    //  var group = new SvgGroup { Fill = colorServer, Stroke = colorServer };
    //  svg.Children.Add(group);

    //  var strokeList = InkCanvas.Strokes.ToList();

    //  foreach (var stroke in strokeList)
    //  {
    //    var geometry = stroke.GetGeometry(stroke.DrawingAttributes).GetOutlinedPathGeometry();

    //    var s = XamlWriter.Save(geometry);

    //    var element = XElement.Parse(s);

    //    var data = element.Attribute("Figures")?.Value;

    //    var path = new SvgPath();

    //    path.PathData = SvgPathBuilder.Parse(data);
    //    path.Fill = colorServer;
    //    path.Stroke = colorServer;

    //    var border = new Border();
    //    border.Width = geometry.Bounds.Width;
    //    border.Height = geometry.Bounds.Height;
    //    border.Background = new SolidColorBrush(Colors.Transparent);
    //    Canvas.SetLeft(border, geometry.Bounds.Left);
    //    Canvas.SetTop(border, geometry.Bounds.Top);


    //    var p = new Path();
    //    p.Data = geometry;
    //    p.Fill = new SolidColorBrush(Colors.Black);
    //    p.VerticalAlignment = VerticalAlignment.Stretch;
    //    p.HorizontalAlignment = HorizontalAlignment.Stretch;
    //    //p.Width = geometry.Bounds.Width;
    //    //p.Height = geometry.Bounds.Height;
    //    p.Stretch = Stretch.Fill;


    //    //Canvas.SetLeft(p, geometry.Bounds.Left);
    //    //Canvas.SetTop(p, geometry.Bounds.Top);

    //    InkCanvas.Strokes.Remove(stroke);
    //    border.Child = p;

    //    MainCanvas.Children.Add(border);

    //    InkCanvas.Visibility = Visibility.Hidden;

    //    border.PreviewMouseLeftButtonDown += P_PreviewMouseLeftButtonDown;
    //    border.PreviewMouseLeftButtonUp += P_PreviewMouseLeftButtonUp;
    //    //group.Children.Add(new SvgPath
    //    //{
    //    //  PathData = SvgPathBuilder.Parse(data),
    //    //  Fill = colorServer,
    //    //  Stroke = colorServer
    //    //});
    //    //if (s.IsNotNullOrEmpty())
    //    //{
    //    //  var element = XElement.Parse(s);

    //    //  var data = element.Attribute("Figures")?.Value;

    //    //  if (data.IsNotNullOrEmpty())
    //    //  {
    //    //    group.Children.Add(new SvgPath
    //    //    {
    //    //      PathData = SvgPathBuilder.Parse(data),
    //    //      Fill = colorServer,
    //    //      Stroke = colorServer
    //    //    });
    //    //  }
    //  }
    //}
  }
}
