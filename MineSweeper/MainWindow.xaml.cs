using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MineSweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private int seconds = 0;
        private readonly DispatcherTimer timer;
        private readonly GameManger mgr;

        public MainWindow()
        {
            InitializeComponent();
            mgr = GameManger.GetInstance(GameStateChangeHandler);
            SetPlayField(false);
            timer = StartTimeThread();
        }

        private void SetPlayField(bool reset)
        {
            if (reset) mgr.Reset();
            BombLabel.Content = "Bombs: " + mgr.AmountOfBombs;
            LevelLabel.Content = "Level: " + mgr.Level;
            MineGrid.Children.Clear();
            MineGrid.Children.Add(mgr.GetPlayField());
        }

        private void ResetClickHandler(object sender, RoutedEventArgs e)
        {
            mgr.Reset();
            SetPlayField(true);
        }

        private void PrevClickHandler(object sender, RoutedEventArgs e)
        {
            mgr.Prev();
            SetPlayField(true);

            if (!NextBtn.IsEnabled)
            {
                NextBtn.IsEnabled = true;
            }
        }

        private void NextClickHandler(object sender, RoutedEventArgs e)
        {
            mgr.Next();
            SetPlayField(true);

            if (!PrevBtn.IsEnabled)
            {
                PrevBtn.IsEnabled = true;
            }
        }

        private DispatcherTimer StartTimeThread()
        {
            DispatcherTimer timer = new()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += (object? sender, EventArgs e) =>
            {
                seconds++;
                TimeLabel.Content = "Time: " + seconds;
            };
            timer.Start();
            return timer;
        }

        private void GameStateChangeHandler(GameManagerEvents events)
        {
            switch (events)
            {
                case GameManagerEvents.MinLevelReached:
                    PrevBtn.IsEnabled = false;
                    break;

                case GameManagerEvents.MaxLevelReached:
                    NextBtn.IsEnabled = false;
                    break;

                case GameManagerEvents.Win:
                case GameManagerEvents.Loose:
                    timer.Stop();
                    new GameOverWindow(GameOverWindowCallback, events == GameManagerEvents.Win).ShowDialog();
                    break;
            }

        }

        private void GameOverWindowCallback(bool nextLevel)
        {
            RoutedEventArgs e = new();
            if(nextLevel)
            {
                NextClickHandler(NextBtn, e);
                return;
            }

            ResetClickHandler(ResetBtn, e);             
        }

    }
}