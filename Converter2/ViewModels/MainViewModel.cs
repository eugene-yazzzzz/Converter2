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
            Name = "JPG Web",
            Format = ".jpg",
            Settings = new Dictionary<string, object>
            {
                ["Resize"] = true,
                ["Width"] = 1280,
                ["Height"] = 720
            }
        }
    };

        public async Task Convert(string input, string output, Preset preset, FormatEnum format)
        {
            await _service.ConvertAsync(input, output, preset, format);
            //test 123
            //test 123
            //test 12313131
        }
    }
}
