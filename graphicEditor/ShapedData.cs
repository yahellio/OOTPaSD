using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;

namespace graphicEditor
{
    public class ShapeData
    {
        public string Type { get; set; } 
        public List<Point> Points { get; set; } 
        public double StrokeThickness { get; set; } 
        public string StrokeColor { get; set; } 
        public string FillColor { get; set; } 
        public int Vertices { get; set; } 
    }
}
