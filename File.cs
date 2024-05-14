using System.IO;

namespace Sudoku_2024;

public static class Files
{
    public static string[,] Read()
    {
        // TODO change for a dynamic folder location
        const string fileName = "C:\\Users\\anton\\RiderProjects\\Sudoku_2024\\Sudoku_2024\\schemes\\scheme_solve.txt"; 
        
        string[,] splitText = new string[9,9];

        if (File.Exists(fileName))
        {
            string[] allText = File.ReadAllLines(fileName);

            for (var i = 0; i < 9; i++)
            {
                var j = 0;
                foreach (var number in allText[i].Split(' '))
                {
                    splitText[i, j] = number;
                    j++;
                }
            }
        }

        return splitText;
    }
}