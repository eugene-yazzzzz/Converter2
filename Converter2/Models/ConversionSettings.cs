using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter2.Models
{
    class ConversionSettings
    {
        public bool Resize { get; set; } = false;
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
