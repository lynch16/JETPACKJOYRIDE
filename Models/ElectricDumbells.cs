using Godot;
using System;

public partial class ElectricDumbells : Area2D
{

    private int[] _dumbellDistanceOptions = [
        50,
        100,
        200
    ];

    private bool _dumbellEnabled = false;
    private float _screenWarningOffset = 10f;
    private Vector2 _velocity = 200f * Vector2.Left;

    private float maxY = 50f;
    private float minY = 600f;

    private CollisionShape2D _crossBeamCollider;
    private Line2D _crossBeam;

    /**
     * 1. Randonly pick the size of the enemy
     * 2. Based on size, position nodeA and nodeB within bounds
     * 3. Initialize collision to match
     **/
    public override void _Ready()
    {
        var distanceOptIndex = GD.Randi() % _dumbellDistanceOptions.Length;
        var nodeDistance = _dumbellDistanceOptions[distanceOptIndex];

        var nodeA = GetNode<Area2D>("NodeAlpha");
        var verticalSpawn = (float)GD.RandRange(200.0, 600.0 - nodeDistance); // Subtract node distnace from the bottom to account for NodeB
        var midPoint = (verticalSpawn + nodeDistance) / 2;
        GD.Print("verticalSpawn " + verticalSpawn);

        // Spawn off screen
        var screenSize = GetViewportRect().Size;

        // nodeA is bound with parent object so set whole object rooted at nodeA
        Position = new Vector2(screenSize.X - _screenWarningOffset, verticalSpawn);


        var nodeB = GetNode<Area2D>("NodeBeta");
        nodeB.Position = new Vector2(nodeA.Position.X, nodeA.Position.Y + nodeDistance); // Set nodeB off of nodeA


        // Create a capsule collider across the two nodes
        _crossBeamCollider = new CollisionShape2D();
        // Position Collider along the same X axis, centered between the two nodes on the Y axis
        _crossBeamCollider.Position = new Vector2(nodeA.Position.X, nodeDistance/2);

        // Set the size of the collider to be the same size as the Nodes
        var crossBeamColliderShape = new CapsuleShape2D();
        crossBeamColliderShape.Height = nodeDistance;
        _crossBeamCollider.Shape = crossBeamColliderShape;

        AddChild(_crossBeamCollider);

        // Create a line between the two nodes
        _crossBeam = new Line2D();
        // Create the Line2D with points at the ends of the nodes
        _crossBeam.AddPoint(nodeA.Position);
        _crossBeam.AddPoint(nodeB.Position);

        var texture = GD.Load<Texture2D>("res://Assets/Sprites/SizedBall.png");
        _crossBeam.Texture = texture;
        _crossBeam.TextureMode = Line2D.LineTextureMode.Tile;
        _crossBeam.TextureRepeat = Line2D.TextureRepeatEnum.Enabled;
        AddChild(_crossBeam);

        // Rotate the whole node a random amount
        Rotation = (float)GD.RandRange(0, Math.Tau);

        _dumbellEnabled = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += _velocity * (float)delta;

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
        }
    }
}
