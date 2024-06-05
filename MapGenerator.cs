using System.IO;

namespace Sudoku_2024;

public static class MapGenerator
{

    private const int Size = 9;
    private  static readonly string[,] Field = new string[Size,Size];
    private  static string[,] _answerField = new string[Size,Size];

    private static void CreateScheme()
    {
        for(var k=0; k<Size; k++)
        {
            for (var k1 = 0; k1 < Size; k1++)
            {
                Field[k, k1] = "0";
            }
        }
        var rand = new Random();
        int i = rand.Next(0, Size);
        int j = rand.Next(0, Size);
        string value = rand.Next(1, 10).ToString();
        
        Field[i, j] = value;

        SudokuAlgo.SolveSudoku(Field);
        _answerField = Field.Clone() as string[,] ?? throw new InvalidOperationException();
        
        while (!SudokuAlgo.HasMultiSolutions(Field))
        {
            i = rand.Next(0, Size);
            j = rand.Next(0, Size);
            value = Field[i, j];
            
            Field[i, j] = "0";
        }
        Field[i, j] = value;
    }

    public static void WriteIntoFile()
    {
        CreateScheme();
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string schemesPath = Directory.GetParent(currentDirectory).Parent.Parent.Parent.FullName;;
        string fileName = Path.Combine(schemesPath, "schemes", "scheme.txt");
        string fileAnswerName = Path.Combine(schemesPath, "schemes", "scheme_solved.txt");

        using StreamWriter writer = new StreamWriter(fileName);
        using StreamWriter writer2 = new StreamWriter(fileAnswerName);
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (j != Size - 1)
                {
                    writer.Write(Field[i, j] + " ");
                    writer2.Write(_answerField[i, j] + " ");
                }
                else
                {
                    writer.Write(Field[i, j]);
                    writer2.Write(_answerField[i, j]);
                }
                
            }
            writer.WriteLine();
            writer2.WriteLine();
        }
    }
}