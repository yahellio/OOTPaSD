using System;
using System.IO;
using System.Reflection;
using System.Windows;
using graphicEditor.Factory;

namespace graphicEditor.Plugins
{
    public static class PluginLoader
    {
        public static void LoadFromDll(string path)
        {
            try
            {
                if (!File.Exists(path)) throw new FileNotFoundException("Файл не найден", path);

                Assembly pluginAssembly = Assembly.LoadFrom(path);
               
                foreach (var type in pluginAssembly.GetTypes())
                {
                
                    if (type.IsClass && !type.IsAbstract && typeof(MainShape).IsAssignableFrom(type))
                    {
                        string shapeName = type.Name;
                    
                        if (!ShapeFactory.IsShapeRegistered(shapeName))
                        {
                            ShapeFactory.RegisterShape(shapeName, type);
                          
                            System.Windows.MessageBox.Show($"Фигура с именем {shapeName} и типом {type} успешно зарегистрирована!");
                        }

                    }
                }
            
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при загрузке плагина: {ex.Message}");
            }
        }

        public static void RegisterAllShapes()
        {
            
            var assembly = Assembly.GetExecutingAssembly();

            foreach (var type in assembly.GetTypes())
            {

                if (typeof(MainShape).IsAssignableFrom(type) && !type.IsAbstract)
                {

                    var shapeName = type.Name;

                    if (!ShapeFactory.IsShapeRegistered(shapeName))
                    {
                        ShapeFactory.RegisterShape(shapeName, type);
                        Console.WriteLine($"Зарегистрирована фигура: {shapeName}");
                    }
                }
            }
        }
    }
}
