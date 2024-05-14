using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Sudoku_2024;

public class Scheme {
    private const int Size = 9;

    private int _mistakes = 0;

    public bool ToShowPossibleMoves = false;

    private Grid _mainGrid = new Grid();

    private Button _currentButton = new Button();
    
    private bool _wasFirstClick = false;
    public string[,] Fields
    {
        get;
        set;
    } = new string[Size, Size];


    // CODE THAT RESPOND FOR LIGHTING BUTTONS WITH THE SAME CONTENT.....................................................
    private void LightningUpButton(char clickedRow, char clickedColumn, string clickedContent)
    {
        int intClickedRow = Int32.Parse(clickedRow.ToString());
        int intClickedColumn = Int32.Parse(clickedColumn.ToString());
            
        foreach (var child in _mainGrid.Children)
        {
            if (child is Button button)
            {
                if (button.Name.StartsWith("Block"))
                {
                    if (Fields[intClickedRow, intClickedColumn] == "10")
                    {
                        if (button.Name[5] == clickedRow || button.Name[6] == clickedColumn ||
                            (Int32.Parse(button.Name[5].ToString()) / 3 == intClickedRow / 3 
                             && Int32.Parse(button.Name[6].ToString()) / 3 == intClickedColumn / 3))
                        {
                            if (button.Content.ToString() == clickedContent && 
                                !(clickedRow == button.Name[5] && clickedColumn == button.Name[6]))
                            {
                                button.Background = Brushes.LightSalmon;
                                continue;
                            }
                        }
                    }
                    if (Fields[Int32.Parse(button.Name[5].ToString()), Int32.Parse(button.Name[6].ToString())] == "10")
                    {
                        button.Background = Brushes.Salmon;
                        continue;
                    }
                    if (button.Content.ToString() == clickedContent && button.Content.ToString() != " " && 
                        !(clickedRow == button.Name[5] && clickedColumn == button.Name[6]) )
                    {
                        if (Fields[intClickedRow, intClickedColumn] != "0" )
                        {
                            button.Background = Brushes.CornflowerBlue;
                            continue;
                        }
                    }
                
                    if (button.Name[5] == clickedRow && button.Name[6] == clickedColumn)
                    {
                        button.Background = Brushes.LightSkyBlue;
                    } else if (button.Name[5] == clickedRow || button.Name[6] == clickedColumn ||
                               (Int32.Parse(button.Name[5].ToString()) / 3 == intClickedRow / 3 
                                && Int32.Parse(button.Name[6].ToString()) / 3 == intClickedColumn / 3))
                    {
                        button.Background = Brushes.LightBlue;
                    } else
                    {
                        button.Background = Brushes.White;
                    }
                }
            }
        }
    }
    
    // BUTTON CLICK FUNCTION............................................................................................
    private void FieldButtonClicked(object sender, RoutedEventArgs e)
    {
        char clickedRow = ' ';
        char clickedColumn = ' ';
        int intClickedRow = 0;
        int intClickedColumn = 0;
        string clickedContent = " ";
        
        if (sender is Button clickedButton)
        {
            clickedRow = clickedButton.Name[5];
            clickedColumn = clickedButton.Name[6];
            intClickedRow = Int32.Parse(clickedRow.ToString());
            intClickedColumn = Int32.Parse(clickedColumn.ToString());
            clickedContent = clickedButton.Content.ToString() ?? throw new InvalidOperationException();

            _currentButton = clickedButton;
            _wasFirstClick = true;
            
            // CODE THAT RESPOND FOR SHOWING POSSIBLE MOVES (miniHINT)..................................................
            if (Fields[intClickedRow, intClickedColumn] == "0" && ToShowPossibleMoves)
            {
                string[] possibleMoves = SudokuAlgo.GetPossibleMoves(intClickedRow, intClickedColumn, Fields).ToArray();
                clickedButton.FontSize = 10;
                clickedButton.Foreground = Brushes.Gray;
                string possibleMovesString = "";

                int counter = 1;
                foreach (var symbol in possibleMoves)
                {
                    if (counter % 3 == 1 && counter!=1)
                    {
                        possibleMovesString += "\n";
                    }
                    
                    possibleMovesString += symbol + ' ';
                    counter++;
                }
                
                clickedButton.Content = possibleMovesString;
            }
        }
        
        LightningUpButton(clickedRow, clickedColumn, clickedContent);
    }
    private void EnterButtonClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            if (_wasFirstClick)
            {
                int currentRow = Int32.Parse(_currentButton.Name[5].ToString());
                int currentColumn = Int32.Parse(_currentButton.Name[6].ToString());

                if (Fields[currentRow, currentColumn] == "0" || Fields[currentRow, currentColumn] == "10")
                {
                    _currentButton.FontSize = 30;
                    _currentButton.Content = button.Content;
                    
                    string[,] temp = Fields.Clone() as string[,] ?? throw new InvalidOperationException();
                    temp[currentRow, currentColumn] = button.Content.ToString() ?? throw new InvalidOperationException();

                    if (SudokuAlgo.SolveSudoku(temp) && 
                        SudokuAlgo.GetPossibleMoves(currentRow, currentColumn, Fields).Contains(button.Content.ToString())) 
                    {
                        Fields[currentRow, currentColumn] = button.Content.ToString() ?? throw new InvalidOperationException();
                        _currentButton.Foreground = Brushes.Black;
                        _currentButton.Background = Brushes.LightSkyBlue;
                        LightningUpButton(_currentButton.Name[5], _currentButton.Name[6],
                                            Fields[currentRow, currentColumn]);
                        
                        var mainWindow = (MainWindow)Application.Current.MainWindow;
                        if (SudokuAlgo.IsWin(Fields))
                        {
                            mainWindow.EndAnimation("win");
                        }
                    }
                    else
                    {
                        Fields[currentRow, currentColumn] = "10";
                        _currentButton.Foreground = Brushes.Crimson;
                        _currentButton.Background = Brushes.Salmon;
                        LightningUpButton(_currentButton.Name[5], _currentButton.Name[6],
                            button.Content.ToString() ?? throw new InvalidOperationException());
                        _mistakes += 1;
                        var mainWindow = (MainWindow)Application.Current.MainWindow;
                        mainWindow.AttemptsTextBlock.Text = "Mistakes: " + _mistakes.ToString() + "/3";

                        if (_mistakes == 3)
                        {
                            mainWindow.EndAnimation("loose");
                        }
                    }
                }
            }
        }
    }
    
    // BUTTON SETTINGS..................................................................................................
    private void ButtonProperties(Button newBut, Brush backColor, Brush foreColor, int thickness)
    {
        newBut.VerticalAlignment = VerticalAlignment.Center;
        newBut.HorizontalAlignment = HorizontalAlignment.Center;
        newBut.FontSize = 30;
        newBut.FontFamily = new FontFamily("Comic Sans Ms");
        newBut.Background = backColor;
        newBut.Foreground = foreColor;
        newBut.Width = 60;
        newBut.Height = 60;
        newBut.BorderBrush = Brushes.Black;
        newBut.BorderThickness = new Thickness(thickness);
        newBut.Cursor=Cursors.Hand;
    }
    
    // VISUAL BORDERS DRAWING...........................................................................................
    private void SchemeBorders()
    {
        for (var i = 0; i <= 9; i++)
        {
            Line newLine = new Line();
            Line newLine2 = new Line();
            
            Grid.SetRow(newLine, i+1);
            Grid.SetColumn(newLine, 2);
            Grid.SetColumnSpan(newLine,9);
            Grid.SetRow(newLine2, 1);
            Grid.SetColumn(newLine2, i+2);
            Grid.SetRowSpan(newLine2,9);
            
            if (i == 0 || i == 9)
            {
                newLine.StrokeThickness = 15;
                newLine2.StrokeThickness = 15;
            }
            else if(i == 3 || i == 6)
            {
                newLine.StrokeThickness = 9;
                newLine2.StrokeThickness = 9;
            }
            else
            {
                newLine.StrokeThickness = 2;
                newLine2.StrokeThickness = 2;
            }
            
            newLine.X1 = 0;
            newLine.X2 = 540;
            newLine.Y1 = 0;
            newLine.Y2 = 0;
            newLine.Stroke = Brushes.Black;
            
            newLine2.X1 = 0;
            newLine2.X2 = 0;
            newLine2.Y1 = 0;
            newLine2.Y2 = 540;
            newLine2.Stroke = Brushes.Black;

            _mainGrid.Children.Add(newLine);
            _mainGrid.Children.Add(newLine2);
        }
    }
    
    // CREATING XAML BUTTONS............................................................................................
    public void CreateFields(Grid myGrid)
    {
        _mainGrid = myGrid;
        const int iSlip = 1;
        const int jSlip = 2;
        
        for (var i = 0; i < Size; i++)
        {
            for (var j = 0; j < Size; j++)
            {
                Button newButton = new Button();

                newButton.Content = Fields[i, j] != "0" ? Fields[i, j] : " ";
                
                Grid.SetRow(newButton, i+iSlip);
                Grid.SetColumn(newButton, j+jSlip);
                newButton.Name = "Block" + i + j;
                newButton.Click += FieldButtonClicked;
                
                ButtonProperties(newButton, Brushes.White,Brushes.Black,0);
                myGrid.Children.Add(newButton);
            }
        }
        SchemeBorders();
    }
    public void CreateInputButtons(Grid myGrid)
    {
        for (var i = 0; i < Size; i++)
        {
            Button newButton = new Button();

            newButton.Content = i+1;
            
            Grid.SetRow(newButton, i+1);
            Grid.SetColumn(newButton, 0);

            ButtonProperties(newButton, Brushes.DarkCyan, Brushes.Ivory, 4);
            newButton.Click += EnterButtonClicked;
            
            myGrid.Children.Add(newButton);
        }
    }
    
    // RESTART FUNCTION.................................................................................................
    public void Restart(string[,] schemeFromFile)
    {
        _mistakes = 0;
        var mainWindow = (MainWindow)Application.Current.MainWindow;
        mainWindow.AttemptsTextBlock.Text = "Mistakes: " + _mistakes.ToString() + "/3";
        
        _wasFirstClick = false;
        _currentButton = new Button();
        Fields = schemeFromFile;
        ToShowPossibleMoves = false;

        // UPGRADE NEW SCHEME CONTENT
        int k1 = 0;
        int k2 = 0;
        foreach (var child in _mainGrid.Children)
        {
            if (child is Button button)
            {
                if (button.Name.StartsWith("Block"))
                {
                    if (k2 == 9)
                    {
                        k2 = 0;
                        k1 += 1;
                    }
                    
                    if (k1 == 9)
                    {
                        return;
                    }

                    if (Fields[k1, k2] == "0")
                    {
                        button.Content = " "; 
                    }
                    else
                    {
                        button.Content = Fields[k1, k2];
                    }
                    
                    button.Background = Brushes.White;
                    
                    k2 += 1;
                }
            }
        }
    }
}