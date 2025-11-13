using Godot;
using System;
using GdUnit4;

[TestSuite]
public class UnitTestExample
{
    [TestCase]
    [RequireGodotRuntime]
    public void TestSaveHighScore()
    {
        int highScore = 101;
        SaveManager manager = Assertions.AutoFree(new SaveManager());

        manager.SaveHighScore(highScore, "test");

        Assertions.AssertInt(highScore).Equals(manager.LoadHighScore("test"));
    }

    [After]
    [RequireGodotRuntime]
    public void CleanUp()
    {
        SaveManager manager = Assertions.AutoFree(new SaveManager());
        manager.ClearHighScore("test");
    }
}
