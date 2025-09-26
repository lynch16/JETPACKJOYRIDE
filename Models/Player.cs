using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Signal]
    public delegate void HitEventHandler();

    public Vector2 ScreenSize;

    private bool isRunning;

    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
        SetProcess(false);
    }

    public override void _Process(double delta)
    {
        // TODO: If on ground, running animation
        if (IsOnFloor())
        {
            if (!isRunning)
            {
                GD.Print("Running animation start");
                isRunning = true;
            }
        } else
        {
            GD.Print("Running animation stop");
        }
    }
    public void Start(Vector2 position)
    {
        SetProcess(true);
        Position = position;
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    public void Hit()
    {
        // TODO: Trigger death animation
        EmitSignal(SignalName.Hit);
        // Must be deferred as we can't change physics properties on a physics callback.
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        SetProcess(false);
    }
}
