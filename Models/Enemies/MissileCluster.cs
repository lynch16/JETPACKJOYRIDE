using Godot;
using System;
using System.Collections.Generic;

public partial class MissileCluster : Node2D
{
    [Export]
    public PackedScene Missle;

    private int[] NumMissileSize = [1, 2, 3];
    private int MaxMissles = 5;

    public override void _Ready()
    {
        // Spawn X missiles, spanning the height of the viewport
        var screenSize = GetViewportRect().Size;
        var floorHeight = GetNode<CollisionShape2D>("/root/Main/World/Floor/CollisionShape2D").Shape.GetRect().Size.Y;
        var ceilingHeight = GetNode<CollisionShape2D>("/root/Main/World/Ceiling/CollisionShape2D").Shape.GetRect().Size.Y;

        var missileSizeEndex = (int)(GD.Randi() % NumMissileSize.Length);
        var numMissiles = NumMissileSize[missileSizeEndex];

        var spawnDistance = ((screenSize.Y - floorHeight) - ceilingHeight) / MaxMissles;

        var spawnIndex = (int)(GD.Randi() % (MaxMissles - numMissiles));

        for (var i = 0; i < numMissiles; i++)
        {
            MissileEnemy newMissile = Missle.Instantiate() as MissileEnemy;
            newMissile.SetTargetPosition(ceilingHeight + ((i + spawnIndex) * spawnDistance));
            AddChild(newMissile);
        }
    }
}
