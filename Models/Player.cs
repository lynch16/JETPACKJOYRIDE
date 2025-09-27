using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Signal]
    public delegate void HitEventHandler();

    [Export]
    public PackedScene BulletScene { get; set; }

    [Export]
    private float _flightSpeed = 8000f;
    [Export]
    private float _fallSpeed = 10000f;

    public Vector2 ScreenSize;

    private bool isRunning;
    private bool _bulletSpawned = false;

    private Node2D _bulletSpawnPoint;

    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        _bulletSpawnPoint = GetNode<Node2D>("BulletSpawnPoint");
        //SetPhysicsProcess(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("move_up"))
        {
            isRunning = false;
            GD.Print("Running animation stop");
        }

        if (Input.IsActionPressed("move_up"))
        {
            Velocity = Vector2.Up * _flightSpeed * (float)delta;

            if (!_bulletSpawned)
            {
                Bullet bullet = BulletScene.Instantiate<Bullet>();
                bullet.Position = _bulletSpawnPoint.Position;
                AddChild(bullet);

                _bulletSpawned = true;
                GetNode<Timer>("BulletSpawnTimer").Start();
            }
        } else
        {
            Velocity = Vector2.Down * _fallSpeed * (float)delta;
        }

        MoveAndSlide();

        // TODO: If on ground, running animation
        if (IsOnFloor())
        {
            if (!isRunning)
            {
                GD.Print("Running animation start");
                isRunning = true;
            }
        }
    }

    public void OnBulletTimerSpawnEnd()
    {
        _bulletSpawned = false;
    }

    public void Start(Vector2 position)
    {
        SetPhysicsProcess(true);
        Position = position;
    }

    public void OnHit()
    {
        // TODO: Trigger death animation
        EmitSignal(SignalName.Hit);
        GD.Print("Player Hit");
        // Must be deferred as we can't change physics properties on a physics callback.
        //SetPhysicsProcess(false);
    }
}
