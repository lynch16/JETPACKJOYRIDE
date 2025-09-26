using Godot;
using System;

public partial class MissileEnemy : Area2D
{
    private Player _player;
    private Sprite2D _missileSprite;
    private Sprite2D _warningSprite;

    private Vector2 _direction = Vector2.Left;
    private Vector2 _velcoity = Vector2.Zero;
    private float _launchSpeed = 2000f;
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
        _warningSprite = GetNode<Sprite2D>("Warning");
        GetNode<Timer>("LaunchTimer").Start();

        Position = new Vector2(_screenSize.X - _screenWarningOffset, 0);

        _warningSprite.Show();
        // TODO:  Start warning sprite animation (flashing)
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
        
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body is Player)
        {
            (body as Player).OnHit();
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
        // TODO:  Start missile animation
    }

    public void OnExplosionTimerEnd()
    {
        // TODO: Stop explosion animation
        QueueFree();
    }

    public void Hit()
    {
        // Explode and dequeue
        isDieing = true;
        GetNode<Timer>("ExplosionTimer").Start();
        // TODO:  Start explosion animation
    }
}
