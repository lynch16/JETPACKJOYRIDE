using Godot;
using Godot.Collections;
using System;
using KaimiraGames;
using System.Collections.Generic;


public partial class SpawnManager : Node
{
    private WeightedList<(PackedScene scene, int timeout)> EnemyScenes = new();

    [Export]
    private PackedScene CoinScene { get; set; }
    private PackedScene _nextSpawn { get; set; }

    private int _coinRatio = 100/20; // Spawn coins 20% of the time

    // Scene, Spawn Timer, Weight
    private WeightedListItem<(PackedScene scene, int timeout)> ElectricDumbells = new(
        (GD.Load<PackedScene>("res://Models/Enemies/ElectricDumbells.tscn"), 5), 2
    );
    private readonly WeightedListItem<(PackedScene scene, int timeout)> MissileEnemy = new(
        (GD.Load<PackedScene>("res://Models/Enemies/MissleEnemy.tscn"), 5), 3
    );
    private WeightedListItem<(PackedScene scene, int timeout)> MissileCluster = new(
        (GD.Load<PackedScene>("res://Models/Enemies/MissileCluster.tscn"), 5), 2
    );
    private WeightedListItem<(PackedScene scene, int timeout)> LaserEnemy = new(
        (GD.Load<PackedScene>("res://Models/Enemies/laser_group.tscn"), 5), 3
    );
    private WeightedListItem<(PackedScene scene, int timeout)> LaserCluster = new(
        (GD.Load<PackedScene>("res://Models/Enemies/LaserCluster.tscn"), 5), 2
    );
    private WeightedListItem<(PackedScene scene, int timeout)> CoinManager = new(
        (GD.Load<PackedScene>("res://Models/Rewards/CoinManager.tscn"), 5), 2
    );
    public override void _Ready()
    {
        var enemyItems = new List<WeightedListItem<(PackedScene scene, int timer)>>()
        {
            ElectricDumbells,
            MissileCluster,
            LaserCluster

        };
        EnemyScenes = new(enemyItems);

        GetNode<Hud>("/root/Main/HUD").Connect(Hud.SignalName.Start, Callable.From(
            OnStart));
        GetNode<Main>("/root/Main").Connect(Main.SignalName.GameOver, Callable.From(
            OnGameOver));
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
        var _newEnemyType = (int)(GD.Randi() % EnemyScenes.Count);

        _nextSpawn = EnemyScenes[_newEnemyType].scene;
        GetNode<Timer>("SpawnTimer").Start(EnemyScenes[_newEnemyType].timeout);
    }

    public void OnEnemySpawnTimerEnd()
    {
        var enemy = _nextSpawn.Instantiate();
        AddChild(enemy);
        _nextSpawn = null;
        StartSpawnTimer();
    }

    public void RampDifficulty(int difficulty)
    {
        if (difficulty == 1)
        {
            var newItems = new List<WeightedListItem<(PackedScene scene, int timeout)>>()
            {
                CoinManager,
                MissileEnemy,
            };
            EnemyScenes.Add(newItems);
        }
        else if (difficulty == 2)
        {
            var newItems = new List<WeightedListItem<(PackedScene scene, int timeout)>>()
            {
                LaserEnemy,
            };
            EnemyScenes.Add(newItems);
        }
    }
}
