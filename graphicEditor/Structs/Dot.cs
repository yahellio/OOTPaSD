using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace graphicEditor
{
    public struct Dot
    {
        public double x;
        public double y;

        public Dot(Point pnt)
        {
            x = pnt.X;
            y = pnt.Y;
        }
        
        public static double Distance(Dot d1, Dot d2)
        {
            double deltaX = d1.x - d2.x;
            double deltaY = d1.y - d2.y;
            deltaX *= deltaX;
            deltaY *= deltaY;

            return Math.Sqrt(deltaY + deltaX);
        }
        
    }

}
