using Converter2.Models;
using Spire.Presentation;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Converter2.Services.Converters
{
    public class PresentationConverter : IConverter
    {
        public Task ConvertAsync(string input, string output, Preset preset)
        {
            return Task.Run(() =>
            {
                var fromExt = Path.GetExtension(input).ToLower();
                var toExt = Path.GetExtension(output).ToLower();

                if (fromExt != ".txt")
                    throw new NotSupportedException("Only TXT input is supported.");

                Presentation presentation = new Presentation();
                ISlide slide = presentation.Slides.Append();
                IAutoShape shape = slide.Shapes.AppendShape(
                    ShapeType.Rectangle,
                    new System.Drawing.RectangleF(50, 50, 600, 400)
                );
                shape.TextFrame.Text = File.ReadAllText(input);

                if (toExt == ".ppt")
                    presentation.SaveToFile(output, FileFormat.PPT);
                else if (toExt == ".pptx")
                    presentation.SaveToFile(output, FileFormat.Pptx2019);
                else
                    throw new NotSupportedException($"Output format {toExt} is not supported.");
            });
        }
    }
}