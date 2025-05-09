﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphicEditor.ConvertJson
{
    interface IShapeSerializable
    {
        ShapeDTO ToDTO();
        void FromDTO(ShapeDTO dto);
    }
}
