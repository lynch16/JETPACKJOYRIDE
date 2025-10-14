using Godot;
using System;

public partial class Main : Node2D
{
    [Signal]
    public delegate void GameOverEventHandler();

    public override void _Ready()
    {
        GetNode<Player>("./World/Player").Connect(Player.SignalName.Hit, Callable.From(OnPlayerHit));
    }

    public void OnPlayerHit()
    {
        EmitSignal(SignalName.GameOver);
        GD.Print("Game Over");
    }
}
