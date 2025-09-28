using Godot;
using System;

public partial class SaveManager : Node
{
    private string saveGameLocation = "user://savegame.data";

    public void SaveHighScore(int highScore)
    {
        var saveFile = FileAccess.Open(saveGameLocation, FileAccess.ModeFlags.Write);
        saveFile.StoreVar(highScore);
        saveFile.Close();
    }

    public int LoadHighScore()
    {
        var saveFile = FileAccess.Open(saveGameLocation, FileAccess.ModeFlags.Read);
        if (saveFile != null)
        {
            var highScore = saveFile.GetVar();
            saveFile.Close();
            return (int)highScore;
        }

        return 0;

    }
}
