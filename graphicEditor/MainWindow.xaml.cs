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

namespace graphicEditor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    bool isDrawing;
    Point startPoint;
    int VertNum;

    //Shapes
    Type currType;

    List<Dot> CordList;

    Double StrokeThickness;
    Brush StrokeColorBrush;
    Brush FillColorBrush;

    private System.Windows.Shapes.Shape previewElem;
    private System.Windows.Shapes.Shape previewPolyElem;

    private Stack<List<UIElement>> undoStack = new Stack<List<UIElement>>();
    private Stack<List<UIElement>> redoStack = new Stack<List<UIElement>>();

    public MainWindow()
    {
        InitializeComponent();

        SaveState();

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
        previewPolyElem = null;
        isDrawing = true;
        startPoint = e.GetPosition(DrawingArea);

        Dot d1 = new Dot(startPoint);

        CordList = new List<Dot>();
        CordList.Add(d1);

        previewElem = shapeRender(d1, d1);

    }
    
    private void EndPaint(object sender, MouseButtonEventArgs e)
    {
        DrawingArea.Children.Remove(previewPolyElem);
        isDrawing = false;
        SaveState();
        SaveShapeToFile(previewElem, "shapes.json");

    }

    private void ProcessRender(object sender, MouseEventArgs e)
    {
        if (!isDrawing) return;

        DrawingArea.Children.Remove(previewElem);

        Dot d1 = new Dot(startPoint);
        Dot d2 = new Dot(e.GetPosition(DrawingArea));

        previewElem = shapeRender(d1, d2);
       
    }

    
    private void PolyClickPaint(object sender, MouseButtonEventArgs e)
    {
        if (!isDrawing || ((currType != typeof(Polygon)) && (currType != typeof(Polyline)))) return;


        startPoint = e.GetPosition(DrawingArea);

 
        Dot d = new Dot(startPoint);
        CordList.Add(d);

        ProcessRender(sender, e);
       

    }



    private System.Windows.Shapes.Shape shapeRender(Dot d1, Dot d2)
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
            if (parameters[i].ParameterType == typeof(Dot))
            {
                constructorParams[i] = (i == 0) ? d1 : d2;
            }
            else if (parameters[i].ParameterType == typeof(int))
            {
                
                constructorParams[i] = VertNum;
            }
            else if (parameters[i].ParameterType == typeof(List<Dot>))
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

        

        return shape.Render(DrawingArea, FillColorBrush, pen);
    }

    private void ClearDrawingArea(object sender, RoutedEventArgs e)
    {
        if (isDrawing) return;
        SaveState();
        DrawingArea.Children.Clear();
   
    }

    private void ShapeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string shapeName = (cbShapes.SelectedItem as ComboBoxItem)?.Content.ToString();
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


    private void SaveState()
    {
        var currentState = new List<UIElement>(DrawingArea.Children.Cast<UIElement>());
        undoStack.Push(currentState);

        redoStack.Clear();
    }

    private void UndoDrawing(object sender, RoutedEventArgs e)
    {
        if (isDrawing || undoStack.Count == 0) return;

        if (redoStack.Count == 0) undoStack.Pop();

        var currentState = new List<UIElement>(DrawingArea.Children.Cast<UIElement>());
        redoStack.Push(currentState);

   
        var previousState = undoStack.Pop();
        DrawingArea.Children.Clear();
        foreach (var element in previousState)
        {
            DrawingArea.Children.Add(element);
        }
    }

    private void RedoDrawing(object sender, RoutedEventArgs e)
    {
        if (isDrawing || redoStack.Count == 0) return;
        
        var currentState = new List<UIElement>(DrawingArea.Children.Cast<UIElement>());
        undoStack.Push(currentState);
        
        var nextState = redoStack.Pop();
        DrawingArea.Children.Clear();
        foreach (var element in nextState)
        {
            DrawingArea.Children.Add(element);
        }
    }


    private void SaveShapeToFile(System.Windows.Shapes.Shape shape, string filePath)
    {
      
        var shapeData = new ShapeData
        {
            Type = shape.GetType().Name,
            Points = CordList.Select(d => new Point(d.x, d.y)).ToList(),
            StrokeThickness = shape.StrokeThickness,
            StrokeColor = shape.Stroke.ToString(), 
            FillColor = shape.Fill?.ToString() ?? "Transparent",
            Vertices = VertNum
        };

        var shapesData = new List<ShapeData>();
        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);
            shapesData = JsonConvert.DeserializeObject<List<ShapeData>>(existingJson);
        }
        
        shapesData.Add(shapeData);

        string json = JsonConvert.SerializeObject(shapesData, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    private void LoadShapesFromFile(string filePath)
    {
        if (!File.Exists(filePath)) return;

        string json = File.ReadAllText(filePath);
        var shapesData = JsonConvert.DeserializeObject<List<ShapeData>>(json);


        DrawingArea.Children.Clear();

        foreach (var shapeData in shapesData)
        {
            var pen = new Pen
            {
                Thickness = shapeData.StrokeThickness,
                Brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(shapeData.StrokeColor))
            };

            var fillBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(shapeData.FillColor));

           
            MainShape shape = ShapeFactory.CreateShape(shapeData.Type, new object[] { shapeData.Points[0], shapeData.Points[1] });
            if (shape != null)
            {
                var renderedShape = shape.Render(DrawingArea, fillBrush, pen);
                DrawingArea.Children.Add(renderedShape);
            }
        }
    }

    private void btnLoad_Click(object sender, RoutedEventArgs e)
    {
        LoadShapesFromFile("shapes.json");
    }
}