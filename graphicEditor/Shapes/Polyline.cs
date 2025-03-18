using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace graphicEditor
{
    class Polyline : RectangleShape
    {
        public Polyline(List<Dot> tDots)
        {

            dots = tDots.Select(d => new Point(d.x, d.y)).ToList();

            TopLeft.x = tDots.Min(d => d.x);
            BottomRight.x = tDots.Max(d => d.x);
            TopLeft.y = tDots.Min(d => d.y);
            BottomRight.y = tDots.Max(d => d.y);

        }

        protected List<Point> dots;
        public override System.Windows.Shapes.Shape Render(Canvas canvas)
        {
            return Render(canvas, this.Fill, this.Frame);
        }

        public override System.Windows.Shapes.Shape Render(Canvas canvas, Brush _fill, Pen _frame)
        {
            var polyline = new System.Windows.Shapes.Polyline();


            polyline.Stroke = _frame.Brush;
            polyline.StrokeThickness = _frame.Thickness;

            polyline.MouseLeftButtonUp += (sender, e) =>
            {
                var parent = canvas;
                MouseButtonEventArgs newEvent = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left)
                {
                    RoutedEvent = System.Windows.Input.Mouse.MouseUpEvent
                };
                parent.RaiseEvent(newEvent);
            };

            polyline.MouseRightButtonDown += (sender, e) =>
            {

                var parent = canvas;
                MouseButtonEventArgs newEvent = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Right)
                {
                    RoutedEvent = System.Windows.Input.Mouse.MouseDownEvent
                };
                parent.RaiseEvent(newEvent);
            };

            polyline.Points = new PointCollection(dots);
            canvas.Children.Add(polyline);

            return polyline;

        }

    }
}
