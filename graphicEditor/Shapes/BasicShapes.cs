using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace graphicEditor
{
    public abstract class RectangleShape : MainShape
    {
        protected Point TopLeft;
        protected Point BottomRight;
    }

    public abstract class RoundShape : MainShape
    {
        protected Point Center;
        protected double Radius;
    }
}
