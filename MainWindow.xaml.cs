using FolderFit.View.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace FolderFit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private static string folderPath;
        private string logData = "Hi! Welcome to FolderFit.";

        public event PropertyChangedEventHandler PropertyChanged;

        public string LogData
        {
            get { return logData; }
            set
            {
                logData = value;
                OnPropertyChanged(nameof(LogData));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateLog(string message)
        {
            logData += "\n" + message;
            OnPropertyChanged(nameof(LogData));
            scrollViewer.ScrollToEnd();
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            UpdateLog("Please select a folder to start decluttering.");
        }

        private void sortByTypeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            sortByDateRadioButton.IsChecked = false;
            UpdateLog("Sorting by type selected.");
        }

        private void sortByDateRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            sortByTypeRadioButton.IsChecked = false;
            UpdateLog("Sorting by date selected.");
        }

        

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(folderPathUC.FolderPath) || !System.IO.Directory.Exists(folderPathUC.FolderPath))
            {
                MessageBox.Show("Invalid folder path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateLog("Invalid folder path. Please select a valid folder path.");
                return;
            }
            folderPath = folderPathUC.FolderPath;
            StartDecluttering();
        }

        internal static void CloseFunc()
        {
            App.Current.Shutdown();
        }

        internal static void MinimizeFunc()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        internal static void MaximizeFunc()
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            else
                Application.Current.MainWindow.WindowState= WindowState.Maximized;
        }

        private void StartDecluttering()
        {
            if (sortByTypeRadioButton.IsChecked == true)
            {
                SortFilesByType();
                if (openFolderCheckBox.IsChecked == true)
                {
                    System.Diagnostics.Process.Start(folderPath);
                }
            }
            else if (sortByDateRadioButton.IsChecked == true)
            {
                SortFilesByDate();
                if (openFolderCheckBox.IsChecked == true)
                {
                    System.Diagnostics.Process.Start(folderPath);
                }
            }
            else
            {
                MessageBox.Show("Select a sort type", "Warning! No sort type selected!", MessageBoxButton.OK, MessageBoxImage.Warning);
                UpdateLog("No sort type selected. Please select a sort type.");
            }
        }

        private void SortFilesByType()
        {
            UpdateLog("Sorting files by type...");
            string sortedFolderPath = System.IO.Path.Combine(folderPath, "Sorted");
            if (!System.IO.Directory.Exists(sortedFolderPath))
            {
                System.IO.Directory.CreateDirectory(sortedFolderPath);
            }

            string[] files = System.IO.Directory.GetFiles(folderPath);
            string[] subDirectories = System.IO.Directory.GetDirectories(folderPath);

            int totalItems = files.Length + subDirectories.Length;
            progressBar.Maximum = totalItems;
            progressBar.Value = 0;



            foreach (string file in files)
            {
                string extension = System.IO.Path.GetExtension(file).ToLower();
                string category;
                if (!fileTypes.TryGetValue(extension, out category))
                {
                    category = "other";
                }

                string categoryFolderPath = System.IO.Path.Combine(sortedFolderPath, category);
                if (!System.IO.Directory.Exists(categoryFolderPath))
                {
                    System.IO.Directory.CreateDirectory(categoryFolderPath);
                }

                string destFilePath = System.IO.Path.Combine(categoryFolderPath, System.IO.Path.GetFileName(file));
                System.IO.File.Move(file, destFilePath);
                progressBar.Value += 1;
            }

            
            foreach (string subDirectory in subDirectories)
            {
                if (subDirectory != sortedFolderPath)
                {
                    string categoryFolderPath = System.IO.Path.Combine(sortedFolderPath, "folders");
                    if (!System.IO.Directory.Exists(categoryFolderPath))
                    {
                        System.IO.Directory.CreateDirectory(categoryFolderPath);
                    }

                    string destFolderPath = System.IO.Path.Combine(categoryFolderPath, System.IO.Path.GetFileName(subDirectory));
                    System.IO.Directory.Move(subDirectory, destFolderPath);
                    progressBar.Value += 1;
                }
            }
            UpdateLog("Files sorted by type successfully.");
        }

        private void SortFilesByDate()
        {
            UpdateLog("Sorting files by date...");
            string sortedFolderPath = System.IO.Path.Combine(folderPath, "Sorted");
            if (!System.IO.Directory.Exists(sortedFolderPath))
            {
                System.IO.Directory.CreateDirectory(sortedFolderPath);
            }

            string[] files = System.IO.Directory.GetFiles(folderPath);
            string[] subDirectories = System.IO.Directory.GetDirectories(folderPath);

            int totalItems = files.Length + subDirectories.Length;
            progressBar.Maximum = totalItems;
            progressBar.Value = 0;

            foreach (string file in files)
            {
                DateTime lastModified = System.IO.File.GetLastWriteTime(file);
                string dateFolder = lastModified.ToString("yyyy-MM-dd");
                string dateFolderPath = System.IO.Path.Combine(sortedFolderPath, dateFolder);

                if (!System.IO.Directory.Exists(dateFolderPath))
                {
                    System.IO.Directory.CreateDirectory(dateFolderPath);
                }

                string destFilePath = System.IO.Path.Combine(dateFolderPath, System.IO.Path.GetFileName(file));
                System.IO.File.Move(file, destFilePath);
                progressBar.Value += 1;
            }

            foreach (string subDirectory in subDirectories)
            {
                if (subDirectory != sortedFolderPath)
                {
                    DateTime lastModified = System.IO.Directory.GetLastWriteTime(subDirectory);
                    string dateFolder = lastModified.ToString("yyyy-MM-dd");
                    string dateFolderPath = System.IO.Path.Combine(sortedFolderPath, dateFolder);

                    if (!System.IO.Directory.Exists(dateFolderPath))
                    {
                        System.IO.Directory.CreateDirectory(dateFolderPath);
                    }

                    string destFolderPath = System.IO.Path.Combine(dateFolderPath, System.IO.Path.GetFileName(subDirectory));
                    System.IO.Directory.Move(subDirectory, destFolderPath);
                    progressBar.Value += 1;
                }
            }
            UpdateLog("Files sorted by date successfully.");
        }


        private readonly Dictionary<string, string> fileTypes = new Dictionary<string, string>
            {
                { ".jpg", "images" },
                { ".png", "images" },
                { ".gif", "images" },
                { ".jpeg", "images" },
                { ".bmp", "images" },
                { ".ico", "images" },
                { ".svg", "images" },
                { ".psd", "images" },
                { ".ai", "images" },
                { ".eps", "images" },
                { ".doc", "documents" },
                { ".xls", "documents" },
                { ".xlsx", "documents" },
                { ".ppt", "documents" },
                { ".pptx", "documents" },
                { ".csv", "documents" },
                { ".pdf", "documents" },
                { ".rtf", "documents" },
                { ".odt", "documents" },
                { ".ods", "documents" },
                { ".odp", "documents" },
                { ".odg", "documents" },
                { ".odf", "documents" },
                { ".odc", "documents" },
                { ".odm", "documents" },
                { ".docx", "documents" },
                { ".txt", "documents" },
                { ".mp4", "videos" },
                { ".avi", "videos" },
                { ".mov", "videos" },
                { ".wmv", "videos" },
                { ".flv", "videos" },
                { ".mkv", "videos" },
                { ".m4v", "videos" },
                { ".3gp", "videos" },
                { ".webm", "videos" },
                { ".m4a", "music" },
                { ".flac", "music" },
                { ".mp3", "music" },
                { ".wav", "music" },
                { ".ogg", "music" },
                { ".wma", "music" },
                { ".mid", "music" },
                { ".midi", "music" },
                { ".cpl", "installation files" },
                { ".dll", "installation files" },
                { ".msm", "installation files" },
                { ".exe", "installation files" },
                { ".msi", "installation files" },
                { ".msu", "installation files" },
                { ".msp", "installation files" },
                { ".reg", "installation files" },
                { ".lnk", "installation files" },
                { ".bat", "installation files" },
                { ".cmd", "installation files" },
                { ".vbs", "installation files" },
                { ".vbe", "installation files" },
                { ".vbscript", "installation files" },
                { ".zip", "compressed files" },
                { ".rar", "compressed files" },
                { ".7z", "compressed files" },
                { ".tar", "compressed files" },
                { ".gz", "compressed files" },
                { ".gzip", "compressed files" },
                { ".bz2", "compressed files" },
                { ".iso", "iso files" },
                { ".img", "iso files" },
                { ".bin", "iso files" },
                { ".torrent", "torrent files" },
                { ".py", "programming files" },
                { ".cs", "programming files" },
                { ".java", "programming files" },
                { ".js", "programming files" },
                { ".c", "programming files" },
                { ".cpp", "programming files" },
                { ".h", "programming files" },
                { ".swift", "programming files" },
                { ".vb", "programming files" },
                { ".csproj", "programming files" },
                { ".sln", "programming files" },
                { ".cshtml", "programming files" },
                { ".csx", "programming files" },
                { ".sql", "programming files" },
                { ".json", "programming files" },
                { ".xml", "programming files" },
                { ".html", "programming files" },
                { ".css", "programming files" },
                { ".php", "programming files" },
                { ".asp", "programming files" },
                { ".aspx", "programming files" },
                { ".asax", "programming files" },
                { ".ascx", "programming files" },
                { ".ashx", "programming files" },
                { ".asmx", "programming files" },
                { ".asc", "programming files" },
                { ".asm", "programming files" },
                { ".rmskin", "skins" },
                { ".vsix", "visual studio files" },
            };

    }
}
