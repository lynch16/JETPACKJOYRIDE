using Godot;
using Godot.Collections;
using System;

public partial class SpawnManager : Node
{
    [Export]
    private PackedScene[] EnemyScenes { get; set; }
    [Export]
    private PackedScene CoinScene { get; set; }
    private PackedScene _nextSpawn { get; set; }

    private int _coinRatio = 100/20; // Spawn coins 20% of the time

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

        // Generate random number between 0-100. Spawn coin if multiple of 10;
        var random = (GD.Randi() % 100);
        var shouldSpawnCoin = random % _coinRatio == 0;

        if (shouldSpawnCoin)
        {
            _nextSpawn = CoinScene;
        }
        else
        {
            _nextSpawn = EnemyScenes[_newEnemyType];
        }

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
