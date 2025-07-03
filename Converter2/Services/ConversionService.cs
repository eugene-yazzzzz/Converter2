using Converter2.Models;

using Converter2.Services.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter2.Services
{
    public class ConversionService
    {
        private readonly Dictionary<string, IConverter> _converters;

        public ConversionService()
        {
            _converters = new Dictionary<string, IConverter>(StringComparer.OrdinalIgnoreCase)
            {
                // 1. Конвертеры изображений
                [".jpg=>.png"] = new ImageConverter(),
                [".png=>.jpg"] = new ImageConverter(),
                [".bmp=>.jpg"] = new ImageConverter(),
                [".webp=>.png"] = new ImageConverter(),

                // 2. Конвертеры аудио
                [".mp3=>.wav"] = new MediaConverter(),
                [".wav=>.mp3"] = new MediaConverter(),
                [".flac=>.mp3"] = new MediaConverter(),
                [".aac=>.wav"] = new MediaConverter(),

                // 3. Конвертеры видео
                [".mp4=>.avi"] = new MediaConverter(),
                [".avi=>.mp4"] = new MediaConverter(),
                [".mov=>.mkv"] = new MediaConverter(),

                // 4. Конвертеры документов (включая TXT)
                [".txt=>.pdf"] = new DocumentConverter(),
                [".txt=>.docx"] = new DocumentConverter(),
                [".txt=>.html"] = new DocumentConverter(),
                [".docx=>.pdf"] = new DocumentConverter(),
                [".pdf=>.docx"] = new DocumentConverter(),

                // 5. Конвертеры таблиц
                [".txt=>.xlsx"] = new SpreadsheetConverter(),
                [".csv=>.xlsx"] = new SpreadsheetConverter(),

                // 6. Конвертеры презентаций
                [".txt=>.pptx"] = new PresentationConverter(),

                // 7. Специальные конвертеры (текст в изображения)
                [".txt=>.jpg"] = new ImageConverter(),
                [".txt=>.png"] = new ImageConverter()
            };
        }

        public async Task ConvertAsync(string inputPath, string outputPath, Preset preset, FormatEnum outputFormat)
        {
            if (!File.Exists(inputPath))
                throw new FileNotFoundException("Input file not found", inputPath);

            if (!FileValidator.Validate(inputPath))
                throw new InvalidOperationException("Invalid input file format");

            string inputExt = Path.GetExtension(inputPath).ToLower();
            string outputExt = Path.GetExtension(outputPath).ToLower();

            string converterKey = $"{inputExt}=>{outputExt}";

            if (!_converters.TryGetValue(converterKey, out IConverter converter))
                throw new NotSupportedException($"Conversion from {inputExt} to {outputExt} not supported");

            await converter.ConvertAsync(inputPath, outputPath, preset);
        }
        public bool CanConvert(string inputExt, string outputExt)
        {
            string converterKey = $"{inputExt}=>{outputExt}";
            return _converters.ContainsKey(converterKey);
        }
    }
}
