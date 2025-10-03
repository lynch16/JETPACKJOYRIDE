using Godot;
using System;

public partial class Bullet : Area2D
{
    public Vector2 _direction = Vector2.Down;
    private float _launchSpeed = Globals.BaseGameSpeed * 2; // Twice speed of movement
    private bool isDieing = false;

    public override void _Ready()
    {
        // Add some random left/right to the down vector
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = _launchSpeed * _direction * (float)delta;
        Position += velocity;
    }

    public void OnExplosionTimerEnd()
    {
        QueueFree();
    }

    private void OnBodyEntered(Node2D body)
    {
        isDieing = true;
        SetPhysicsProcess(false);
        GetNode<GpuParticles2D>("ExplosionParticles").Emitting = true;
        GetNode<Timer>("BulletLifecycleTimer").Start();
    }
}
