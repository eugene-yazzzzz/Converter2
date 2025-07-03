using Converter2.Models;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = SixLabors.ImageSharp.Color;
using Image = SixLabors.ImageSharp.Image;


namespace Converter2.Services.Converters
{
    public class ImageConverter : IConverter
    {
        public async Task ConvertAsync(string input, string output, Preset preset)
        {
            // Загружаем изображение
            using var image = await Image.LoadAsync(input);

            var fromExt = Path.GetExtension(input).ToLower();
            var toExt = Path.GetExtension(output).ToLower();
            if (fromExt == ".txt")
            {
                await ConvertTxtToImageAsync(input, output, preset);
            }

            // Применяем настройки преобразования
            else
            {
                if (preset.Settings.TryGetValue("Resize", out var resize) && (bool)resize)
                {
                    int width = (int)preset.Settings["Width"];
                    int height = (int)preset.Settings["Height"];
                    image.Mutate(x => x.Resize(width, height));
                }
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
        private async Task ConvertTxtToImageAsync(string input, string output, Preset preset)
        {
            string text = File.ReadAllText(input);
            int width = 800;
            int height = 600;
            string fontFamily = "Arial";
            float fontSize = 12;
            Color backgroundColor = Color.White;
            Color textColor = Color.Black;

            if (preset.Settings.TryGetValue("Width", out var w))
                width = (int)w;
            if (preset.Settings.TryGetValue("Height", out var h))
                height = (int)h;
            if (preset.Settings.TryGetValue("FontFamily", out var ff))
                fontFamily = (string)ff;
            if (preset.Settings.TryGetValue("FontSize", out var fs))
                fontSize = (float)fs;

            using var image = new Image<Rgba32>(width, height, backgroundColor);

            var font = SystemFonts.Get(fontFamily).CreateFont(fontSize);
            var textOptions = new RichTextOptions(font)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Origin = new PointF(10, 10),
                WrappingLength = width - 20 // Заменили WrapTextWidth на WrappingLength
            };

            image.Mutate(ctx => ctx.DrawText(textOptions, text, textColor));

            await image.SaveAsync(output);
        }
    }
}
