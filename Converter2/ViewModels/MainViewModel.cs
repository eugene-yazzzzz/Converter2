using Converter2.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Converter2.Models;
using Converter2.Services;

namespace Converter2.ViewModels
{
    public class MainViewModel
    {
        private readonly ConversionService _service = new();

        public List<Preset> Presets { get; set; } = new()
        {
            // Аудио пресеты
            new()
            {
                Name = "MP3 High Quality",
                Format = ".mp3",
                Settings = new Dictionary<string, object>
                {
                    ["Arguments"] = "-i \"{input}\" -b:a 320k \"{output}\""
                }
            },
            new()
            {
                Name = "MP3 Medium Quality",
                Format = ".mp3",
                Settings = new Dictionary<string, object>
                {
                    ["Arguments"] = "-i \"{input}\" -b:a 192k \"{output}\""
                }
            },

            // Изображения - JPG
            new()
            {
                Name = "JPG Web Optimized",
                Format = ".jpg",
                Settings = new Dictionary<string, object>
                {
                    ["Quality"] = 80,
                    ["Resize"] = true,
                    ["Width"] = 1920,
                    ["Height"] = 1080
                }
            },
            new()
            {
                Name = "JPG High Quality",
                Format = ".jpg",
                Settings = new Dictionary<string, object>
                {
                    ["Quality"] = 90,
                    ["Resize"] = false
                }
            },

            // PNG
            new()
            {
                Name = "PNG Transparent",
                Format = ".png",
                Settings = new Dictionary<string, object>
                {
                    ["CompressionLevel"] = 6,
                    ["Resize"] = false
                }
            },
            new()
            {
                Name = "PNG Lossless",
                Format = ".png",
                Settings = new Dictionary<string, object>
                {
                    ["CompressionLevel"] = 9,
                    ["Resize"] = false
                }
            },

            // BMP
            new()
            {
                Name = "BMP 24-bit",
                Format = ".bmp",
                Settings = new Dictionary<string, object>
                {
                    ["BitsPerPixel"] = 24,
                    ["Resize"] = false
                }
            },
            new()
            {
                Name = "BMP 8-bit",
                Format = ".bmp",
                Settings = new Dictionary<string, object>
                {
                    ["BitsPerPixel"] = 8,
                    ["Resize"] = false
                }
            },

            // GIF
            new()
            {
                Name = "GIF 256 Colors",
                Format = ".gif",
                Settings = new Dictionary<string, object>
                {
                    ["ColorTableSize"] = 256,
                    ["Resize"] = false
                }
            },
            new()
            {
                Name = "GIF Animated",
                Format = ".gif",
                Settings = new Dictionary<string, object>
                {
                    ["Delay"] = 100,
                    ["LoopCount"] = 0
                }
            },

            // TIFF
            new()
            {
                Name = "TIFF LZW Compression",
                Format = ".tiff",
                Settings = new Dictionary<string, object>
                {
                    ["Compression"] = "LZW",
                    ["Resize"] = false
                }
            },

            // WebP
            new()
            {
                Name = "WebP Balanced",
                Format = ".webp",
                Settings = new Dictionary<string, object>
                {
                    ["Quality"] = 75,
                    ["Resize"] = false
                }
            },

            //PDF
            new()
            {
                Name = "PDF Standard",
                Format = ".pdf",
                Settings = new Dictionary<string, object>
                {
                    ["Quality"] = "Standard",
                    ["CompressImages"] = true
                }
            },
            new()
            {
                Name = "TXT to Image",
                Format = ".jpg",
                Settings = new Dictionary<string, object>
                {
                    ["Resize"] = true,
                    ["Width"] = 1280,
                    ["Height"] = 720,
                    ["FontFamily"] = "Arial",
                    ["FontSize"] = 24
                }
            },
            new()
            {
                Name = "TXT to Spreadsheet",
                Format = ".xlsx",
                Settings = new Dictionary<string, object>()
            },
            new()
            {
                Name = "TXT to Presentation",
                Format = ".pptx",
                Settings = new Dictionary<string, object>()
            }
        };
        public async Task Convert(string input, string output, Preset preset, FormatEnum format)
        {
            await _service.ConvertAsync(input, output, preset, format);
        }
    }
}