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

                if (fromExt == ".docx" && toExt == ".pdf")
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
                else if (fromExt == ".txt" && toExt == ".pdf")
                {
                    ConvertTxtToPdf(input, output);
                }
                else
                {
                    throw new NotSupportedException($"Конвертация {fromExt} → {toExt} не поддерживается.");
                }
            });
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

        private void ConvertTxtToPdf(string input, string output)
        {
            var text = File.ReadAllText(input);
            var doc = new Document();
            var section = doc.AddSection();
            var paragraph = section.AddParagraph();
            paragraph.AppendText(text);
            doc.SaveToFile(output, FileFormat.PDF);
        }
    }
}
