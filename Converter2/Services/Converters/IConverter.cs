using Converter2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter2.Services.Converters
{
    public interface IConverter
    {
        Task ConvertAsync(string input, string output, Preset preset);
    }
}
