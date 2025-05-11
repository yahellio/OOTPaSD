using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;
using graphicEditor.Shapes;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using graphicEditor.Plugins;
using graphicEditor.Factory;
using graphicEditor.Undo_redo;
using Microsoft.Win32;

namespace graphicEditor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    bool isDrawing;
    Point startPoint;
    int VertNum;

    MainShape lastShape;

    //Shapes
    Type currType;

    List<Point> CordList;

    Double StrokeThickness;
    Brush StrokeColorBrush;
    Brush FillColorBrush;

    private System.Windows.Shapes.Shape previewElem;
    private System.Windows.Shapes.Shape previewPolyElem;

    private Stack<List<UIElement>> undoStack = new Stack<List<UIElement>>();
    private Stack<List<UIElement>> redoStack = new Stack<List<UIElement>>();
    private graphicEditor.Undo_redo.Undo_redo undo_redo;

    public MainWindow()
    {

        InitializeComponent();

        undo_redo = new Undo_redo.Undo_redo();

        ShapeFactory.RegisterShape("Line", typeof(Line));
        ShapeFactory.RegisterShape("Rectangle", typeof(Rectangle));
        ShapeFactory.RegisterShape("Ellipse", typeof(Ellipse));
        ShapeFactory.RegisterShape("Polygon", typeof(Polygon));
        ShapeFactory.RegisterShape("RegularPolygon", typeof(RegularPolygon));
        ShapeFactory.RegisterShape("Polyline", typeof(Polyline));

        PluginLoader.RegisterAllShapes();

        isDrawing = false;
        currType = typeof(Line);

        StrokeThickness = 1;
        VertNum = 4;

        StrokeColorBrush = Brushes.Black;
        FillColorBrush = Brushes.White;

        List<SolidColorBrush> colorOptions = new()
        {
            Brushes.White,
            Brushes.Black,
            Brushes.Red,
            Brushes.Blue,
            Brushes.Green,
            Brushes.Yellow,
            Brushes.Orange,
            Brushes.Pink
        };

        cbFillColor.ItemsSource = colorOptions;
        cbStrokeColor.ItemsSource = colorOptions;

        cbStrokeColor.SelectedIndex = 1;
        cbStrokeThickness.SelectedIndex = 0;
        cbShapes.SelectedIndex = 0;
        cbFillColor.SelectedIndex = 2;

        UpdateShapeComboBox();
    }

    private void UpdateShapeComboBox()
    {
    
        cbShapes.Items.Clear();

        var shapes = ShapeFactory.GetRegisteredShapes();
        
        foreach (var shapeName in shapes)
        {
            cbShapes.Items.Add(new ComboBoxItem { Content = shapeName });
        }

        if (cbShapes.Items.Count > 0)
        {
            cbShapes.SelectedIndex = 0;
        }
    }

    private void FillColorChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cbFillColor.SelectedItem is SolidColorBrush selectedBrush)
        {
            FillColorBrush = selectedBrush;
        }
    }

    private void StrokeColorChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cbStrokeColor.SelectedItem is SolidColorBrush selectedBrush)
        {
            StrokeColorBrush = selectedBrush;
        }
    }

    private void StrokeThicknessChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cbStrokeThickness.SelectedItem is ComboBoxItem selectedItem)
        {
            string thicknessValue = selectedItem.Content.ToString();

            if (double.TryParse(thicknessValue, out double thickness))
            {
                StrokeThickness = thickness;
            }
        }
    }


    private void StartPaint(object sender, MouseButtonEventArgs e)
    {
        if (currType == typeof(RegularPolygon))
        {

            if (int.TryParse(tbSides.Text, out int sides) && sides >= 3)
            {
                VertNum = sides;
            }
            else
            {
                VertNum = 4;
                tbSides.Text = VertNum.ToString();
                MessageBox.Show("Количество сторон должно быть целым числом не меньше 3. Установлено значение по умолчанию: 4.");
                return;
            }

        }

        previewPolyElem = null;
        isDrawing = true;
        startPoint = e.GetPosition(DrawingArea);

        Point d1 = startPoint;

        CordList = new List<Point>();
        CordList.Add(d1);

        previewElem = shapeRender(d1, d1);

    }
    
    private void EndPaint(object sender, MouseButtonEventArgs e)
    {
        DrawingArea.Children.Remove(previewPolyElem);
        isDrawing = false;

        if (DrawingArea.Children.Count > 0)
        {
            undo_redo.AddAction(lastShape);
        }
    }

    private void ProcessRender(object sender, MouseEventArgs e)
    {
        if (!isDrawing) return;

        DrawingArea.Children.Remove(previewElem);

        Point d1 = startPoint;
        Point d2 = e.GetPosition(DrawingArea);

        previewElem = shapeRender(d1, d2);
       
    }

    
    private void PolyClickPaint(object sender, MouseButtonEventArgs e)
    {
        if (!isDrawing || !ShapeFactory.IsPolygon(currType.Name)) return;


        startPoint = e.GetPosition(DrawingArea);

 
        Point d = startPoint;
        CordList.Add(d);

        ProcessRender(sender, e);
       

    }



    private System.Windows.Shapes.Shape shapeRender(Point d1, Point d2)
    {
      
        var pen = new Pen() { Thickness = StrokeThickness, Brush = StrokeColorBrush }; 
        string shapeName = (cbShapes.SelectedItem as ComboBoxItem)?.Content.ToString();

        Type shapeType = ShapeFactory.GetType(shapeName);
      
        var constructor = shapeType.GetConstructors().FirstOrDefault();
        var parameters = constructor.GetParameters();

        //create array
        object[] constructorParams = new object[parameters.Length];


        for (int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i].ParameterType == typeof(Point))
            {
                constructorParams[i] = (i == 0) ? d1 : d2;
            }
            else if (parameters[i].ParameterType == typeof(int))
            {
                
                constructorParams[i] = VertNum;
            }
            else if (parameters[i].ParameterType == typeof(List<Point>))
            {
                DrawingArea.Children.Remove(previewPolyElem);
                var line2 = new Line(d1, d2);
                previewPolyElem = line2.Render(DrawingArea, FillColorBrush, pen);
                constructorParams[i] = CordList;
            }
            else
            { 
                throw new ArgumentException($"Неизвестный тип параметра: {parameters[i].ParameterType.Name}");
            }
        }

        MainShape shape = ShapeFactory.CreateShape(shapeName, constructorParams);
        System.Windows.Shapes.Shape rendShape = shape.Render(DrawingArea, FillColorBrush, pen);
        lastShape = shape;

        return rendShape;
    }

    private void ClearDrawingArea(object sender, RoutedEventArgs e)
    {
        if (isDrawing) return;
     
        DrawingArea.Children.Clear();
        undo_redo.Reset();
    }

    private void ShapeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string shapeName = (cbShapes.SelectedItem as ComboBoxItem)?.Content.ToString();
        if (string.IsNullOrEmpty(shapeName)) return;

        tbSides.Visibility = Visibility.Collapsed;
        currType = ShapeFactory.GetType(shapeName);


        if (currType == typeof(RegularPolygon))
        {
            tbSides.Visibility = Visibility.Visible;

            if (int.TryParse(tbSides.Text, out int sides) && sides >= 3)
            {
                VertNum = sides; 
            }
            else
            {
                VertNum = 4; 
                tbSides.Text = VertNum.ToString(); 
                MessageBox.Show("Количество сторон должно быть целым числом не меньше 3. Установлено значение по умолчанию: 4.");
            }
        }

    }

    private void tbSides_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (currType == typeof(RegularPolygon)) 
        {
            if (int.TryParse(tbSides.Text, out int sides) && sides >= 3)
            {
                VertNum = sides;
            }

        }
    }


    private void UndoDrawing(object sender, RoutedEventArgs e)
    {
        if (isDrawing) return;
        undo_redo.Undo(DrawingArea);
    }

    private void RedoDrawing(object sender, RoutedEventArgs e)
    {
        if (isDrawing) return;
        undo_redo.Redo(DrawingArea);
    }

    private void SaveToFile(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "JSON Files (*.json)|*.json",
            DefaultExt = ".json",
            Title = "Сохранить проект"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                ConvertJson.ShapeSerializer.Save(undo_redo.curStack.ToList(), dialog.FileName);
                MessageBox.Show("Файл успешно сохранён!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void LoadFromFile(object sender, RoutedEventArgs e)
    {
        if (isDrawing) return;

        var dialog = new OpenFileDialog
        {
            Filter = "JSON Files (*.json)|*.json",
            DefaultExt = ".json",
            Title = "Открыть проект"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                undo_redo.Reset();
                ConvertJson.ShapeSerializer.Load(DrawingArea, dialog.FileName, undo_redo.curStack);
                MessageBox.Show("Файл успешно загружен!", "Загрузка", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void LoadPlugin_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "DLL Files (*.dll)|*.dll",
            Title = "Выберите файл плагина"
        };

        if (dialog.ShowDialog() == true)
        {
            PluginLoader.LoadFromDll(dialog.FileName);
            UpdateShapeComboBox();
            MessageBox.Show("Плагин успешно загружен!");
        }
    }


}