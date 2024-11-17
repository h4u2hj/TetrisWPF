using System.IO;

namespace TetrisWPFModel.Persistence;

public class TetrisDataAccess : ITetrisDataAccess
{
    public void Save(string path, GameGrid grid, int score)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine(score);
                for (int r = 0; r < grid.Rows; r++)
                {
                    for (int c = 0; c < grid.Columns; c++)
                    {
                        writer.Write(grid[r, c] + " ");
                    }
                }
            }
        }
        catch (Exception e)
        {
            throw new TetrisDataException(e.Message);
        }
    }

    public int LoadScore(string fileName)
    {
        int score = 0;
        try
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                score = Convert.ToInt32(reader.ReadLine());
            }
        }
        catch (Exception e)
        {
            throw new TetrisDataException(e.Message);
        }
        return score;
    }


    public GameGrid Load(string path)
    {
        GameGrid grid;
        try
        {
            using (StreamReader reader = new StreamReader(path))
            {
                reader.ReadLine();

                string line = reader.ReadLine() ?? string.Empty;
                string[] cellValues = line.Split(" ");
                if (line.Length == 144)
                {
                    grid = new GameGrid(18, 4);
                }
                else if (line.Length == 288)
                {
                    grid = new GameGrid(18, 8);
                }
                else if (line.Length == 432)
                {
                    grid = new GameGrid(18, 12);
                }
                else
                {
                    throw new TetrisDataException("Error in file length");
                }

                int i = 0;
                for (int r = 0; r < grid.Rows; r++)
                {
                    for (int c = 0; c < grid.Columns; c++)
                    {
                        grid[r, c] = Convert.ToInt32(cellValues[i]);
                        i++;
                    }
                }

                return grid;
            }
        }
        catch (Exception e)
        {
            throw new TetrisDataException(e.Message);
        }
    }
}