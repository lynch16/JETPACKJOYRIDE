using Godot;
using Godot.Collections;
using System;

public partial class SpawnManager : Node
{
    [Export]
    public PackedScene MissleEnemyScene { get; set; }
    [Export]
    public PackedScene ElectricDumbellsScene { get; set; }

    private PackedScene _nextEnemey {  get; set; }
    private PackedScene[] _enemyTypes { get; set; }

    public override void _Ready()
    {
        _enemyTypes = [
            MissleEnemyScene,
            ElectricDumbellsScene
        ];
    }

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
        var _newEnemyType = GD.Randi() % _enemyTypes.Length;
        _nextEnemey = _enemyTypes[_newEnemyType];
        GetNode<Timer>("SpawnTimer").Start();
    }

    public void OnEnemySpawnTimerEnd()
    {
        var enemy = _nextEnemey.Instantiate();
        AddChild(enemy);
        if (enemy.HasMethod("SetPlayerFollower"))
        {
            (enemy as MissileEnemy).SetPlayerFollower(true);
        }

        _nextEnemey = null;
        StartSpawnTimer();
    }
}

// TODO: Ramping difficulty that reduces the timer length as player gets further in level
