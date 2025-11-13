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
            GetNode<ScoreManager>("/root/Main/Utilities/ScoreManager").OnCoinHit();
            GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D").Play();
            GetNode<Timer>("Timer").Start();
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Hide();
        }
    }

    private void OnTimer()
    {
        QueueFree();
    }
}
