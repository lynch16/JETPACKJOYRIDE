using Godot;
using System;
using System.Collections.Generic;

public partial class LaserCluster : Node2D
{
    [Export]
    public PackedScene LaserGroupScene;

    private int NumLasers = 5;
    private int[] LaserChainSize = [1, 2, 3];

    public override void _Ready()
    {
        // Spawn 5 LaserGroups, spanning the height of the viewport
        var screenSize = GetViewportRect().Size;
        var floorHeight = GetNode<CollisionShape2D>("/root/Main/World/Floor/CollisionShape2D").Shape.GetRect().Size.Y;
        var ceilingHeight = GetNode<CollisionShape2D>("/root/Main/World/Ceiling/CollisionShape2D").Shape.GetRect().Size.Y;

        var spawnDistance = ((screenSize.Y - floorHeight) - ceilingHeight)/ (NumLasers - 1);


        var laserStartIndex = (int)(GD.Randi() % NumLasers);
        var laserChainIndex = (int)(GD.Randi() % LaserChainSize.Length);
        var indexesToEnable = new List<int>();

        for (var i = laserStartIndex; i < laserStartIndex + LaserChainSize[laserChainIndex]; i++)
        {
            indexesToEnable.Add((int)(i % NumLasers));
        }

        for (var i = 0; i < NumLasers; i++)
        {
            LaserGroup newLaser = LaserGroupScene.Instantiate() as LaserGroup;
            newLaser.SetTargetPosition(ceilingHeight + (i * spawnDistance));
            AddChild(newLaser);

            if (!indexesToEnable.Contains(i))
            {
                newLaser.DisableLaser();
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}
