using Godot;
using System;

public partial class SaveManager : Node
{
    private string getSaveGameString(string fileName)
    {
        return "user://" + fileName + ".data";
    }

    public void SaveHighScore(int highScore, string fileName = "savegame")
    {
        var saveFile = FileAccess.Open(getSaveGameString(fileName), FileAccess.ModeFlags.Write);
        saveFile.StoreVar(highScore);
        saveFile.Close();
    }

    public int LoadHighScore(string fileName = "savegame")
    {
        var saveFile = FileAccess.Open(getSaveGameString(fileName), FileAccess.ModeFlags.Read);
        if (saveFile != null)
        {
            var highScore = saveFile.GetVar();
            saveFile.Close();
            return (int)highScore;
        }

        return 0;
    }

    public void ClearHighScore(string filename)
    {
        DirAccess.RemoveAbsolute(getSaveGameString(filename));

    }
}
