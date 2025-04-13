using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace graphicEditor
{
    class Ellipse : RectangleShape
    {
        public Ellipse(Point d1, Point d2)
        {
            if (d1.X > d2.X)
            {
                this.TopLeft.X = d2.X;
                this.BottomRight.X = d1.X;
            }
            else
            {
                this.TopLeft.X = d1.X;
                this.BottomRight.X = d2.X;
            }

            if (d1.Y > d2.Y) 
            {
                this.TopLeft.Y = d2.Y;
                this.BottomRight.Y = d1.Y;
            }
            else
            {
                this.TopLeft.Y = d1.Y;
                this.BottomRight.Y = d2.Y;
            }
        }

        public override System.Windows.Shapes.Shape Render(Canvas canvas)
        {
            return Render(canvas, this.Fill, this.Frame);
        }

        public override System.Windows.Shapes.Shape Render(Canvas canvas, Brush _fill, Pen _frame)
        {
            var ecl = new System.Windows.Shapes.Ellipse();

            ecl.Width = Math.Abs(TopLeft.X - BottomRight.X);
            ecl.Height = Math.Abs(TopLeft.Y - BottomRight.Y);

            ecl.Stroke = _frame.Brush;
            ecl.StrokeThickness = _frame.Thickness;
            ecl.Fill = _fill;

            ecl.MouseLeftButtonUp += (sender, e) =>
            {
                var parent = canvas;
                MouseButtonEventArgs newEvent = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left)
                {
                    RoutedEvent = System.Windows.Input.Mouse.MouseUpEvent
                };
                parent.RaiseEvent(newEvent);
            };
            Canvas.SetLeft(ecl, TopLeft.X);
            Canvas.SetTop(ecl, TopLeft.Y);
            canvas.Children.Add(ecl);

            return ecl;

        }

    }


    
}
