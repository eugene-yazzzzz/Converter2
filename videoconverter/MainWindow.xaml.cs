using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<VideoFile> Files { get; set; } = new ObservableCollection<VideoFile>();
        private CancellationTokenSource cts = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();
            lvFiles.ItemsSource = Files;
        }

        private void AddFiles_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Видео файлы|*.mp4;*.avi;*.mkv;*.mov;*.flv|Все файлы|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (var file in dialog.FileNames)
                {
                    var format = GetFileFormat(file);
                    Files.Add(new VideoFile
                    {
                        FilePath = file,
                        FileName = Path.GetFileName(file),
                        InputFormat = format,
                        Status = "Ожидание"
                    });
                }
            }
        }

        private string GetFileFormat(string filePath)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffprobe.exe",
                        Arguments = $"-v error -show_entries format=format_name -of default=noprint_wrappers=1:nokey=1 \"{filePath}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return output.Trim().ToUpper();
            }
            catch
            {
                return "N/A";
            }
        }

        private async void Convert_Click(object sender, RoutedEventArgs e)
        {
            cts = new CancellationTokenSource();
            string outputFormat = ((ComboBoxItem)cmbFormat.SelectedItem).Content.ToString();
            string preset = ((ComboBoxItem)cmbPreset.SelectedItem).Content.ToString();

            pbOverall.Value = 0;
            pbOverall.Maximum = Files.Count;

            var tasks = Files.Select(file => ConvertFileAsync(file, outputFormat, preset, cts.Token));
            await Task.WhenAll(tasks);

            MessageBox.Show("Конвертация завершена!", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task ConvertFileAsync(VideoFile file, string outputFormat, string preset, CancellationToken token)
        {
            try
            {
                file.Status = "В процессе";
                string outputFile = Path.ChangeExtension(file.FilePath, $".converted.{outputFormat.ToLower()}");

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg.exe",
                        Arguments = $"-i \"{file.FilePath}\" -preset {preset} \"{outputFile}\"",
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    },
                    EnableRaisingEvents = true
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data) && e.Data.Contains("time="))
                    {
                        var timeStr = e.Data.Split(new[] { "time=" }, StringSplitOptions.None)[1].Split(' ')[0];
                        if (TimeSpan.TryParse(timeStr, out var currentTime))
                        {
                            Dispatcher.Invoke(() =>
                            {
                                file.Progress = currentTime.TotalSeconds / file.Duration.TotalSeconds * 100;
                            });
                        }
                    }
                };

                process.Start();
                process.BeginErrorReadLine();

                await Task.Run(() =>
                {
                    process.WaitForExit();
                    token.ThrowIfCancellationRequested();
                });

                Dispatcher.Invoke(() =>
                {
                    file.Status = "Готово";
                    pbOverall.Value++;
                });
            }
            catch (OperationCanceledException)
            {
                Dispatcher.Invoke(() => file.Status = "Отменено");
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => file.Status = $"Ошибка: {ex.Message}");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }
    }

    public class VideoFile : INotifyPropertyChanged
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string InputFormat { get; set; }

        private double _progress;
        public double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public TimeSpan Duration { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}