using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Signal]
    public delegate void HitEventHandler();

    [Export]
    public PackedScene BulletScene { get; set; }

    public Vector2 ScreenSize;

    private bool isRunning;
    private float _fallSpeed = 5f;
    private float _flightSpeed = 5f;

    private Node2D _bulletSpawnPoint;

    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        _bulletSpawnPoint = GetNode<Node2D>("BulletSpawnPoint");
        //GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
        //SetProcess(false);
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
        }

        if (Input.IsActionJustPressed("move_up"))
        {
            isRunning = false;
            GD.Print("Running animation stop");
        }

        if (Input.IsActionPressed("move_up"))
        {
            Velocity += Vector2.Up * _flightSpeed * (float)delta;

            Bullet bullet = BulletScene.Instantiate<Bullet>();
            bullet.Position = _bulletSpawnPoint.Position;
            AddChild(bullet);

        } else if (!IsOnFloor())
        {
            Velocity += Vector2.Down * _fallSpeed * (float)delta;
        }
    }
    public void Start(Vector2 position)
    {
        SetProcess(true);
        Position = position;
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    public void OnHit()
    {
        // TODO: Trigger death animation
        EmitSignal(SignalName.Hit);
        // Must be deferred as we can't change physics properties on a physics callback.
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        SetProcess(false);
    }
}
