# WPF-Whiteboard - (Under construction)

This repository contains wpf canvas based whiteboard project.

### General capabilities
- Draw basic and custom shapes
- Inkcanvas drawing
- Move, resize and delete shapes.
- Set style before drawing or change style at run time.
- Basic object selection and rectangular multiple object selection with mouse.
- Get object properties (style, position, selected object count etc)
- Custom shape - (You can override your own path data for drawing custom shapes)
- Keyboard shortcuts for simple functions
- Erase objects and ink strokes by stylus pen inverse

### Current shapes

- Rectangle
- Ellipse
- Line
- Triangle
- Text
- Ink Canvas
- Arrow
- Custom Shape

### Installation
Download repository and add to your solution. Add reference as project reference to you own project and start to use XDrawer.

### Using

Initialize XDrawer
```csharp
Drawer.Initialize({Your own canvas});
 ```
 
User your first brush
```csharp
 Drawer.DrawTool = Tool.Rectangle;
  ```
  
 Change default brush style
 ```csharp
  var style = new DrawerStyle();
  style.Border = new SolidColorBrush(Colors.Black);
  style.Background = new SolidColorBrush(Colors.Red);
  style.Opacity = 1;
  style.BorderSize = 3;
  
  StyleHelper.CurrentStyle = style;
  ```

