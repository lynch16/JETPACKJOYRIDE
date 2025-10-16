using Godot;
using System;
using GdUnit4;

[TestSuite]
public class UnitTestExample
{
    [TestCase]
    public void TestSaveHighScore()
    {
        int highScore = 101;
        SaveManager manager = new SaveManager();

        manager.SaveHighScore(highScore, "test");

        Assertions.AssertInt(highScore).Equals(manager.LoadHighScore("test"));
    }

    [After]
    public void CleanUp()
    {
        SaveManager manager = new SaveManager();
        manager.ClearHighScore("test");
    }
}
