using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Sudoku_2024;

public class Scheme {
    private const int Size = 9;

    private int _mistakes = 0;
    
    private int _hints = 0;
    
    public bool IsSolutionShowing = false;
    
    public bool IsSolutionShowingEnd = false;

    public bool ToShowPossibleMoves = false;

    private Grid _mainGrid = new Grid();

    private Button _currentButton = new Button();
    
    private bool _wasFirstClick = false;
    public string[,] Fields
    {
        get;
        set;
    } = new string[Size, Size];
    
    public string[,] SolvedFields
    {
        get;
        set;
    } = new string[Size, Size];

    public readonly Dictionary<string, Button> FieldButtons = new Dictionary<string, Button>();
    
    public Dictionary<string, Button> PossibleMovesDict = new Dictionary<string, Button>();


    // CODE THAT RESPOND FOR LIGHTING BUTTONS WITH THE SAME CONTENT.....................................................
    private void LightningUpButton(char clickedRow, char clickedColumn, string clickedContent)
    {
        int intClickedRow = Int32.Parse(clickedRow.ToString());
        int intClickedColumn = Int32.Parse(clickedColumn.ToString());

        if (!IsSolutionShowing || IsSolutionShowingEnd)
        {
            foreach (var button in FieldButtons.Values)
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

    
    // SETTING POSSIBLE MOVES TO THE BUTTON CONTENT.....................................................................
    private void SetPossibleMoves(Button clickedButton, int row, int column)
    {
        string[] possibleMoves = SudokuAlgo.GetPossibleMoves(row, column, Fields).ToArray();
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

        PossibleMovesDict[row.ToString() + column] = clickedButton;
    }
    
    // BUTTON CLICK FUNCTION............................................................................................
    private void FieldButtonClicked(object sender, RoutedEventArgs e)
    {
        char clickedRow = ' ';
        char clickedColumn = ' ';
        string clickedContent = " ";
        
        if (sender is Button clickedButton)
        {
            clickedRow = clickedButton.Name[5];
            clickedColumn = clickedButton.Name[6];
            var intClickedRow = Int32.Parse(clickedRow.ToString());
            var intClickedColumn = Int32.Parse(clickedColumn.ToString());
            clickedContent = clickedButton.Content.ToString() ?? throw new InvalidOperationException();

            _currentButton = clickedButton;
            _wasFirstClick = true;
            
            // CODE THAT RESPOND FOR SHOWING POSSIBLE MOVES (miniHINT)..................................................
            if (Fields[intClickedRow, intClickedColumn] == "0" && ToShowPossibleMoves && !IsSolutionShowing)
            {
                SetPossibleMoves(clickedButton, intClickedRow, intClickedColumn);
            }
        }
        
        LightningUpButton(clickedRow, clickedColumn, clickedContent);
    }
    private void EnterButtonClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            if (_wasFirstClick && !IsSolutionShowing)
            {
                int currentRow = Int32.Parse(_currentButton.Name[5].ToString());
                int currentColumn = Int32.Parse(_currentButton.Name[6].ToString());

                if (Fields[currentRow, currentColumn] == "0" || Fields[currentRow, currentColumn] == "10")
                {
                    PossibleMovesDict.Remove(currentRow.ToString() + currentColumn);
                    
                    _currentButton.FontSize = 30;
                    _currentButton.Content = button.Content;
                    
                    string[,] temp = Fields.Clone() as string[,] ?? throw new InvalidOperationException();
                    temp[currentRow, currentColumn] = button.Content.ToString() ?? throw new InvalidOperationException();

                    if (button.Content.ToString() == SolvedFields[currentRow, currentColumn]) 
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
                        else
                        {
                            foreach (var element in PossibleMovesDict)
                            {
                                SetPossibleMoves(element.Value, Int32.Parse(element.Key[0].ToString()),
                                    Int32.Parse(element.Key[1].ToString()));
                            }
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
                        mainWindow.Attempts.Text = "Mistakes: " + _mistakes.ToString() + "/3";

                        if (_mistakes == 3)
                        {
                            mainWindow.EndAnimation("loose");
                        }
                    }
                }
            }
        }
    }
    public void HintButtonClicked()
    {
        if (_wasFirstClick && !IsSolutionShowing)
        {
            int intClickedRow = Int32.Parse(_currentButton.Name[5].ToString());
            int intClickedColumn = Int32.Parse(_currentButton.Name[6].ToString());
            
            if (Fields[intClickedRow, intClickedColumn] == "0" || Fields[intClickedRow, intClickedColumn] == "10")
            {
                if (_hints != 3)
                {
                    PossibleMovesDict.Remove(intClickedRow.ToString() + intClickedColumn);
                    
                    _currentButton.Content = SolvedFields[intClickedRow, intClickedColumn];
                    _currentButton.Foreground = Brushes.Black;
                    _currentButton.FontSize = 30;

                    Fields[intClickedRow, intClickedColumn] = SolvedFields[intClickedRow, intClickedColumn];
            
                    LightningUpButton(_currentButton.Name[5], _currentButton.Name[6], _currentButton.Content.ToString());

                    _hints += 1;
                    var mainWindow = (MainWindow)Application.Current.MainWindow;
                    mainWindow.Hint.Text = _hints + "/3";

                    if (SudokuAlgo.IsWin(Fields))
                    {
                        mainWindow.EndAnimation("win");
                    }
                    else
                    {
                        foreach (var element in PossibleMovesDict)
                        {
                            SetPossibleMoves(element.Value, Int32.Parse(element.Key[0].ToString()),
                                Int32.Parse(element.Key[1].ToString()));
                        }
                    }
                }
                else
                {
                    var mainWindow = (MainWindow)Application.Current.MainWindow;
                    mainWindow.HintLack.Visibility = Visibility.Visible;
                }
            }
        }
    }
    
    // BUTTON SETTINGS..................................................................................................
    private void ButtonProperties(Button newBut, Brush? backColor, Brush? foreColor, int thickness)
    {
        newBut.VerticalAlignment = VerticalAlignment.Center;
        newBut.HorizontalAlignment = HorizontalAlignment.Center;
        newBut.FontSize = 30;
        newBut.FontFamily = new FontFamily("Comic Sans Ms");
        if (backColor is not null)
        {
            newBut.Background = backColor;
        }
        if (foreColor is not null)
        {
            newBut.Foreground = foreColor;
        }
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

                string first = i.ToString();
                string second = j.ToString();
                FieldButtons[first+second] = newButton;
                
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
            
            ButtonProperties(newButton, null, null, 4);
            newButton.Template = (ControlTemplate)Application.Current.MainWindow.FindResource("InputButtons");
            newButton.Click += EnterButtonClicked;
            
            myGrid.Children.Add(newButton);
        }
    }
    
    // RESTART FUNCTION.................................................................................................
    public void Restart()
    {
        _hints = 0;
        _mistakes = 0;
        var mainWindow = (MainWindow)Application.Current.MainWindow;
        mainWindow.Attempts.Text = "Mistakes: " + _mistakes.ToString() + "/3";
        mainWindow.Hint.Text = _hints + "/3";   
        mainWindow.HintLack.Visibility = Visibility.Collapsed;
        
        mainWindow.ButtonPossibleMoves.Visibility = Visibility.Visible;
        mainWindow.ButtonHints.Visibility = Visibility.Visible;
        mainWindow.ButtonShowSolution.Visibility = Visibility.Visible;
        mainWindow.Hint.Visibility = Visibility.Visible;
        mainWindow.RestartButton.Visibility = Visibility.Collapsed;
        
        _wasFirstClick = false;
        IsSolutionShowing = false;
        _currentButton = new Button();
        (Fields, SolvedFields) = Files.Read();
        ToShowPossibleMoves = false;

        // UPGRADE NEW SCHEME CONTENT
        foreach (var child in FieldButtons.Values)
        {
            child.Content = Fields[Int32.Parse(child.Name[5].ToString()), Int32.Parse(child.Name[6].ToString())] == "0"
                ? " " : Fields[Int32.Parse(child.Name[5].ToString()), Int32.Parse(child.Name[6].ToString())];
            child.Background = Brushes.White;
            child.Foreground = Brushes.Black;
            child.FontSize = 30;
        }
        
        MapGenerator.WriteIntoFile();
    }
    
    // SHOW BACKTRACKING ANIMATION......................................................................................
    public async Task<bool> BacktrackingGraphics(string[,] fields, int row=0, int column=0)
    {
        if (!IsSolutionShowing)
        {
            return true;
        }
        if (row == 9)
        {
            return true;
        }
        if (column == 9)
        {
            return await BacktrackingGraphics(fields, row + 1);
        }

        if (fields[row, column] == "0" || fields[row,column] == "10")
        {
            List<string> possibleMoves = SudokuAlgo.GetPossibleMoves(row, column, fields);
            foreach (var move in possibleMoves)
            {
                await Task.Delay(200);
                fields[row, column] = move;
                FieldButtons[row.ToString() + column].Content = move;
                FieldButtons[row.ToString() + column].Foreground = Brushes.ForestGreen;
                
                if (await BacktrackingGraphics(fields, row, column + 1))
                {
                    return true;
                }

                if (IsSolutionShowing)
                {
                    FieldButtons[row.ToString() + column].Background = Brushes.Salmon;
                    await Task.Delay(300);
                    FieldButtons[row.ToString() + column].Background = Brushes.White;
                    fields[row, column] = "0";
                    FieldButtons[row.ToString() + column].Content = " ";
                }
            }

            return false;
        }

        return await BacktrackingGraphics(fields, row, column + 1);
    }
    public async Task DrawSolution()
    {
        await Task.Delay(200);
        for (var i = 0; i < Size; i++)
        {
            for (var j = 0; j < Size; j++)
            {
                if ((string)FieldButtons[i.ToString() + j].Content == " " ||
                    FieldButtons[i.ToString() + j].Foreground == Brushes.ForestGreen)
                {
                    await Task.Delay(50);
                    FieldButtons[i.ToString() + j].Foreground = Brushes.Black;
                    FieldButtons[i.ToString() + j].Background = Brushes.White;
                    FieldButtons[i.ToString() + j].Content = SolvedFields[i, j];
                    Fields[i, j] = SolvedFields[i, j];
                }
            }
        }
    }
}