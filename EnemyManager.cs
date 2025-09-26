using Godot;
using Godot.Collections;
using System;

public partial class EnemyManager : Node
{
    [Export]
    public PackedScene MissileEnemyScene { get; set; }

    private PackedScene _nextEnemey {  get; set; }
    private PackedScene[] _enemyTypes { get; set; }

    public override void _Ready()
    {
        _enemyTypes = [
            MissileEnemyScene
        ];
    }

    public override void _Process(double delta)
    {
        if (_nextEnemey == null)
        {
            var _newEnemyType = GD.Randi() % _enemyTypes.Length;
            _nextEnemey = _enemyTypes[_newEnemyType];
            GetNode<Timer>("EnemySpawnTimer").Start();
        }
    }

    public void OnEnemySpawnTimerEnd()
    {
        var enemy = _nextEnemey.Instantiate();
        AddChild(enemy);

        _nextEnemey = null;
    }
}

// TODO: Ramping difficulty that reduces the timer length as player gets further in level
