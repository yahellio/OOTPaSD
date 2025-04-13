using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace graphicEditor
{
    public class Line : RectangleShape
    {
        public Line(Point d1, Point d2) {
            this.TopLeft = d1;
            this.BottomRight = d2;
        }

        public override System.Windows.Shapes.Shape Render(Canvas canvas)
        {
            return Render(canvas, this.Fill, this.Frame);
        }

        public override System.Windows.Shapes.Shape Render(Canvas canvas, Brush _fill, Pen _frame)
        {
            System.Windows.Shapes.Line line = new System.Windows.Shapes.Line();
                
            line.X1 = this.TopLeft.X;
            line.Y1 = this.TopLeft.Y;
            line.X2 = this.BottomRight.X;
            line.Y2 = this.BottomRight.Y;

            line.Stroke = _frame.Brush;
            line.StrokeThickness = _frame.Thickness;

            line.MouseLeftButtonUp += (sender, e) =>
            {
                var parent = canvas;
                MouseButtonEventArgs newEvent = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left)
                {
                    RoutedEvent = System.Windows.Input.Mouse.MouseUpEvent
                };
                parent.RaiseEvent(newEvent);
            };

            line.MouseRightButtonDown += (sender, e) =>
            {

                var parent = canvas;
                MouseButtonEventArgs newEvent = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Right)
                {
                    RoutedEvent = System.Windows.Input.Mouse.MouseDownEvent
                };
                parent.RaiseEvent(newEvent);
            };

            canvas.Children.Add(line);
            return line;
        }
    }
}
