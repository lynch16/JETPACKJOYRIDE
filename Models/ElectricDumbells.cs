using Godot;
using System;

public partial class ElectricDumbells : Area2D
{
    private bool _dumbellEnabled = false;
    private float _screenWarningOffset = 10f;
    private Vector2 _velocity = Globals.BaseGameSpeed * Vector2.Left;

    private CollisionShape2D _crossBeamCollider;
    private Line2D _crossBeam;
    private float _crossBeamWidth = 20f;

    /**
     * 1. Randonly pick the size of the enemy
     * 2. Based on size, position nodeA and nodeB within bounds
     * 3. Initialize collision to match
     **/
    public override void _Ready()
    {
        var screenSize = GetViewportRect().Size;
        var floorHeight = GetNode<CollisionShape2D>("/root/Main/World/Floor/CollisionShape2D").Shape.GetRect().Size.Y;
        var ceilingHeight = GetNode<CollisionShape2D>("/root/Main/World/Ceiling/CollisionShape2D").Shape.GetRect().Size.Y;

        float[] dumbellDistanceOptions = [
            screenSize.Y / 8,
            screenSize.Y / 6,
            screenSize.Y / 4,
         ];

        var distanceOptIndex = GD.Randi() % dumbellDistanceOptions.Length;
        var nodeDistance = dumbellDistanceOptions[distanceOptIndex];

        var nodeA = GetNode<Area2D>("NodeAlpha");
        var verticalSpawn = (float)GD.RandRange(ceilingHeight, screenSize.Y - floorHeight - nodeDistance); // Subtract node distnace from the bottom to account for NodeB
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
        _crossBeam.Width = _crossBeamWidth;
        // Create the Line2D with points at the ends of the nodes
        _crossBeam.AddPoint(nodeA.Position);
        _crossBeam.AddPoint(nodeB.Position);

        var texture = GD.Load<Texture2D>("res://Assets/Sprites/ElectricCharge.png");
        _crossBeam.Texture = texture;
        _crossBeam.TextureMode = Line2D.LineTextureMode.Stretch;
        AddChild(_crossBeam);

        var lineParticles = GetNode<GpuParticles2D>("LineParticles");
        lineParticles.Position = new Vector2(0, (nodeB.Position.Y - nodeA.Position.Y)/2);
        // Have the line not quite as wide as the rest in order to excentuate the dumbell shape
        (lineParticles.ProcessMaterial as ParticleProcessMaterial).EmissionBoxExtents = new Vector3(_crossBeamWidth * 0.75f, nodeDistance/2, 1);
        lineParticles.Show();

        // Rotate the whole node a random amount
        Rotation = (float)GD.RandRange(0, Math.Tau/8);

        _dumbellEnabled = true;

        GetNode<Hud>("/root/Main/HUD").Start += OnStart;
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
            SetPhysicsProcess(false);
        }
    }

    public void OnStart()
    {
        SetPhysicsProcess(true);
    }
}
