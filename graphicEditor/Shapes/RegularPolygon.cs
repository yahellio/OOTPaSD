using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using graphicEditor.ConvertJson;

namespace graphicEditor.Shapes
{
    class RegularPolygon : RoundShape, IShapeSerializable
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

        public override bool IsPoly => false;

        public override System.Windows.Shapes.Shape Render(Canvas canvas)
        {
            return Render(canvas, this.Fill, this.Frame);
        }
            
        public override System.Windows.Shapes.Shape Render(Canvas canvas, Brush _fill, Pen _frame)
        {   
            var polygon = new System.Windows.Shapes.Polygon();

            this.fill = _fill;
            this.frame = _frame;

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
            RenderedElement = polygon;
            return polygon;

        }

        public ShapeDTO ToDTO()
        {
            return new ShapeDTO
            {
                ShapeType = nameof(RegularPolygon),
                Data = new Dictionary<string, object>
                {
                { "CenterX", Center.X },
                { "CenterY", Center.Y },
                { "StartX", StartDot.X },
                { "StartY", StartDot.Y },
                { "NumVert", NumVert },
                { "Fill", Fill.ToString() },
                { "Stroke", Frame.Brush.ToString() },
                { "Thickness", Frame.Thickness }
                }
            };
        }


        public void FromDTO(ShapeDTO dto)
        {
            double centerX = Convert.ToDouble(dto.Data["CenterX"]);
            double centerY = Convert.ToDouble(dto.Data["CenterY"]);
            double startX = Convert.ToDouble(dto.Data["StartX"]);
            double startY = Convert.ToDouble(dto.Data["StartY"]);
            int numVert = Convert.ToInt32(dto.Data["NumVert"]);

            this.Center = new Point(centerX, centerY);
            this.StartDot = new Point(startX, startY);
            this.NumVert = numVert;
            this.Radius = Center.Distance(StartDot);

            string fillStr = dto.Data.ContainsKey("Fill") ? dto.Data["Fill"].ToString() : "#00000000";
            string strokeStr = dto.Data.ContainsKey("Stroke") ? dto.Data["Stroke"].ToString() : "#FF000000";
            double thickness = dto.Data.ContainsKey("Thickness") ? Convert.ToDouble(dto.Data["Thickness"]) : 1.0;

            this.Fill = (Brush)new BrushConverter().ConvertFromString(fillStr);
            this.Frame = new Pen((Brush)new BrushConverter().ConvertFromString(strokeStr), thickness);
        }


    }
}
