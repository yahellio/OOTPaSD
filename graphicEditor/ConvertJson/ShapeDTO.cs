using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicEditor.ConvertJson
{
    public class ShapeDTO
    {
        public string ShapeType { get; set; }
        public Dictionary<string, dynamic> Data { get; set; }
    }
}
