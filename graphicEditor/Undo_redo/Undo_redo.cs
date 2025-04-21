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
        private Stack<UIElement> curStack = new();
        private Stack<UIElement> redoStack = new();

        public void AddAction(UIElement element)
        {
            curStack.Push(element);
            redoStack.Clear(); 
        }

        public void Undo(Canvas canvas)
        {
            if (curStack.Count > 0)
            {
                UIElement element = curStack.Pop();
                canvas.Children.Remove(element);
                redoStack.Push(element);
            }
        }

        public void Redo(Canvas canvas)
        {
            if (redoStack.Count > 0)
            {
                UIElement element = redoStack.Pop();
                canvas.Children.Add(element);
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
