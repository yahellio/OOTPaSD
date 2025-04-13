using graphicEditor.Factory;
using System;
using System.Linq;
using System.Reflection;

namespace graphicEditor.Plugins
{
    public static class PluginLoader
    {
        public static void RegisterAllShapes()
        {
            // get current build
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