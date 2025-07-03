using Converter2.Models;
using Converter2.Services;
using Converter2.ViewModels;
using System.IO;
using Microsoft.Win32;
using System.Windows;

namespace Converter2
{
    public partial class MainWindow : Window
    {
        private string _inputFilePath;
        private readonly ConversionService _conversionService = new();
        private readonly MainViewModel _viewModel = new MainViewModel();

        private readonly Dictionary<string, List<string>> SupportedConversions = new()
        {
            [".jpg"] = new() { ".png", ".bmp", ".webp", ".tiff", ".gif" },
            [".jpeg"] = new() { ".png", ".bmp", ".webp", ".tiff", ".gif" },
            [".png"] = new() { ".jpg", ".jpeg", ".bmp", ".webp", ".tiff", ".gif" },
            [".bmp"] = new() { ".jpg", ".jpeg", ".png", ".webp", ".tiff", ".gif" },
            [".webp"] = new() { ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".gif" },
            [".tiff"] = new() { ".jpg", ".jpeg", ".png", ".bmp", ".webp", ".gif" },
            [".tif"] = new() { ".jpg", ".jpeg", ".png", ".bmp", ".webp", ".gif" },
            [".gif"] = new() { ".jpg", ".jpeg", ".png", ".bmp", ".webp", ".tiff" },
            [".mp3"] = new() { ".wav", ".flac", ".aac" },
            [".wav"] = new() { ".mp3", ".flac" },
            [".mp4"] = new() { ".avi", ".mov", ".mkv" },
            [".avi"] = new() { ".mp4", ".mov" },
            [".pdf"] = new() { ".docx" },
            [".docx"] = new() { ".pdf", ".txt", ".html", ".rtf" },
            [".txt"] = new() { 
            ".pdf", ".docx", ".doc", ".html",
            ".xls", ".xlsx", ".csv",
            ".ppt", ".pptx",
            ".jpg", ".png", ".jpeg", ".bmp", ".webp",
            ".rtf", ".odt"
        },

        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectInputFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                _inputFilePath = dialog.FileName;
                InputFilePathTextBlock.Text = _inputFilePath;

                var ext = Path.GetExtension(_inputFilePath).ToLower();
                if (SupportedConversions.TryGetValue(ext, out var possibleFormats))
                {
                    FormatComboBox.ItemsSource = possibleFormats;
                    FormatComboBox.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("Для этого расширения не определены форматы конвертации.");
                    FormatComboBox.ItemsSource = null;
                }
            }
        }

        private async void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_inputFilePath))
            {
                MessageBox.Show("Выберите входной файл.");
                return;
            }

            if (FormatComboBox.SelectedItem is not string selectedExt)
            {
                MessageBox.Show("Выберите формат для конвертации.");
                return;
            }

            string outputPath = Path.ChangeExtension(_inputFilePath, selectedExt);

            try
            {
                FormatEnum format = GetFormatByExtension(selectedExt);
                Preset preset = FindPresetForExtension(selectedExt) ?? CreateDefaultPreset(selectedExt);

                await _conversionService.ConvertAsync(_inputFilePath, outputPath, preset, format);
                if (format == FormatEnum.Document || format == FormatEnum.Spreadsheet || format == FormatEnum.Presentation)
                {
                    RemoveSpireDocWarning(outputPath);
                }
                MessageBox.Show($"Конвертация завершена: {outputPath}", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private Preset? FindPresetForExtension(string extension)
        {
            return _viewModel.Presets.FirstOrDefault(p =>
                p.Format.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }

        private Preset CreateDefaultPreset(string extension)
        {
            return new Preset
            {
                Name = "Default",
                Format = extension,
                Settings = new Dictionary<string, object>()
            };
        }

        private FormatEnum GetFormatByExtension(string ext)
        {
            return ext.ToLower() switch
            {
                ".jpg" or ".jpeg" or ".png" or ".bmp" or ".webp" or ".tiff" or ".tif" or ".gif" => FormatEnum.Image,
                ".mp3" or ".wav" or ".flac" or ".aac" => FormatEnum.Audio,
                ".mp4" or ".avi" or ".mov" or ".mkv" => FormatEnum.Video,
                ".pdf" or ".docx" or ".doc" or ".txt" or ".rtf" or ".odt" or ".html" => FormatEnum.Document,
                ".xls" or ".xlsx" or ".csv" or ".ods" => FormatEnum.Spreadsheet,
                ".ppt" or ".pptx" or ".odp" => FormatEnum.Presentation,
                _ => throw new NotSupportedException($"Неизвестное расширение: {ext}")
            };
        }
        private void RemoveSpireDocWarning(string filePath)
        {
            try
            {
                string content = File.ReadAllText(filePath);
                content = content.Replace("Evaluation Warning: The document was created with Spire.Doc for .NET.", "");
                File.WriteAllText(filePath, content);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить предупреждение Spire.Doc: {ex.Message}");
            }
        }
    }
}