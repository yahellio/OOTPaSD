using graphicEditor.ConvertJson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace graphicEditor
{
    public class Trapezoid : RectangleShape, IShapeSerializable
    {
        protected List<System.Windows.Point> points;

        public Trapezoid(System.Windows.Point p1, System.Windows.Point p2)
        {
            points = CalculateTrapezoidPoints(p1, p2);

            TopLeft = new System.Windows.Point(points.Min(p => p.X), points.Min(p => p.Y));
            BottomRight = new System.Windows.Point(points.Max(p => p.X), points.Max(p => p.Y));
        }

        public override bool IsPoly => false;

        public override Shape Render(Canvas canvas)
        {
            return Render(canvas, this.Fill, this.Frame);
        }

        public override Shape Render(Canvas canvas, Brush _fill, Pen _frame)
        {
            var polygon = new System.Windows.Shapes.Polygon();

            polygon.Stroke = _frame.Brush;
            polygon.StrokeThickness = _frame.Thickness;
            polygon.Fill = _fill;
            polygon.Points = new PointCollection(points);


            this.fill = _fill;
            this.frame = _frame;

            polygon.MouseLeftButtonUp += (sender, e) =>
            {
                var parent = canvas;
                var newEvent = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left)
                {
                    RoutedEvent = Mouse.MouseUpEvent
                };
                parent.RaiseEvent(newEvent);
            };

            polygon.MouseRightButtonDown += (sender, e) =>
            {
                var parent = canvas;
                var newEvent = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Right)
                {
                    RoutedEvent = Mouse.MouseDownEvent
                };
                parent.RaiseEvent(newEvent);
            };

            canvas.Children.Add(polygon);
            RenderedElement = polygon;
            return polygon;
        }

        private List<System.Windows.Point> CalculateTrapezoidPoints(System.Windows.Point start, System.Windows.Point end)
        {
            double width = end.X - start.X;
            double height = end.Y - start.Y;

            System.Windows.Point p1 = start;
            System.Windows.Point p2 = new System.Windows.Point(start.X + width, start.Y);
            System.Windows.Point p3 = new System.Windows.Point(start.X + width * 0.75, start.Y + height);
            System.Windows.Point p4 = new System.Windows.Point(start.X + width * 0.25, start.Y + height);

            return new List<System.Windows.Point> { p1, p2, p3, p4 };
        }

        public ShapeDTO ToDTO()
        {
            return new ShapeDTO
            {
                ShapeType = nameof(Trapezoid),
                Data = new Dictionary<string, object>
                {
                    { "Points", points.Select(p => new Dictionary<string, double> { { "X", p.X }, { "Y", p.Y } }).ToList() },
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
                var loadedPoints = new List<System.Windows.Point>();
                foreach (var ptObj in pointObjs)
                {
                    var jObj = ptObj as JObject;
                    if (jObj != null)
                    {
                        double x = jObj["X"].ToObject<double>();
                        double y = jObj["Y"].ToObject<double>();
                        loadedPoints.Add(new System.Windows.Point(x, y));
                    }
                }

                if (loadedPoints.Count != 4)
                    throw new Exception("Trapezoid must have exactly 4 points.");


                points = loadedPoints;
                TopLeft = new System.Windows.Point(points.Min(p => p.X), points.Min(p => p.Y));
                BottomRight = new System.Windows.Point(points.Max(p => p.X), points.Max(p => p.Y));
            }

            string fillStr = dto.Data.ContainsKey("Fill") ? dto.Data["Fill"].ToString() : "#00000000";
            string strokeStr = dto.Data.ContainsKey("Stroke") ? dto.Data["Stroke"].ToString() : "#FF000000";
            double thickness = dto.Data.ContainsKey("Thickness") ? Convert.ToDouble(dto.Data["Thickness"]) : 1.0;

            this.Fill = (Brush)new BrushConverter().ConvertFromString(fillStr);
            this.Frame = new Pen((Brush)new BrushConverter().ConvertFromString(strokeStr), thickness);
        }
    }
}
