using Converter2.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Converter2.Services.Converters
{
    public class ImageConverter : IConverter
    {
        public async Task ConvertAsync(string input, string output, Preset preset)
        {
            // Загружаем изображение
            using var image = await Image.LoadAsync(input);

            // Применяем настройки преобразования
            if (preset.Settings.TryGetValue("Resize", out var resize) && (bool)resize)
            {
                int width = (int)preset.Settings["Width"];
                int height = (int)preset.Settings["Height"];
                image.Mutate(x => x.Resize(width, height));
            }

            // Определяем формат для сохранения по расширению файла
            IImageEncoder encoder = GetEncoder(output);

            if (encoder == null)
            {
                throw new NotSupportedException($"Формат файла {Path.GetExtension(output)} не поддерживается");
            }

            // Сохраняем изображение
            await image.SaveAsync(output, encoder);
        }

        private IImageEncoder GetEncoder(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            return extension switch
            {
                ".png" => new PngEncoder(),
                ".jpg" or ".jpeg" => new JpegEncoder(),
                ".bmp" => new BmpEncoder(),
                ".gif" => new GifEncoder(),
                ".tiff" or ".tif" => new TiffEncoder(),
                ".webp" => new WebpEncoder(),
                _ => throw new NotSupportedException($"Формат {extension} не поддерживается")
            };
        }
    }
}