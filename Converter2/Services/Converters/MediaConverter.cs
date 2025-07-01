using Converter2.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter2.Services.Converters
{
    public class MediaConverter : IConverter
    {
        public Task ConvertAsync(string input, string output, Preset preset)
        {
            return Task.Run(() =>
            {
                var args = preset.Settings["Arguments"].ToString()
                    .Replace("{input}", input)
                    .Replace("{output}", output);

                var process = new Process
                {
                    StartInfo = new()
                    {
                        FileName = "ffmpeg.exe",
                        Arguments = args,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                    throw new Exception(process.StandardError.ReadToEnd());
            });
        }
    }
}
