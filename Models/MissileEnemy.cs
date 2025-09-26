using Godot;
using System;

public partial class MissileEnemy : CharacterBody2D
{
    private Player _player;
    private Sprite2D _missileSprite;
    private Sprite2D _warningSprite;

    private Vector2 _direction = Vector2.Left;
    private float _launchSpeed = 10f;
    private float _followPlayerSpeed = 1f;
    private Vector2 _screenSize;

    private bool isLaunched = false;
    private bool isDieing = false;

    public override void _Ready()
    {
        _screenSize = GetViewportRect().Size;
        _player = GetNode<Player>("/Main/World/Player");
        _missileSprite = GetNode<Sprite2D>("Missile");
        _missileSprite.Hide();
        _warningSprite = GetNode<Sprite2D>("Warning");
        GetNode<Timer>("LaunchTimer").Start();

        _warningSprite.Show();
        // TODO:  Start warning sprite animation (flashing)
    }

    public override void _PhysicsProcess(double delta)
    {
        if (isDieing) { return; }

        // Before launch, follow player Y position with a slight delay
        if (!isLaunched)
        {
            var playerPosition = _player.GetPosition();
            var targetPosition = new Vector2(
                x: _screenSize.X - 10,
                y: playerPosition.Y
            );
            Position = Position.Lerp(targetPosition, _followPlayerSpeed * (float)delta);

        } else
        {
            var collision = MoveAndCollide(Velocity * (float)delta);

            while (collision != null)
            {
                var collider = collision.GetCollider();

                // Only explode when hitting player; move through everything else
                if (collider is Player)
                {
                    (collider as Player).OnHit();
                    Hit();
                    break;
                } else
                {
                    var remainder = collision.GetRemainder();
                    collision = MoveAndCollide(remainder);
                }
            }
        }
        
    }

    public void OnLaunchTimerEnd()
    {
        _warningSprite.Hide();
        _missileSprite.Show();
        Velocity = _launchSpeed * _direction;
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
