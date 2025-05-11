using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

                if (constructor != null)
                    return (MainShape)constructor.Invoke(parameters.Length > 0 ? parameters : GetDefaultParameters(constructor));
            }
            throw new ArgumentException($"Shape '{shapeName}' is not registered.");
        }

        private static object[] GetDefaultParameters(ConstructorInfo constructor)
        {
            return constructor.GetParameters().Select(p =>
                (object)(
                    p.ParameterType == typeof(Point) ? new Point(0, 0) :
                    p.ParameterType == typeof(List<Point>) ? new List<Point> { new Point(0, 0), new Point(1, 1), new Point(2, 2) } :
                    p.ParameterType == typeof(int) ? 3 :
                    null
                )
            ).ToArray();
        }


        public static Type GetType(string shapeName)
        {
            if (shapeName == null)
                throw new ArgumentNullException(nameof(shapeName));

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

        public static bool IsPolygon(string shapeName)
        {
            if (shapeTypes.ContainsKey(shapeName))
            {
                Type type = shapeTypes[shapeName];

                var constructor = type.GetConstructors().FirstOrDefault();

                if (constructor != null)
                {
                    object[] defaultParams = GetDefaultParameters(constructor);

                    var instance = (MainShape)constructor.Invoke(defaultParams);
                    return instance.IsPoly;
                }
            }

            throw new ArgumentException($"Shape '{shapeName}' is not registered or does not have a suitable constructor.");
        }


    }
}
