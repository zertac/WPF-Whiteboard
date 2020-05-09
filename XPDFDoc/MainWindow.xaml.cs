using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Svg;
using XPDFDoc.Drawers;
using XPDFDoc.Helpers;
using Type = XPDFDoc.Drawers.Type;

namespace XPDFDoc
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {

    private UIElement _activeElement;

    public MainWindow()
    {
      InitializeComponent();

      this.Loaded += MainWindow_Loaded;

      //InkCanvas.Visibility = Visibility.Hidden;

      Drawer.Initialize(MainCanvas);
      Drawer.ContinuousDraw = true;

      //this.PreviewMouseLeftButtonDown += MainWindow_PreviewMouseLeftButtonDown;
      //this.BtnArrow.PreviewMouseLeftButtonDown += Btn_MouseLeftButtonDown;
      //BtnDraw.PreviewMouseLeftButtonDown += BtnDraw_PreviewMouseLeftButtonDown;

      BtnNone.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.DrawType = Type.None;
      };

      BtnRectangle.Click += delegate (object sender, RoutedEventArgs args)
       {
         Drawer.DrawType = Type.Rectangle;
       };

      BtnEllipse.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.DrawType = Type.Ellipse;
      };

      BtnTriangle.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.DrawType = Type.Triangle;
      };

      BtnLine.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.DrawType = Type.Line;
      };

      BtnStyle.Click += delegate (object sender, RoutedEventArgs args)
      {
        //var a = new DrawerStyle();
        //a.Background = new SolidColorBrush(Colors.Transparent);
        //a.Border = new SolidColorBrush(Colors.Blue);
        //a.BorderSize = 3;
        //a.Opacity = 1;

        //StyleHelper.CurrentStyle = a;

        Drawer.GetSelectedObject().SetTextStyle(FontSizeProperty, Convert.ToDouble(32));
      };

      BtnText.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.DrawType = Type.Text;
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
        Drawer.DrawType = Type.Ink;
      };

      BtnArrow.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.DrawType = Type.Arrow;
      };

      BtnCustom.Click += delegate (object sender, RoutedEventArgs args)
      {
        Drawer.CustomShapeData =
          "M16 2.59375L15.28125 3.28125L2.28125 16.28125L3.71875 17.71875L5 16.4375L5 28L14 28L14 18L18 18L18 28L27 28L27 16.4375L28.28125 17.71875L29.71875 16.28125L16.71875 3.28125 Z M 16 5.4375L25 14.4375L25 26L20 26L20 16L12 16L12 26L7 26L7 14.4375Z";
        Drawer.DrawType = Type.Custom;
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

    private void P_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {

    }

    private void P_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      _activeElement = (UIElement)sender;

      var al = AdornerLayer.GetAdornerLayer((UIElement)sender);
      var adn = new ResizingAdorner((UIElement)sender);

      al?.Add(adn);
    }
  }
}
