using Godot;
using Godot.Collections;
using System;

public partial class SpawnManager : Node
{
    [Export]
    private PackedScene[] EnemyScenes { get; set; }
    private PackedScene _nextSpawn { get; set; }

    public void OnStart()
    {
        StartSpawnTimer();
    }

    public void OnGameOver()
    {
        GetNode<Timer>("SpawnTimer").Stop(); // Halt additional spawns
    }

    private void StartSpawnTimer()
    {
        var _newEnemyType = GD.Randi() % EnemyScenes.Length;
        _nextSpawn = EnemyScenes[_newEnemyType];
        GetNode<Timer>("SpawnTimer").Start();
    }

    public void OnEnemySpawnTimerEnd()
    {
        var enemy = _nextSpawn.Instantiate();
        AddChild(enemy);
        if (enemy.HasMethod("SetPlayerFollower"))
        {
            (enemy as MissileEnemy).SetPlayerFollower(true);
        }

        _nextSpawn = null;
        StartSpawnTimer();
    }
}

// TODO: Ramping difficulty that reduces the timer length as player gets further in level
