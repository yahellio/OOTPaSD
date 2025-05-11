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
    class Polygon : RectangleShape, IShapeSerializable
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
            RenderedElement = polygon;
            return polygon;

        }

        public ShapeDTO ToDTO()
        {
            var pointList = dots.Select(p => new Dictionary<string, double>
            {
                { "X", p.X },
                { "Y", p.Y }
            }).ToList<object>(); // важно использовать object, чтобы работал JSON

            return new ShapeDTO
            {
                ShapeType = nameof(Polygon),
                Data = new Dictionary<string, object>
                {
                    { "Points", pointList },
                    { "Fill", Fill?.ToString() ?? "#00000000" },
                    { "Stroke", Frame?.Brush?.ToString() ?? "#FF000000" },
                    { "Thickness", Frame?.Thickness ?? 1.0 }
                }
            };
        }

        public void FromDTO(ShapeDTO dto)
        {
            if (dto.Data.TryGetValue("Points", out object rawPoints) && rawPoints is IEnumerable<object> pointObjs)
            {
                var loadedDots = new List<Point>();
                foreach (var ptObj in pointObjs)
                {
                    var jObj = ptObj as Newtonsoft.Json.Linq.JObject;
                    if (jObj != null)
                    {
                        double x = jObj["X"].ToObject<double>();
                        double y = jObj["Y"].ToObject<double>();
                        loadedDots.Add(new Point(x, y));
                    }
                }

                if (loadedDots.Count == 0)
                    throw new Exception("Polygon must have at least one point.");

                dots = loadedDots;

                TopLeft = new Point(dots.Min(p => p.X), dots.Min(p => p.Y));
                BottomRight = new Point(dots.Max(p => p.X), dots.Max(p => p.Y));
            }

            string fillStr = dto.Data.ContainsKey("Fill") ? dto.Data["Fill"].ToString() : "#00000000";
            string strokeStr = dto.Data.ContainsKey("Stroke") ? dto.Data["Stroke"].ToString() : "#FF000000";
            double thickness = dto.Data.ContainsKey("Thickness") ? Convert.ToDouble(dto.Data["Thickness"]) : 1.0;

            this.Fill = (Brush)new BrushConverter().ConvertFromString(fillStr);
            this.Frame = new Pen((Brush)new BrushConverter().ConvertFromString(strokeStr), thickness);
        }


    }
}
