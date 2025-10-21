using Godot;
using System;
using System.Reflection.Metadata;

public partial class MissileEnemy : Area2D
{
    private Player _player;
    private Sprite2D _missileSprite;
    private AnimatedSprite2D _warningSprite;
    private GpuParticles2D _smokeParticles;
    private GpuParticles2D _explosionParticles;

    private Vector2 _direction = Vector2.Left;
    private Vector2 _velcoity = Vector2.Zero;
    private float _launchSpeed = 1000f;
    private float _followPlayerSpeed = 10f;
    private float _screenWarningOffset = 10f;
    private Vector2 _screenSize;

    private bool isLaunched = false;
    private bool isDieing = false;
    private bool isPlayerFollower = false;

    public override void _Ready()
    {
        _screenSize = GetViewportRect().Size;
        _player = GetNode<Player>("/root/Main/World/Player");
        _missileSprite = GetNode<Sprite2D>("Missile");
        _missileSprite.Hide();
        _warningSprite = GetNode<AnimatedSprite2D>("Warning");
        _smokeParticles = GetNode<GpuParticles2D>("SmokeParticles");
        _explosionParticles = GetNode<GpuParticles2D>("Explosion");
        GetNode<Timer>("LaunchTimer").Start();

        Position = new Vector2(_screenSize.X - _screenWarningOffset, 0);

        _warningSprite.Animation = "default";
        _warningSprite.Play();
        _warningSprite.Show();

        GetNode<Main>("/root/Main").GameOver += OnGameOver;
    }

    public void SetPlayerFollower(bool isFollower)
    {
        isPlayerFollower = isFollower;

        if (isPlayerFollower)
        {
            var playerPosition = _player.GetPosition();
            Position = new Vector2(_screenSize.X - _screenWarningOffset, playerPosition.Y);
        }
    }

    public override void _Process(double delta)
    {
        var timer = GetNode<Timer>("LaunchTimer");
        var percentTimeLeft = timer.TimeLeft / timer.WaitTime;
        if (percentTimeLeft <= .2f && _warningSprite.Animation == "default")
        {
            _warningSprite.Animation = "UrgentWarning";
            _warningSprite.Play();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (isDieing) { return; }

        // Before launch, follow player Y position with a slight delay
        if (!isLaunched && isPlayerFollower)
        {
            var playerPosition = _player.GetPosition();
            var targetPosition = new Vector2(
                x: _screenSize.X - _screenWarningOffset,
                y: playerPosition.Y
            );
            Position = Position.Lerp(targetPosition, _followPlayerSpeed * (float)delta);
        }
        else
        {
            Position += _velcoity * (float)delta;
        }

        // Dequeue if has flowed off screne
        if (Position.X < -100)
        {
            QueueFree();
        }
        
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body is Player)
        {
            (body as Player).OnHit();
            SetPhysicsProcess(false);
            Hit();
        }
    }

    public void OnLaunchTimerEnd()
    {
        _warningSprite.Hide();
        _missileSprite.Show();
        Position = new Vector2(_screenSize.X + _screenWarningOffset, Position.Y);

        _velcoity = _launchSpeed * _direction;
        isLaunched = true;
        _smokeParticles.Emitting = true;
    }

    public void OnExplosionTimerEnd()
    {
        isDieing = true;
        QueueFree();
    }

    public void Hit()
    {
        // Explode and dequeue
        if (!isDieing)
        {
            GetNode<Timer>("ExplosionTimer").Start();
        }

        _smokeParticles.Emitting = false;
        _explosionParticles.Emitting = true;
        _missileSprite.Hide();

        isDieing = true;
    }

    // Detonate all missiles on GameOver
    private void OnGameOver()
    {
        Hit();
    }
}
