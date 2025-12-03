using Godot;
using System;
using System.Reflection.Metadata;

public partial class LaserGroup : Area2D
{
    private bool _laserEnabled = true;
    private bool _laserOn = false;
    private bool _hasFired = false;
    private float _screenWarningOffset; // Based on player position so that player is centered in laser beams
    private Vector2 _velocity = Globals.BaseGameSpeed * Vector2.Left;

    private CollisionShape2D _crossBeamCollider;
    private Line2D _crossBeam;
    private float _crossBeamWidth = 5f;

    private Player _player;
    private const float FollowSpeed = 3.0f;
    private float? _targetYPosition = null;

    // Same material is used for both effects
    private ParticleProcessMaterial _laserParticleMaterial;

    /**
 * 2. Based on size, position nodeA and nodeB within bounds
 * 3. Initialize collision to match
 **/
    public override void _Ready()
    {
        _laserParticleMaterial = GetNode<GpuParticles2D>("LaserModuleA/Sprite2D/GPUParticles2D").ProcessMaterial as ParticleProcessMaterial;
        _player = GetNode<Player>("/root/Main/World/Player");
        var screenSize = GetViewportRect().Size;
        var floorHeight = GetNode<CollisionShape2D>("/root/Main/World/Floor/CollisionShape2D").Shape.GetRect().Size.Y;
        var ceilingHeight = GetNode<CollisionShape2D>("/root/Main/World/Ceiling/CollisionShape2D").Shape.GetRect().Size.Y;
        _screenWarningOffset = _player.GlobalPosition.X / 2;

        var nodeDistance = screenSize.X - (_screenWarningOffset * 2);

        var nodeA = GetNode<Area2D>("LaserModuleA");
        var verticalSpawn = (float)GD.RandRange(ceilingHeight, screenSize.Y - floorHeight); // Subtract node distnace from the bottom to account for NodeB
        // nodeA is bound with parent object so set whole object rooted at nodeA
        Position = new Vector2(screenSize.X - _screenWarningOffset, verticalSpawn);

        var nodeB = GetNode<Area2D>("LaserModuleB");
        nodeB.Position = new Vector2(nodeA.Position.X + nodeDistance, nodeA.Position.Y); // Set nodeB off of nodeA

        // Create a capsule collider across the two nodes
        _crossBeamCollider = new CollisionShape2D();
        // Position Collider along the same X axis, centered between the two nodes on the Y axis
        _crossBeamCollider.Position = new Vector2(nodeDistance / 2, nodeA.Position.Y);

        // Set the size of the collider to be the same size as the Nodes
        var crossBeamColliderShape = new RectangleShape2D();
        crossBeamColliderShape.Size = new Vector2(nodeDistance, _crossBeamWidth);
        _crossBeamCollider.Shape = crossBeamColliderShape;
        AddChild(_crossBeamCollider);

        // Create a line between the two nodes
        _crossBeam = new Line2D();
        _crossBeam.Width = _crossBeamWidth;
        // Create the Line2D with points at the ends of the nodes
        _crossBeam.AddPoint(new Vector2(nodeA.Position.X, nodeA.Position.Y));
        _crossBeam.AddPoint(new Vector2(nodeB.Position.X, nodeB.Position.Y));

        var texture = GD.Load<Texture2D>("res://Assets/Sprites/LaserBeam.png");
        _crossBeam.Texture = texture;
        _crossBeam.TextureMode = Line2D.LineTextureMode.Stretch;
        AddChild(_crossBeam);

        GetNode<AnimationPlayer>("LaserModuleA/AnimationPlayer").Play("default");
        GetNode<AnimationPlayer>("LaserModuleB/AnimationPlayer").Play("default");

        TurnOffLaser();
        GetNode<AudioStreamPlayer2D>("LaserStartTimer/LaserCharge").Play();
    }

    public override void _PhysicsProcess(double delta)
    {
        var laserStartTimer = GetNode<Timer>("LaserStartTimer");
        var timeLeft = (laserStartTimer.WaitTime - laserStartTimer.TimeLeft) / laserStartTimer.WaitTime;

        // Show laser particles that get bigger the closer to final trigger to simulate the sharge up
        if (timeLeft > 0.5)
        {
            GetNode<GpuParticles2D>("LaserModuleA/Sprite2D/GPUParticles2D").Emitting = true;
            GetNode<GpuParticles2D>("LaserModuleB/Sprite2D/GPUParticles2D").Emitting = true;
            _laserParticleMaterial.RadialVelocityMax = (float)timeLeft * 50f;
        }

        if (!_laserOn)
        {
            if (_hasFired)
            {
                Position += _velocity * 2 * (float)delta;
            }
            else
            {
                Position = Position.Lerp(new Vector2(_player.Position.X - _screenWarningOffset, _targetYPosition ?? _player.Position.Y), (float)delta * FollowSpeed);
            }
        }

        // Dequeue if has flowed off screne
        if (Position.X < -1000)
        {
            QueueFree();
        }
    }

    public void SetTargetPosition(float? targetPosition)
    {
        _targetYPosition = targetPosition;  
    }

    public void DisableLaser()
    {
        TurnOffLaser();
        // Change to different color
        _laserEnabled = false;
        GetNode<Node2D>("LaserModuleA/Sprite2D").SelfModulate = Colors.SlateGray;
        GetNode<AnimationPlayer>("LaserModuleA/AnimationPlayer").Stop();
        GetNode<Node2D>("LaserModuleB/Sprite2D").SelfModulate = Colors.SlateGray;
        GetNode<AnimationPlayer>("LaserModuleB/AnimationPlayer").Stop();
        GetNode<GpuParticles2D>("LaserModuleA/Sprite2D/GPUParticles2D").Hide();
        GetNode<GpuParticles2D>("LaserModuleB/Sprite2D/GPUParticles2D").Hide();
    }

    public void TurnOffLaser()
    {
        _laserOn = false;
        _crossBeam.Hide();
        _crossBeamCollider.SetDeferred("disabled", true); // Cant change physics mid frame
        GetNode<AudioStreamPlayer2D>("LaserStartTimer/LaserCharge").Stop();
        GetNode<AudioStreamPlayer2D>("LaserRunTimer/LaserSound").Stop();
        if (_hasFired)
        {
            GetNode<GpuParticles2D>("LaserModuleA/Sprite2D/GPUParticles2D").Hide();
            GetNode<GpuParticles2D>("LaserModuleB/Sprite2D/GPUParticles2D").Hide();
        } else
        {
            GetNode<GpuParticles2D>("LaserModuleA/Sprite2D/GPUParticles2D").Emitting = false;
            GetNode<GpuParticles2D>("LaserModuleB/Sprite2D/GPUParticles2D").Emitting = false;
        }
    }

    public void StartLaser()
    {
        // Prevent startup if laser disabled
        if (_laserEnabled)
        {
            _laserOn = true;
            _crossBeam.Show();
            _crossBeamCollider.Disabled = false;
        }
        
        GetNode<Timer>("LaserRunTimer").Start();
        GetNode<AudioStreamPlayer2D>("LaserRunTimer/LaserSound").Play();
        GetNode<AudioStreamPlayer2D>("LaserStartTimer/LaserCharge").Stop();
    }

    private void OnLaserRunTimerTimeout()
    {
        _hasFired = true;
        TurnOffLaser();
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Player)
        {
            (body as Player).OnHit();
            TurnOffLaser();
        }
    }
}
