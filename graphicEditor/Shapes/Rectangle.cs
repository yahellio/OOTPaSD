using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace graphicEditor
{
    class Rectangle : RectangleShape
    {
        public Rectangle(Dot d1, Dot d2)
        {
            if (d1.x > d2.x)
            {
                this.TopLeft.x = d2.x;
                this.BottomRight.x = d1.x;
            }
            else
            {
                this.TopLeft.x = d1.x;
                this.BottomRight.x = d2.x;
            }

            if (d1.y > d2.y)
            {
                this.TopLeft.y = d2.y;
                this.BottomRight.y = d1.y;
            }
            else
            {
                this.TopLeft.y = d1.y;
                this.BottomRight.y = d2.y;
            }
        }

        public override System.Windows.Shapes.Shape Render(Canvas canvas)
        {
            return Render(canvas, this.Fill, this.Frame);
        }

        public override System.Windows.Shapes.Shape Render(Canvas canvas, Brush _fill, Pen _frame)
        {
            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();

            rect.Width = Math.Abs(TopLeft.x - BottomRight.x);
            rect.Height = Math.Abs(TopLeft.y - BottomRight.y);

            rect.Stroke = _frame.Brush;
            rect.StrokeThickness = _frame.Thickness;
            rect.Fill = _fill;

            rect.MouseLeftButtonUp += (sender, e) =>
            {
                var parent = canvas;
                MouseButtonEventArgs newEvent = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left)
                {
                    RoutedEvent = System.Windows.Input.Mouse.MouseUpEvent
                };
                parent.RaiseEvent(newEvent);
            };

            Canvas.SetLeft(rect, TopLeft.x);
            Canvas.SetTop(rect, TopLeft.y);
            canvas.Children.Add(rect);
         
            return rect;
        }

    }

}
