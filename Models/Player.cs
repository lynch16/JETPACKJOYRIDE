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
    private float _fallSpeed = 12000f;

    public Vector2 ScreenSize;

    private Vector2[] directions = [Vector2.Left, Vector2.Down, Vector2.Right];

    private bool isRunning;
    private bool _bulletSpawned = false;
    private int _bulletSpawnDirection = 1;
    private int _bulletSpread = 8;

    private Node2D _bulletSpawnPoint;
    private AnimatedSprite2D _sprite;
    private Sprite2D _muzzleFlash;

    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        _bulletSpawnPoint = GetNode<Node2D>("BulletSpawnPoint");
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _muzzleFlash = GetNode<Sprite2D>("MuzzleFlash");
        //SetPhysicsProcess(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("move_up"))
        {
            isRunning = false;
            _sprite.Animation = "flying";
        }

        if (Input.IsActionPressed("move_up"))
        {
            Velocity = Vector2.Up * _flightSpeed * (float)delta;

            if (!_bulletSpawned)
            {
                Bullet bullet = BulletScene.Instantiate<Bullet>();
                bullet.Position = _bulletSpawnPoint.Position;
                _bulletSpawnDirection += 1;
                if (_bulletSpawnDirection > 2)
                {
                    _bulletSpawnDirection = 0;
                }

                bullet._direction = (Vector2.Down + directions[_bulletSpawnDirection]/ _bulletSpread).Normalized();
                AddChild(bullet);

                _bulletSpawned = true;
                GetNode<Timer>("BulletSpawnTimer").Start();

                _muzzleFlash.Show();
                GetNode<Timer>("MuzzleFlash/MuzzleFlashTimer").Start();
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
                _sprite.Animation = "running";
                isRunning = true;
            }
        }
    }

    public void OnBulletTimerSpawnEnd()
    {
        _bulletSpawned = false;
    }

    public void OnMuzzleFlashTimerEnd()
    {
        _muzzleFlash.Hide();
    }

    public void OnStart()
    {
        SetPhysicsProcess(true);
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
