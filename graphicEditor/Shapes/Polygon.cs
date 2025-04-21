using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace graphicEditor
{
    class Polygon : RectangleShape
    {
        public Polygon(List<Point> tDots)
        {
            dots = tDots;

            TopLeft.X = tDots.Min(d => d.X);
            BottomRight.X = tDots.Max(d => d.X);
            TopLeft.Y = tDots.Min(d => d.Y);
            BottomRight.Y = tDots.Max(d => d.Y);
        }

        public override bool IsPoly => true;

        protected List<Point> dots;
        public override System.Windows.Shapes.Shape Render(Canvas canvas)
        {
            return Render(canvas, this.Fill, this.Frame);
        }

        public override System.Windows.Shapes.Shape Render(Canvas canvas, Brush _fill, Pen _frame)
        {
            var polygon = new System.Windows.Shapes.Polygon();

            polygon.Stroke = _frame.Brush;
            polygon.StrokeThickness = _frame.Thickness;
            polygon.Fill = _fill;

            polygon.MouseLeftButtonUp += (sender, e) =>
            {
                var parent = canvas;
                MouseButtonEventArgs newEvent = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left)
                {
                    RoutedEvent = System.Windows.Input.Mouse.MouseUpEvent
                };
                parent.RaiseEvent(newEvent);
            };

            polygon.MouseRightButtonDown += (sender, e) =>
            {

                var parent = canvas;
                MouseButtonEventArgs newEvent = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Right)
                {
                    RoutedEvent = System.Windows.Input.Mouse.MouseDownEvent
                };
                parent.RaiseEvent(newEvent);
            };

            polygon.Points = new PointCollection(dots);
            canvas.Children.Add(polygon);

            return polygon;

        }

    }
}
