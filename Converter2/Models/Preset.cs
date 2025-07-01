using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter2.Models
{
    public class Preset    
    {
        public string Name { get; set; }
        public string Format { get; set; }
        public Dictionary<string, object> Settings { get; set; }
    }
}
