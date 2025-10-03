using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

public partial class CoinManager : Node2D
{
    [Export]
    public PackedScene CoinScene { get; set; }
    private float _screenWarningOffset = 10f;
    private Vector2 _velocity = Globals.BaseGameSpeed * Vector2.Left;
    private float _coinDistanceRatio = 16f;

    private float _floorHeight;
    private float _ceilingHeight;
    private Vector2 _screenSize;

    /**
     * 1. Decide what shape of coin to create
     * 2. Position coins in corresponding shape
     * 3. Position shape in world
     * 4. Enable velocity
     **/
    public override void _Ready()
    {
        _screenSize = GetViewportRect().Size;
        _floorHeight = GetNode<CollisionShape2D>("/root/Main/World/Floor/CollisionShape2D").Shape.GetRect().Size.Y;
        _ceilingHeight = GetNode<CollisionShape2D>("/root/Main/World/Ceiling/CollisionShape2D").Shape.GetRect().Size.Y;

        // Pick a style
        var coinOrientation = GD.Randi() % 3;

        // Initialize collection
        List<Vector2> coinVectors = new List<Vector2>();

        // 0 == Big Block
        // 1 == Tiered Blocks
        // 2 == Circles
        // Generate all sections
        if (coinOrientation == 0)
        {
            SetStartPosition(_coinDistanceRatio * 5);
            BuildDiamond(coinVectors);
        }
        else if (coinOrientation == 1)
        {
            SetStartPosition(_coinDistanceRatio * 9);
            BuildTripleCoinBlock(coinVectors);
        }
        else
        {
            SetStartPosition(_coinDistanceRatio * 8);
            BuildTripleCircle(coinVectors);
        }

        // Instantiate coins
        foreach (var coinVector in coinVectors)
        {
            var coinA = CoinScene.Instantiate<Area2D>();
            coinA.Position = coinVector;
            AddChild(coinA);
        }

        var debugCoin = CoinScene.Instantiate<Area2D>();
        debugCoin.Position = Position;
        debugCoin.Modulate = new Color(0, 0, 1);
        AddChild(debugCoin);
    }

    private void SetStartPosition(float coinShapeHeight)
    {
        var verticalSpawn = (float)GD.RandRange(_ceilingHeight, (_screenSize.Y - _floorHeight - coinShapeHeight) / 2);
        Position = new Vector2(_screenSize.X + _screenWarningOffset, verticalSpawn);
    }

    private void BuildTripleCircle(List<Vector2> coinPositions)
    {
        var positionWithOffset = Position + new Vector2(0, _coinDistanceRatio);
        BuildCircle(coinPositions, positionWithOffset);
        positionWithOffset = Position + new Vector2(_coinDistanceRatio * 8, _coinDistanceRatio * 3);
        BuildCircle(coinPositions, positionWithOffset);
        positionWithOffset = Position + new Vector2(_coinDistanceRatio * 8 * 2, _coinDistanceRatio);
        BuildCircle(coinPositions, positionWithOffset);
    }

    private void BuildCircle(List<Vector2> coinPositions, Vector2 startPosition)
    {
        (int, int)[] coinTuples = [
            (7, 0),
            (6, -1),
            (5, -2),
            (4, -2),
            (3, -1),
            (2, 0),
            (2, 1),
            (3, 2),
            (4, 3),
            (5, 3),
            (6, 2),
            (7, 1),
        ];

        foreach (var coinTuple in coinTuples)
        {
            coinPositions.Add(
                new Vector2(startPosition.X + (_coinDistanceRatio * coinTuple.Item1), startPosition.Y + (_coinDistanceRatio * coinTuple.Item2))
            );
        }
    }

    private void BuildTripleCoinBlock(List<Vector2> coinPositions)
    {
        var positionWithOffset = Position + new Vector2(0, _coinDistanceRatio);
        BuildCoinBlock(coinPositions, positionWithOffset);
        positionWithOffset = Position + new Vector2(_coinDistanceRatio * 8, _coinDistanceRatio * 4);
        BuildCoinBlock(coinPositions, positionWithOffset);
        positionWithOffset = Position + new Vector2(_coinDistanceRatio * 8 * 2, _coinDistanceRatio * 7);
        BuildCoinBlock(coinPositions, positionWithOffset);
    }

    private void BuildCoinBlock(List<Vector2> coinPositions, Vector2 startPosition)
    {
        (int, int)[] coinTuples = [
            (7, 0),
            (6, 0),
            (5, 0),
            (4, 0),
            (3, 0),
            (2, 0),
            (1, 0),
            (0, 0),
            (6, -1),
            (5, -1),
            (4, -1),
            (3, -1),
            (2, -1),
            (1, -1),
            (6, 1),
            (5, 1),
            (4, 1),
            (3, 1),
            (2, 1),
            (1, 1),
        ];

        foreach (var coinTuple in coinTuples)
        {
            coinPositions.Add(
                new Vector2(startPosition.X + (_coinDistanceRatio * coinTuple.Item1), startPosition.Y + (_coinDistanceRatio * coinTuple.Item2))
            );
        }
    }
    private void BuildDiamond(List<Vector2> coinPositions)
    {
        (int, int)[] coinTuples = [
            (5, -1),
            (4, -1),
            (3, -1),
            (2, -1),
            (1, -1),
            (6, 0),
            (5, 0),
            (4, 0),
            (3, 0),
            (2, 0),
            (1, 0),
            (0, 0),
            (5, 1),
            (4, 1),
            (3, 1),
            (2, 1),
            (1, 1),
            (4, 2),
            (3, 2),
            (2, 2),
            (3, 3),
        ];

        foreach (var coinTuple in coinTuples)
        {
            coinPositions.Add(
                new Vector2(Position.X + (_coinDistanceRatio * coinTuple.Item1), Position.Y + (_coinDistanceRatio * coinTuple.Item2))
            );
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += _velocity * (float)delta;

        if (GetChildCount() == 0)
        {
            QueueFree();
        }
    }
}
