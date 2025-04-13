using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace graphicEditor.Shapes
{
    class RegularPolygon : RoundShape
    {
        protected int NumVert;
        protected Point StartDot;
        public RegularPolygon(Point d1, Point d2, int num)
        {
            Center = d1;
            Radius = d1.Distance(d2);
            NumVert = num;
            StartDot = d2;
        }

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

            double dx = StartDot.X - Center.X;
            double dy = StartDot.Y - Center.Y;
            double startAngle = Math.Atan2(dy, dx);

            for (int i = 0; i < NumVert; i++)
            {
                double angle = startAngle + 2 * Math.PI * i / NumVert - Math.PI / 2;
                double x = Center.X + Radius * Math.Cos(angle);
                double y = Center.Y + Radius * Math.Sin(angle);

                /*
                x = Math.Max(0, Math.Min(canvas.ActualWidth, x));
                y = Math.Max(0, Math.Min(canvas.ActualHeight, y));
                */
                
                polygon.Points.Add(new Point(x, y));
            }


            canvas.Children.Add(polygon);
            return polygon;

        }
    }
}
