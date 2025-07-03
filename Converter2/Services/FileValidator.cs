using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Converter2.Services
{
    public static class FileValidator
    {
        private static readonly Dictionary<string, List<byte[]>> _signatures = new()
    {
        { ".mp3", new() { new byte[] {0xFF,0xFB}, new byte[] {0x49,0x44,0x33}}},
        { ".wav", new() { new byte[] {0x52,0x49,0x46,0x46}}},
        { ".jpg", new() { new byte[] {0xFF,0xD8,0xFF}}},
        { ".jpeg", new() { new byte[] {0xFF,0xD8,0xFF}} },
        { ".png", new() { new byte[] {0x89,0x50,0x4E,0x47}}},
        { ".txt", new() } 
    };

        public static bool Validate(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLower();
            if (!_signatures.ContainsKey(ext)) return false;

            // Для TXT всегда возвращаем true
            if (ext == ".txt") return true;

            var signatures = _signatures[ext];
            var longest = signatures.Max(s => s.Length);
            var header = new byte[longest];

            using var stream = File.OpenRead(filePath);
            stream.Read(header, 0, longest);

            return signatures.Any(sig => header.Take(sig.Length).SequenceEqual(sig));
        }
    }
}
