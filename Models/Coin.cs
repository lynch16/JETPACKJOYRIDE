using Godot;
using System;

public partial class Coin : Area2D
{
    public override void _Ready()
    {
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Dequeue if has flowed off screne
        if (GlobalPosition.X < -100)
        {
            QueueFree();
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Player)
        {
            GD.Print("COIN!");
            GetNode<ScoreManager>("/root/Main/Utilities/ScoreManager").OnCoinHit();
            QueueFree();
        }
    }
}
