using graphicEditor.Factory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.IO;
using System.Windows.Media;

namespace graphicEditor.ConvertJson
{
    public static class ShapeSerializer
    {
        public static void Save(List<MainShape> shapes, string path)
        {
            var serializable = shapes
                .Where(s => s is IShapeSerializable)
                .Cast<IShapeSerializable>()
                .Select(s => s.ToDTO())
                .ToList();

            File.WriteAllText(path, JsonConvert.SerializeObject(serializable, Formatting.Indented));
        }

        public static void Load(Canvas canvas, string path, Stack<MainShape> logicalShapes)
        {
            if (!File.Exists(path)) return;

            canvas.Children.Clear();

            var json = File.ReadAllText(path);
            var dtos = JsonConvert.DeserializeObject<List<ShapeDTO>>(json);

            if (dtos == null) return;

            for (int i = dtos.Count - 1; i >= 0; i--)
            {
                var dto = dtos[i];
                if (!ShapeFactory.IsShapeRegistered(dto.ShapeType))
                    continue;

                var shape = ShapeFactory.CreateShape(dto.ShapeType);

                if (shape is IShapeSerializable serializable)
                {
                    serializable.FromDTO(dto);
                }

                shape.Render(canvas);

                logicalShapes.Push(shape);
            }
        }

    }
}
