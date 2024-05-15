using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace Sudoku_2024;

public partial class MainWindow : Window
{
    private readonly Scheme _sudokuScheme = new Scheme();
    
    public MainWindow()
    {
        InitializeComponent();

        Grid myGrid = MainGrid;

        string[,] schemeFromFile = Files.Read();
        
        _sudokuScheme.Fields = schemeFromFile;
        _sudokuScheme.CreateFields(myGrid);
        _sudokuScheme.CreateInputButtons(myGrid);
    }
    
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        SmoothAppearance();
    }
    private void SmoothAppearance()
    {
        DoubleAnimation opacityAnimation = new DoubleAnimation();
        opacityAnimation.From = 0;
        opacityAnimation.To = 1;
        opacityAnimation.Duration = TimeSpan.FromSeconds(2);

        StartGrid.BeginAnimation(Grid.OpacityProperty, opacityAnimation);
        
        var moveAnimation = (Storyboard)FindResource("MoveAnimationText");
        var moveAnimation2 = (Storyboard)FindResource("MoveAnimationButton");
        
        moveAnimation.Begin();
        moveAnimation2.Begin();
    }

    private void ButtonPossibleMoves_OnCLick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            if (button.Background == Brushes.LightBlue)
            {
                _sudokuScheme.ToShowPossibleMoves = true;
                button.Background = Brushes.Teal;
            }
            else
            {
                _sudokuScheme.ToShowPossibleMoves = false;
                button.Background = Brushes.LightBlue;
            }
        }
    }

    private void HintButtonClicked(object sender, RoutedEventArgs e)
    {
        _sudokuScheme.HintButtonClicked();
    }

    private void StartButtonClicked(object sender, RoutedEventArgs e)
    {
        DoubleAnimation opacityAnimation = new DoubleAnimation();
        opacityAnimation.From = 1;
        opacityAnimation.To = 0;
        opacityAnimation.Duration = TimeSpan.FromSeconds(2);
        
        StartGrid.BeginAnimation(Grid.OpacityProperty, opacityAnimation);
        
        DoubleAnimation opacityAnimation2 = new DoubleAnimation();
        opacityAnimation2.From = 0;
        opacityAnimation2.To = 1;
        opacityAnimation2.Duration = TimeSpan.FromSeconds(4);
        
        MainGrid.BeginAnimation(Grid.OpacityProperty, opacityAnimation2);
        MainGrid.Visibility=Visibility.Visible;
    }

    private void RestartButtonClicked(object sender, RoutedEventArgs e)
    {
        string[,] schemeFromFile = Files.Read();
        
        _sudokuScheme.Restart(schemeFromFile);
        
        DoubleAnimation opacityAnimation2 = new DoubleAnimation();
        opacityAnimation2.From = 0;
        opacityAnimation2.To = 1;
        opacityAnimation2.Duration = TimeSpan.FromSeconds(2);
        
        EndMenu.Visibility=Visibility.Collapsed;
        
        MainGrid.BeginAnimation(Grid.OpacityProperty, opacityAnimation2);
        MainGrid.Visibility = Visibility.Visible;
    }
    
    public void EndAnimation(string result)
    {
        if (result == "win")
        {
            foreach (var child in EndMenu.Children)
            {
                if (child is Polygon polygon)
                {
                    polygon.Fill = Brushes.Green;
                }
            }
            
            Result.Foreground = Brushes.Green;
            Result.Text = "YOU WIN";
            
            BorderEdge.BorderBrush = Brushes.Green;
            
            RestartButtonWin.Visibility = Visibility.Visible;
            RestartButtonLoose.Visibility = Visibility.Collapsed;
        }
        else
        {
            foreach (var child in EndMenu.Children)
            {
                if (child is Polygon polygon)
                {
                    polygon.Fill = Brushes.DarkRed;
                }
            }
            
            Result.Foreground = Brushes.DarkRed;
            Result.Text = "YOU LOOSE";
            
            BorderEdge.BorderBrush = Brushes.DarkRed;
            
            RestartButtonWin.Visibility = Visibility.Collapsed;
            RestartButtonLoose.Visibility = Visibility.Visible;
        }
        
        DoubleAnimation opacityAnimation2 = new DoubleAnimation();
        opacityAnimation2.From = 0;
        opacityAnimation2.To = 1;
        opacityAnimation2.Duration = TimeSpan.FromSeconds(3);
        
        MainGrid.Visibility=Visibility.Collapsed;
        
        EndMenu.BeginAnimation(Grid.OpacityProperty, opacityAnimation2);
        EndMenu.Visibility = Visibility.Visible;
    }
}