using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.Serialization.Formatters;
using System.Windows.Shapes;
namespace graphicEditor.Undo_redo
{
    public class Undo_redo
    {
        public Stack<MainShape> curStack = new();
        private Stack<MainShape> redoStack = new();

        public void AddAction(MainShape element)
        {
            curStack.Push(element);
            redoStack.Clear(); 
        }

        public void Undo(Canvas canvas)
        {
            if (curStack.Count > 0)
            {
                MainShape element = curStack.Pop();

                canvas.Children.Remove(element.RenderedElement);

                redoStack.Push(element);
            }
        }

        public void Redo(Canvas canvas)
        {
            if (redoStack.Count > 0)
            {
                MainShape element = redoStack.Pop();

                canvas.Children.Add(element.RenderedElement);

                curStack.Push(element);

            }
        }

        public void Reset()
        {
            curStack.Clear();
            redoStack.Clear();
        }
    }


}
