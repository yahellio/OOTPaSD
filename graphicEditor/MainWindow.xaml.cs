﻿using System.Text;
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

    public MainWindow()
    {
        InitializeComponent();

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

        /*
        switch (currType)
        {
            case ShapeType.stLine:
                var line = new Line(d1, d2);
                return line.Render(DrawingArea, FillColorBrush, pen);

            case ShapeType.stRectangle:
                var rect = new Rectangle(d1, d2);
                return rect.Render(DrawingArea, FillColorBrush, pen);
                
            case ShapeType.stElipse:
                var ellips = new Ellipse(d1, d2);
                return ellips.Render(DrawingArea, FillColorBrush, pen);

            case ShapeType.stPolyline:
                DrawingArea.Children.Remove(previewPolyElem);
                var line2 = new Line(d1, d2);
                previewPolyElem = line2.Render(DrawingArea, FillColorBrush, pen);
                var polyLine = new Polyline(CordList);
                return polyLine.Render(DrawingArea, FillColorBrush, pen);
            case ShapeType.stPolygon:
                DrawingArea.Children.Remove(previewPolyElem);
                var line3 = new Line(d1, d2);
                previewPolyElem = line3.Render(DrawingArea, FillColorBrush, pen);
                var polyg = new Polygon(CordList);
                return polyg.Render(DrawingArea, FillColorBrush, pen);
            
            case ShapeType.stRegPolygon:
                if (int.TryParse(tbSides.Text, out int sides) && sides >= 3)
                {
                }
                else
                {
                    VertNum = 4;
                    tbSides.Text = VertNum.ToString();
                    MessageBox.Show("Количество сторон должно быть целым числом не меньше 3. Установлено значение по умолчанию: 4.");
                }
                var regPol = new RegularPolygon(d1, d2, VertNum);
                return regPol.Render(DrawingArea, FillColorBrush, pen);
           
            default:
                return null;

        }
        */
    }

    private void UndoDrawing(object sender, RoutedEventArgs e)
    {
        if (isDrawing) return;
        DrawingArea.Children.Remove(previewElem);
    }

    private void RedoDrawing(object sender, RoutedEventArgs e)
    {
        if (isDrawing) return;
        if (!DrawingArea.Children.Contains(previewElem))
        {
            DrawingArea.Children.Add(previewElem);
        }
    }

    private void ClearDrawingArea(object sender, RoutedEventArgs e)
    {
        if (isDrawing) return;
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
}