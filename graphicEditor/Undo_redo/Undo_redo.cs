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
    class Undo_redo
    {
        private Stack<List<UIElement>> curStack = new Stack<List<UIElement>>();
        private Stack<List<UIElement>> redoStack = new Stack<List<UIElement>>();

        public void Undo()
        {
            if (curStack.Count > 0)
            {
                redoStack.Push(curStack.Pop());
            }
        }

        public void Redo()
        {
            if (redoStack.Count > 0)
            {
                curStack.Push(redoStack.Pop());
            }
        }

        public void Clear()
        {
            curStack.Clear();
            redoStack.Clear();
        }

    }
}
