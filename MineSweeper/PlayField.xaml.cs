using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MineSweeper
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class PlayField : UserControl
    {

        readonly MineManager mgr;
        readonly Button[][] buttons;
        public int AmountOfBombs { get => mgr.AmountOfBombs; }        
        public PlayField(MineManager mineManager)
        {
            InitializeComponent();
            mgr = mineManager;
            buttons = CreateButtons(mgr);
        }

        Button[][] CreateButtons(MineManager mineManager)
        {
            GridLength width = new(1, GridUnitType.Star);
            GridLength height = new(ButtonGrid.Height / mineManager.Height);

            Button[][] buttons = new Button[mineManager.Width][];
            for (int x = 0; x < mineManager.Width; x++)
            {
                Button[] rowButtons = new Button[mineManager.Height];

                ButtonGrid.ColumnDefinitions.Add(new() { Width = width });

                for (int y = 0; y < mineManager.Height; y++)
                {
                    ButtonGrid.RowDefinitions.Add(new RowDefinition() { Height = height });

                    Button button = new();
                    button.Click += HandleClick;
                    button.MouseRightButtonDown += HandleRightClick;
                    Grid.SetRow(button, y);
                    Grid.SetColumn(button, x);
                    button.Background = Brushes.DarkGray;

                    rowButtons[y] = button;
                    ButtonGrid.Children.Add(button);
                }
                buttons[x] = rowButtons;
            }
            return buttons;
        }

        private void HandleRightClick(object sender, MouseButtonEventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.Background != Brushes.DarkGray) return;
            if ((btn.Content == null || btn.Content.ToString() == ""))
            {
                btn.Content = "X?";
                btn.Foreground = Brushes.Red;
                return;
            }

            if (btn.Content.ToString() == "X?")
            {
                btn.Content = "";
                btn.Foreground = Brushes.Black;
            }
        }

        private void HandleClick(object sender, RoutedEventArgs e)
        { 
            Button btn = (Button)sender;
            int x = Grid.GetColumn(btn);
            int y = Grid.GetRow(btn);

            VisibleFieldState[] states = mgr.Check(x, y);

            foreach (VisibleFieldState state in states)
            {
                Button button = buttons[state.x][state.y];
                button.Background = state.type == FieldType.Mine ? Brushes.Red : Brushes.White;
                button.Foreground = GetFontColor(state);
                button.Content = CreateButtonContent(state);

                if (state.type == FieldType.Mine) mgr.OnGameOver();                
            }
            mgr.CheckForWin(states.Length);
        }
        private static SolidColorBrush GetFontColor(VisibleFieldState state)
        {
            if (state.type == FieldType.Mine)
            {
                return Brushes.White;
            }

            if (state.type != FieldType.Number)
            {
                return Brushes.Black;
            }

            return state.mineCount switch
            {
                1 => Brushes.Blue,
                2 => Brushes.Green,
                3 => Brushes.Purple,
                _ => Brushes.Red, // Everything lager than 3
            };
        }

        private static object CreateButtonContent(VisibleFieldState state)
        {
            return state.type switch
            {
                FieldType.Mine => "X",
                FieldType.Number => state.mineCount,
                _ => "" // FieldType.Clear 
            };
        }
    }
}

