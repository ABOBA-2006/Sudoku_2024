using System.IO;

namespace Sudoku_2024;

public static class Files
{
    public static (string[,], string[,]) Read()
    {
        MapGenerator.WriteIntoFile();
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string schemesPath = Directory.GetParent(currentDirectory).Parent.Parent.Parent.FullName;;
        string fileName = Path.Combine(schemesPath, "schemes", "scheme.txt");
        string fileAnswerName = Path.Combine(schemesPath, "schemes", "scheme_solved.txt");
        
        
        string[,] splitText = new string[9,9];
        string[,] splitAnswerText = new string[9,9];

        if (File.Exists(fileName) && File.Exists(fileAnswerName))
        {
            string[] allText = File.ReadAllLines(fileName);
            string[] allAnswerText = File.ReadAllLines(fileAnswerName);

            for (var i = 0; i < 9; i++)
            {
                var j = 0;
                foreach (var number in allText[i].Split(' '))
                {
                    splitText[i, j] = number;
                    j++;
                }
                
                j = 0;
                foreach (var number in allAnswerText[i].Split(' '))
                {
                    splitAnswerText[i, j] = number;
                    j++;
                }
            }
        }

        return (splitText, splitAnswerText);
    }
}