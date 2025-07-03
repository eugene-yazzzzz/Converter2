using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Converter2.Models;
using Spire.Doc;
using UglyToad.PdfPig;

namespace Converter2.Services.Converters
{
    public class DocumentConverter : IConverter
    {
        public Task ConvertAsync(string input, string output, Preset preset)
        {
            return Task.Run(() =>
            {
                var fromExt = Path.GetExtension(input).ToLower();
                var toExt = Path.GetExtension(output).ToLower();

                if (fromExt == ".txt")
                {
                    ConvertTxtToDocument(input, output, toExt);
                }
                else if (fromExt == ".docx" && toExt == ".pdf")
                {
                    ConvertDocxToPdf(input, output);
                }
                else if (fromExt == ".docx" && toExt == ".txt")
                {
                    ConvertDocxToTxt(input, output);
                }
                else if (fromExt == ".pdf" && toExt == ".txt")
                {
                    ConvertPdfToTxt(input, output);
                }
                else
                {
                    throw new NotSupportedException($"Конвертация {fromExt} → {toExt} не поддерживается.");
                }
            });
        }

        private void ConvertTxtToDocument(string input, string output, string format)
        {
            var doc = new Document();
            doc.LoadFromFile(input, FileFormat.Txt);

            switch (format)
            {
                case ".pdf":
                    doc.SaveToFile(output, FileFormat.PDF);
                    break;
                case ".docx":
                    doc.SaveToFile(output, FileFormat.Docx);
                    break;
                case ".doc":
                    doc.SaveToFile(output, FileFormat.Doc);
                    break;
                case ".html":
                    doc.SaveToFile(output, FileFormat.Html);
                    break;
                default:
                    throw new NotSupportedException($"Формат {format} не поддерживается.");
            }
        }

        private void ConvertDocxToPdf(string input, string output)
        {
            var doc = new Document();
            doc.LoadFromFile(input);
            doc.SaveToFile(output, FileFormat.PDF);
        }

        private void ConvertDocxToTxt(string input, string output)
        {
            var doc = new Document();
            doc.LoadFromFile(input);
            File.WriteAllText(output, doc.GetText());
        }

        private void ConvertPdfToTxt(string input, string output)
        {
            using var pdf = PdfDocument.Open(input);
            var sb = new StringBuilder();
            foreach (var page in pdf.GetPages())
            {
                sb.AppendLine(page.Text);
            }
            File.WriteAllText(output, sb.ToString());
        }
        public IEnumerable<string> OutputFormats => new List<string>
        {
            "PDF", "DOCX", "DOC", "XLS", "XLSX", "PPT", "PPTX",
            "JPG", "PNG", "HTML", "JPEG", "TXT"
        };
    }
}
