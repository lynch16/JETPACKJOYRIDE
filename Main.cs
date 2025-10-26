using Godot;
using System;

public partial class Main : Node2D
{
    [Signal]
    public delegate void GameOverEventHandler();

    private ScoreManager scoreManager;
    private SpawnManager spawnManager;
    private int currentDifficulty = 0;

    public override void _Ready()
    {
        GetNode<Player>("./World/Player").Connect(Player.SignalName.Hit, Callable.From(OnPlayerHit));
        scoreManager = GetNode<ScoreManager>("./Utilities/ScoreManager");
        spawnManager = GetNode<SpawnManager>("./Utilities/SpawnManager");
    }

    public override void _Process(double delta)
    {
        var currentScore = scoreManager.GetScore();
        if ( currentScore > 200 && currentDifficulty < 1)
        {
            currentDifficulty++;
            spawnManager.RampDifficulty(currentDifficulty);
        } else if ( currentScore > 500 && currentDifficulty < 2)
        {
            currentDifficulty++;
            spawnManager.RampDifficulty(currentDifficulty);
        }
    }

    public void OnPlayerHit()
    {
        EmitSignal(SignalName.GameOver);
        GD.Print("Game Over");
    }
}
