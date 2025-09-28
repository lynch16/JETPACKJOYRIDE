using Godot;
using System;

public partial class Main : Node2D
{
    [Signal]
    public delegate void GameOverEventHandler();

    public void OnPlayerHit()
    {
        EmitSignal(SignalName.GameOver);
        GD.Print("Game Over");
    }
}
