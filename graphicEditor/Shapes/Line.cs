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
using Newtonsoft.Json;
using graphicEditor.ConvertJson;

namespace graphicEditor
{
    public class Line : RectangleShape, IShapeSerializable
    {
        public Line(Point d1, Point d2) {
            this.TopLeft = d1;
            this.BottomRight = d2;

        }

        public override bool IsPoly => false;

        public override System.Windows.Shapes.Shape Render(Canvas canvas)
        {
            return Render(canvas, this.Fill, this.Frame);
        }

        public override System.Windows.Shapes.Shape Render(Canvas canvas, Brush _fill, Pen _frame)
        {
            System.Windows.Shapes.Line line = new System.Windows.Shapes.Line();
            
            this.fill = _fill;
            this.frame = _frame;

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
            RenderedElement = line;
            return line;
        }
        public ShapeDTO ToDTO()
        {
            return new ShapeDTO
            {
                ShapeType = nameof(Line),
                Data = new Dictionary<string, object>
                {
                    { "X1", TopLeft.X },
                    { "Y1", TopLeft.Y },
                    { "X2", BottomRight.X },
                    { "Y2", BottomRight.Y },
                    { "Fill", Fill.ToString() },
                    { "Stroke", Frame.Brush.ToString() },
                    { "Thickness", Frame.Thickness }
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
