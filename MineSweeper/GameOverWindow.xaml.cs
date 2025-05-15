using System.ComponentModel;
using System.Windows;


namespace MineSweeper
{
    /// <summary>
    /// Interaktionslogik für GameOver.xaml
    /// </summary>
    public partial class GameOverWindow : Window
    {

        private readonly Action<bool> callback;
        private bool wasCalled = false;
        public GameOverWindow(Action<bool> callback, bool wasWon)
        {            
            InitializeComponent();
            this.callback = callback;
            
            Message.Text = wasWon ? "You Won!" : "You Lost.";
            BtnNext.Visibility = wasWon ? Visibility.Visible : Visibility.Collapsed;
            Closed += OnWindowClosing;

            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            this.Left = mainWindow.Left / 2 + (mainWindow.Width - this.ActualWidth) / 2;
            this.Top = mainWindow.Top / 2 + (mainWindow.Height - this.ActualHeight) / 2;
        }

        private void OnWindowClosing(object? sender, EventArgs e)
        {
            if(!wasCalled) callback(false);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            wasCalled = true;
            callback(false);
            Close(); 
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            wasCalled = true;
            callback(true);
            Close();
        }

    }
}
