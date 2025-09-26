using Godot;
using System;

public partial class Bullet : CharacterBody2D
{
    private Vector2 _direction = Vector2.Down;
    private float _launchSpeed = 5f;
    private bool isDieing = false;

    public override void _Ready()
    {
        // Add some random left/right to the down vector
    }

    public override void _PhysicsProcess(double delta)
    {
        var collision = MoveAndCollide(Velocity * (float)delta);

        while (collision != null)
        {
            var collider = collision.GetCollider();
            Hit();
        }

        Velocity = _launchSpeed * _direction * (float)delta;
    }

    public void OnExplosionTimerEnd()
    {
        // TODO: Stop explosion animation
        QueueFree();
    }

    public void Hit()
    {
        isDieing = true;
        GetNode<Timer>("ExplosionTimer").Start();
        // TODO:  Start explosion animation
    }
}
