using Godot;
using System;
using System.Reflection.Metadata;

public partial class LaserGroup : Area2D
{
    private bool _laserEnabled = false;
    private float _screenWarningOffset = 40f;
    private Vector2 _velocity = Globals.BaseGameSpeed * Vector2.Left;

    private CollisionShape2D _crossBeamCollider;
    private Line2D _crossBeam;
    private float _crossBeamWidth = 5f;

    private Player _player;
    private const float FollowSpeed = 3.0f;
    private float? _targetYPosition = null;

    /**
 * 2. Based on size, position nodeA and nodeB within bounds
 * 3. Initialize collision to match
 **/
    public override void _Ready()
    {
        _player = GetNode<Player>("/root/Main/World/Player");
        var screenSize = GetViewportRect().Size;
        var floorHeight = GetNode<CollisionShape2D>("/root/Main/World/Floor/CollisionShape2D").Shape.GetRect().Size.Y;
        var ceilingHeight = GetNode<CollisionShape2D>("/root/Main/World/Ceiling/CollisionShape2D").Shape.GetRect().Size.Y;

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
        _crossBeamCollider.Position = new Vector2(nodeDistance / 2, getYPos());

        // Set the size of the collider to be the same size as the Nodes
        var crossBeamColliderShape = new RectangleShape2D();
        crossBeamColliderShape.Size = new Vector2(nodeDistance, _crossBeamWidth);
        _crossBeamCollider.Shape = crossBeamColliderShape;
        _crossBeamCollider.Disabled = true;
        AddChild(_crossBeamCollider);

        // Create a line between the two nodes
        _crossBeam = new Line2D();
        _crossBeam.Width = _crossBeamWidth;
        // Create the Line2D with points at the ends of the nodes
        _crossBeam.AddPoint(new Vector2(nodeA.Position.X, nodeA.Position.Y + getYPos()));
        _crossBeam.AddPoint(new Vector2(nodeB.Position.X, nodeB.Position.Y + getYPos()));

        var texture = GD.Load<Texture2D>("res://Assets/Sprites/LaserBeam.png");
        _crossBeam.Texture = texture;
        _crossBeam.TextureMode = Line2D.LineTextureMode.Stretch;
        AddChild(_crossBeam);
        _crossBeam.Hide();
    }

    private float getYPos()
    {
        var nodeA = GetNode<Area2D>("LaserModuleA");
        return nodeA.GetNode<CollisionShape2D>("CollisionShape2D").Shape.GetRect().Size.Y / 8;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_laserEnabled)
        {
            Position = Position.Lerp(new Vector2(_player.Position.X - _screenWarningOffset, _targetYPosition ?? _player.Position.Y), (float)delta * FollowSpeed);
        }

        // Dequeue if has flowed off screne
        if (Position.X < -1000)
        {
            QueueFree();
        }
    }

    public void setTargetPosition(float? targetPosition)
    {
        _targetYPosition = targetPosition;  
    }

    public void EnableLaser()
    {
        _laserEnabled = true;
        _crossBeam.Show();
        _crossBeamCollider.Disabled = false;
        // TODO: Show laser particles
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Player)
        {
            (body as Player).OnHit();
            _laserEnabled = false;
        }
    }
}
