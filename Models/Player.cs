using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Signal]
    public delegate void HitEventHandler();

    [Export]
    public PackedScene BulletScene { get; set; }

    [Export]
    private float FlightSpeed = 8000f;
    [Export]
    private float FallSpeed = 12000f;

    public Vector2 ScreenSize;

    private Vector2[] directions = [Vector2.Left, Vector2.Down, Vector2.Right];

    private bool isRunning;
    private bool _bulletSpawned = false;
    private int _bulletSpawnDirection = 1;
    private int _bulletSpread = 8;
    private bool _hasDied = false;
    private bool _gameStarted = false;

    private Node2D _bulletSpawnPoint;
    private AnimatedSprite2D _sprite;
    private Sprite2D _muzzleFlash;
    private GpuParticles2D _deathParticles;

    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        _bulletSpawnPoint = GetNode<Node2D>("BulletSpawnPoint");
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _muzzleFlash = GetNode<Sprite2D>("MuzzleFlash");
        _deathParticles = GetNode<GpuParticles2D>("DeathAnimation");

        GetNode<Hud>("/root/Main/HUD").Connect(Hud.SignalName.Start, Callable.From(
          OnStart));
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_gameStarted) {  return; }

        if (Input.IsActionJustPressed("move_up"))
        {
            isRunning = false;
            _sprite.Animation = "flying";
        }

        if (Input.IsActionPressed("move_up"))
        {
            Velocity = Vector2.Up * FlightSpeed * (float)delta;

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
            Velocity = Vector2.Down * FallSpeed * (float)delta;
        }

        MoveAndSlide();

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

    private void StartCharacter()
    {
        SetPhysicsProcess(true);
        _sprite.Show();
    }

    // TODO: This is clunky b/c env starts moving before the player has started running
    public void OnStart()
    {
        _gameStarted = true;
        if (_hasDied)
        {
            OnRespawn();
        } else
        {
            StartCharacter();
        }
    }

    public void OnHit()
    {
        EmitSignal(SignalName.Hit);
        GD.Print("Player Hit");
        _deathParticles.Emitting = true;
        _sprite.Hide();
        // Must be deferred as we can't change physics properties on a physics callback.
        SetPhysicsProcess(false);
        _hasDied = true;
    }

    public void OnRespawn()
    {
        GetNode<GpuParticles2D>("RespawnAnimation").Emitting = true;
        GetNode<Timer>("RespawnAnimation/Timer").Start();
    }
}
