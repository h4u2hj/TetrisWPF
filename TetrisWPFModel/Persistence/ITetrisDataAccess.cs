namespace TetrisWPFModel.Persistence;

public interface ITetrisDataAccess
{
    GameGrid Load(string path);
    void Save(string path, GameGrid grid, int score);
    int LoadScore(string fileName);
}