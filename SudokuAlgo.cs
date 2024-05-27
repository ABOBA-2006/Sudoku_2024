namespace Sudoku_2024;

public static class SudokuAlgo
{
    private static bool IsDuplicateRow(string value, int row, string[,] fields)
    {
        for (var column = 0; column < 9; column++)
        {
            if (value == fields[row, column])
            {
                return true;
            }
        }

        return false;
    }
    private static bool IsDuplicateColumn(string value, int column, string[,] fields)
    {
        for (var row = 0; row < 9; row++)
        {
            if (value == fields[row, column])
            {
                return true;
            }
        }

        return false;
    }
    private static bool IsDuplicateSquare(string value, int row, int column, string[,] fields)
    {
        var squareRow = row / 3;
        var squareColumn = column / 3;
        for (var i = squareRow*3; i < squareRow*3+3; i++)
        {
            for (var j = squareColumn * 3; j < squareColumn * 3 + 3; j++)
            {
                if (value == fields[i,j])
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    public static List<string> GetPossibleMoves(int row, int column, string[,] fields)
    {
        List<string> possibleMoves = new List<string>();
        
        for (var number = 1; number <= 9; number++)
        {
            if (!IsDuplicateRow(number.ToString(), row, fields) &&
                !IsDuplicateColumn(number.ToString(), column, fields) &&
                !IsDuplicateSquare(number.ToString(), row, column, fields))
            {
                possibleMoves.Add(number.ToString());
            }
        }

        return possibleMoves;
    }

    private static bool Backtracking(string[,] fields, int row=0, int column=0)
    {
        if (row == 9)
        {
            return true;
        }
        if (column == 9)
        {
            return Backtracking(fields, row + 1);
        }

        if (fields[row, column] == "0" || fields[row,column] == "10")
        {
            List<string> possibleMoves = GetPossibleMoves(row, column, fields);
            Random rng = new Random();
            var randomizedList = possibleMoves.OrderBy(_ => rng.Next()).ToList();
            
            foreach (var move in randomizedList)
            {
                fields[row, column] = move;
                if (Backtracking(fields, row, column + 1))
                {
                    return true;
                }

                fields[row, column] = "0";
            }

            return false;
        }

        return Backtracking(fields, row, column + 1);
    }
    
    public static bool SolveSudoku(string[,] fields)
    {
        if (Backtracking(fields))
        {
            return true;
        }
        return false;
    }
    public static bool IsWin(string[,] array)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (array[i, j] == "0")
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    // FOR MAP GENERATOR................................................................................................
    private static bool _hasOneSolution;
    private static bool _hasMultiplySolution;
    public static bool HasMultiSolutions(string[,] board) {
        _hasOneSolution = false;
        _hasMultiplySolution = false;
        string[,] temp = new string[9,9];
        Array.Copy(board, temp, board.Length);
        Solve(temp, 0, 0);
        return _hasMultiplySolution;
    }
    private static void Solve(string[,] board, int row, int col) {
        if (row == 9)
        {
            if (_hasOneSolution)
            {
                _hasMultiplySolution = true;
            }
            _hasOneSolution = true;
            return;
        }
        if (col == 9) {
            Solve(board, row + 1, 0);
            return;
        }
        if (board[row, col] != "0") {
            Solve(board, row, col + 1);
            return;
        }

        foreach (var move in GetPossibleMoves(row, col, board))
        {
            board[row, col] = move;
            Solve(board, row, col + 1);
            if (_hasMultiplySolution) return; // If multiple solutions found, exit early
            board[row, col] = "0";
        }
    }
}