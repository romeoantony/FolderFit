using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace FolderFit.View.UserControls
{
    public partial class ClearableTextBox : UserControl
    {
        public ClearableTextBox()
        {
            InitializeComponent();
        }

        private string placeholder;

        public string Placeholder
        {
            get { return placeholder; }
            set
            {
                placeholder = value;
                tbPlaceholder.Text = placeholder;
            }
        }

        private string folderPath;

        public string FolderPath
        {
            get { return folderPath; }
            set
            {
                folderPath = value;
                txtInput.Text = folderPath;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtInput.Clear();
            txtInput.Focus();
        }

        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtInput.Text))
                tbPlaceholder.Visibility = Visibility.Visible;
            else
                tbPlaceholder.Visibility = Visibility.Hidden;
            folderPath = txtInput.Text;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            mainWindow.UpdateLog("Selecting folder...");
            try
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    DialogResult result = dialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        string folder = dialog.SelectedPath;
                        folderPath = folder;
                        txtInput.Text = folder;
                    }
                }
                mainWindow.UpdateLog("Folder selected : " + folderPath);
            }
            catch (Exception ex)
            {
                mainWindow.UpdateLog("An error occurred while selecting folder.\nError: " + ex.Message);
            }
        }
    }
}
