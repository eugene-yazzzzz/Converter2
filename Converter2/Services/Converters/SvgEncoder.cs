using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using Svg;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Converter2.Services.Converters
{
    public class SvgEncoder : IImageEncoder
    {
        public bool SkipMetadata { get; set; } = true;

        public void Encode<TPixel>(Image<TPixel> image, Stream stream) where TPixel : unmanaged, IPixel<TPixel>
        {
            var svgDocument = new SvgDocument
            {
                Width = image.Width,
                Height = image.Height
            };

            // Здесь должна быть сложная логика преобразования ImageSharp в SVG
            // Это упрощенный пример:
            using var tempBitmap = new System.Drawing.Bitmap(image.Width, image.Height);
            // ... код преобразования ...

            svgDocument.Write(stream);
        }

        public async Task EncodeAsync<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken)
        {
            await Task.Run(() => Encode(image, stream), cancellationToken);
        }
    }
}