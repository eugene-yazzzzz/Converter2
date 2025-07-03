using Converter2.Models;
using Spire.Xls;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Converter2.Services.Converters
{
    public class SpreadsheetConverter : IConverter
    {
        public Task ConvertAsync(string input, string output, Preset preset)
        {
            return Task.Run(() =>
            {
                var fromExt = Path.GetExtension(input).ToLower();
                var toExt = Path.GetExtension(output).ToLower();

                if (fromExt != ".txt")
                    throw new NotSupportedException("Only TXT input is supported.");

                Workbook workbook = new Workbook();
                Worksheet sheet = workbook.Worksheets[0];

                string[] lines = File.ReadAllLines(input);
                for (int i = 0; i < lines.Length; i++)
                {
                    sheet.Range[i + 1, 1].Text = lines[i];
                }

                if (toExt == ".xls")
                    workbook.SaveToFile(output, ExcelVersion.Version97to2003);
                else if (toExt == ".xlsx")
                    workbook.SaveToFile(output, ExcelVersion.Version2016);
                else
                    throw new NotSupportedException($"Output format {toExt} is not supported.");
            });
        }
    }
}