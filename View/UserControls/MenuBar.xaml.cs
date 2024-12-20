using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace FolderFit.View.UserControls
{
    public partial class MenuBar : UserControl
    {
        public MenuBar()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.CloseFunc();
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.MaximizeFunc();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.MinimizeFunc();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left)
            {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.DragMove();
            }
        }
    }
}
