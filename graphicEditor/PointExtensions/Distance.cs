using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace graphicEditor
{
    public static class PointExtensions  
    {
        public static double Distance(this Point point1, Point point2)  
        {
            double deltaX = point1.X - point2.X;
            double deltaY = point1.Y - point2.Y;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY); 
        }
    }

}
