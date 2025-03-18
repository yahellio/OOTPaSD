using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicEditor
{
    public abstract class RectangleShape : MainShape
    {
        protected Dot TopLeft;
        protected Dot BottomRight;
    }

    public abstract class RoundShape : MainShape
    {
        protected Dot Center;
        protected double Radius;
    }
}
