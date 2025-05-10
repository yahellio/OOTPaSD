using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace graphicEditor
{
    public abstract class MainShape
    {
        protected Pen frame; //color and thickness
        protected Brush fill; //fill color
        public UIElement RenderedElement { get; set; }

        public abstract bool IsPoly { get; }

        public Pen Frame
        {
            get
            {
                return frame;
            }
            set
            {
                frame = value;
            }
        }

        public Brush Fill
        {
            get
            {
                return fill;
            }
            set
            {
                fill = value;
            }
        }

        abstract public System.Windows.Shapes.Shape Render(Canvas canvas);
        abstract public System.Windows.Shapes.Shape Render(Canvas canvas, Brush _fill, Pen _frame);
    }
}

