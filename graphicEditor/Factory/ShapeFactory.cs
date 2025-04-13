using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicEditor.Factory
{
    public static class ShapeFactory
    {
        private static Dictionary<string, Type> shapeTypes = new Dictionary<string, Type>();

        public static void RegisterShape(string shapeName, Type shapeType)
        {
            shapeTypes[shapeName] = shapeType;
        }
        public static MainShape CreateShape(string shapeName, params object[] parameters)
        {
            if (shapeTypes.ContainsKey(shapeName))
            {
                Type type = shapeTypes[shapeName];

                var constructor = type.GetConstructors().FirstOrDefault();

                return (MainShape)constructor.Invoke(parameters);
            }
            throw new ArgumentException($"Shape '{shapeName}' is not registered.");
        }

        public static Type GetType(string shapeName)
        {
            if (shapeTypes.ContainsKey(shapeName))
            {         
                return shapeTypes[shapeName];
            }
            throw new ArgumentException($"Shape '{shapeName}' is not registered.");
        }

        public static List<string> GetRegisteredShapes()
        {
            return shapeTypes.Keys.ToList();
        }

        public static bool IsShapeRegistered(string shapeName)
        {
            return shapeTypes.ContainsKey(shapeName);
        }
    }
}
