using Converter2.Models;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter2.Services.Converters
{
    public class ImageConverter : IConverter
    {
        public async Task ConvertAsync(string input, string output, Preset preset)
        {
            using var image = await Image.LoadAsync(input);

            if (preset.Settings.TryGetValue("Resize", out var resize) && (bool)resize)
            {
                int width = (int)preset.Settings["Width"];
                int height = (int)preset.Settings["Height"];
                image.Mutate(x => x.Resize(width, height));
            }

            await image.SaveAsync(output);
        }
    }
}
