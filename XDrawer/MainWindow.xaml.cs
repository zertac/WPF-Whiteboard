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
          "M21.734375 19.640625L19.636719 21.734375C19.253906 22.121094 18.628906 22.121094 18.242188 21.734375L13 16.496094L7.761719 21.734375C7.375 22.121094 6.746094 22.121094 6.363281 21.734375L4.265625 19.640625C3.878906 19.253906 3.878906 18.628906 4.265625 18.242188L9.503906 13L4.265625 7.761719C3.882813 7.371094 3.882813 6.742188 4.265625 6.363281L6.363281 4.265625C6.746094 3.878906 7.375 3.878906 7.761719 4.265625L13 9.507813L18.242188 4.265625C18.628906 3.878906 19.257813 3.878906 19.636719 4.265625L21.734375 6.359375C22.121094 6.746094 22.121094 7.375 21.738281 7.761719L16.496094 13L21.734375 18.242188C22.121094 18.628906 22.121094 19.253906 21.734375 19.640625Z";
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
