using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
namespace graphicEditor.Undo_redo
{
    class Undo_redo
    {
        private Stack<List<UIElement>> undoStack = new Stack<List<UIElement>>();
        private Stack<List<UIElement>> redoStack = new Stack<List<UIElement>>();

        public void SaveState(Canvas DrawingArea)
        {
            var currentState = new List<UIElement>(DrawingArea.Children.Cast<UIElement>());
            undoStack.Push(currentState);

            redoStack.Clear();
        }

        public Canvas Undo(Canvas DrawingArea, bool isDrawing)
        {
            if (isDrawing || undoStack.Count == 0) return DrawingArea;

            //del current state
            if (redoStack.Count == 0) undoStack.Pop();

            var currentState = new List<UIElement>(DrawingArea.Children.Cast<UIElement>());
            redoStack.Push(currentState);


            var previousState = undoStack.Pop();
            DrawingArea.Children.Clear();
            foreach (var element in previousState)
            {
                DrawingArea.Children.Add(element);
            }

            return DrawingArea;
        }

        public Canvas Redo(Canvas DrawingArea, bool isDrawing)
        {
            if (isDrawing || redoStack.Count == 0) return DrawingArea;

            var currentState = new List<UIElement>(DrawingArea.Children.Cast<UIElement>());
            undoStack.Push(currentState);

            var nextState = redoStack.Pop();

            //т.к. после последнего redo текущее состояние никуда не сохранялось и пропадало
            if(redoStack.Count == 0)
            {
                undoStack.Push(nextState);
            }

            DrawingArea.Children.Clear();
            foreach (var element in nextState)
            {
                DrawingArea.Children.Add(element);
            }

            return DrawingArea;
        }

    }
}
