using Converter2.Models;
using Converter2.Services;
using System.IO;
using Microsoft.Win32;
using System.Windows;


namespace Converter2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _inputFilePath;
        private string _outputExtension;
        private readonly ConversionService _conversionService = new();

        private readonly Dictionary<string, List<string>> SupportedConversions = new()
        {
            [".jpg"] = new() { ".png", ".bmp", ".webp" },
            [".png"] = new() { ".jpg", ".bmp", ".webp" },
            [".mp3"] = new() { ".wav", ".flac", ".aac" },
            [".wav"] = new() { ".mp3", ".flac" },
            [".mp4"] = new() { ".avi", ".mov", ".mkv" },
            [".avi"] = new() { ".mp4", ".mov" },
            [".pdf"] = new() { ".docx" },
            [".docx"] = new() { ".pdf", ".txt" },
            [".txt"] = new() { ".pdf", ".docx" },
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

                // Простой пресет-заглушка
                var preset = new Preset
                {
                    Name = "Auto",
                    Format = selectedExt,
                    Settings = new Dictionary<string, object>
                    {
                        { "Arguments", $"-i \"{{input}}\" \"{{output}}\"" } // минимальный ffmpeg шаблон
                    }
                };

                await _conversionService.ConvertAsync(_inputFilePath, outputPath, preset, format);

                MessageBox.Show($"Конвертация завершена: {outputPath}", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private FormatEnum GetFormatByExtension(string ext)
        {
            return ext switch
            {
                ".jpg" or ".png" or ".bmp" or ".webp" => FormatEnum.Image,
                ".mp3" or ".wav" or ".flac" or ".aac" => FormatEnum.Audio,
                ".mp4" or ".avi" or ".mov" or ".mkv" => FormatEnum.Video,
                ".pdf" or ".docx" or ".txt" => FormatEnum.Document,
                _ => throw new NotSupportedException($"Неизвестное расширение: {ext}")
            };
        }
    }
}