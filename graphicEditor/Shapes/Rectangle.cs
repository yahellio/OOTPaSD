using graphicEditor.ConvertJson;
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
    class Rectangle : RectangleShape, IShapeSerializable
    {
        public Rectangle(Point d1, Point d2)
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

        public override bool IsPoly => false;

        public override System.Windows.Shapes.Shape Render(Canvas canvas)
        {
            return Render(canvas, this.Fill, this.Frame);
        }

        public override System.Windows.Shapes.Shape Render(Canvas canvas, Brush _fill, Pen _frame)
        {
            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();

            rect.Width = Math.Abs(TopLeft.X - BottomRight.X);
            rect.Height = Math.Abs(TopLeft.Y - BottomRight.Y);

            this.fill = _fill;
            this.frame = _frame;

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

            Canvas.SetLeft(rect, TopLeft.X);
            Canvas.SetTop(rect, TopLeft.Y);
            canvas.Children.Add(rect);
            RenderedElement = rect;
            return rect;
        }

        public ShapeDTO ToDTO()
        {
            return new ShapeDTO
            {
                ShapeType = nameof(Rectangle),
                Data = new Dictionary<string, object>
                {
                    { "X1", TopLeft.X },
                    { "Y1", TopLeft.Y },
                    { "X2", BottomRight.X },
                    { "Y2", BottomRight.Y },
                    { "Fill", Fill?.ToString() ?? "#00000000" },
                    { "Stroke", Frame?.Brush?.ToString() ?? "#FF000000" },
                    { "Thickness", Frame?.Thickness ?? 1.0 }
                }
            };
        }

        public void FromDTO(ShapeDTO dto)
        {
            double x1 = Convert.ToDouble(dto.Data["X1"]);
            double y1 = Convert.ToDouble(dto.Data["Y1"]);
            double x2 = Convert.ToDouble(dto.Data["X2"]);
            double y2 = Convert.ToDouble(dto.Data["Y2"]);

            this.TopLeft = new Point(x1, y1);
            this.BottomRight = new Point(x2, y2);

            string fillStr = dto.Data.ContainsKey("Fill") ? dto.Data["Fill"].ToString() : "#00000000";
            string strokeStr = dto.Data.ContainsKey("Stroke") ? dto.Data["Stroke"].ToString() : "#FF000000";
            double thickness = dto.Data.ContainsKey("Thickness") ? Convert.ToDouble(dto.Data["Thickness"]) : 1.0;

            this.Fill = (Brush)new BrushConverter().ConvertFromString(fillStr);
            this.Frame = new Pen((Brush)new BrushConverter().ConvertFromString(strokeStr), thickness);
        }

    }

}
