using Converter2.Models;

using Converter2.Services.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter2.Services
{
    public class ConversionService
    {
        private readonly Dictionary<FormatEnum, IConverter> _converters;

        public ConversionService()
        {
            _converters = new()
        {
            { FormatEnum.Image, new ImageConverter() },
            { FormatEnum.Audio, new MediaConverter() },
            { FormatEnum.Video, new MediaConverter() },
            { FormatEnum.Document, new DocumentConverter() }
            // Документы аналогично
        };
        }

        public Task ConvertAsync(string input, string output, Preset preset, FormatEnum format)
        {
           

            return _converters[format].ConvertAsync(input, output, preset);
        }
    }
}
