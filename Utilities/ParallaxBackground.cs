using Godot;
using KaimiraGames;
using System;
using System.Collections.Generic;

public partial class ParallaxBackground : Godot.Node2D
{
    private bool _isActive = false;
    private float _canvasWidth = 640f;
    // Using center anchor for sprites means initial position is halfway
    private float _initialCanvasOffset = 320f;

    private WeightedList<PackedScene> BackgroundList = new();

    private WeightedListItem<PackedScene> DoubleDoors = new(
        GD.Load<PackedScene>("res://Backgrounds/DoubleDoors.tscn"), 2
    );
    private WeightedListItem<PackedScene> VentNoDoor = new(
        GD.Load<PackedScene>("res://Backgrounds/VentNoDoor.tscn"), 2
    );
    private WeightedListItem<PackedScene> WindowOnly = new(
        GD.Load<PackedScene>("res://Backgrounds/WindowOnly.tscn"), 2
    );
    private WeightedListItem<PackedScene> WindowWithDoor = new(
        GD.Load<PackedScene>("res://Backgrounds/WindowWithDoor.tscn"), 1
    );
    private WeightedListItem<PackedScene> WindowWithDoorNoPosts = new(
        GD.Load<PackedScene>("res://Backgrounds/WindowWithDoorNoPosts.tscn"), 1
    );
    private WeightedListItem<PackedScene> SingleDoor = new(
        GD.Load<PackedScene>("res://Backgrounds/SingleDoor.tscn"), 3
    );
    private WeightedListItem<PackedScene> OnlyPosts = new(
        GD.Load<PackedScene>("res://Backgrounds/OnlyPosts.tscn"), 3
    );

    private int _persistentBackgroundFrames = 3;
    private int _lastCellWritten;

    public override void _Ready()
    {
        GetNode<Hud>("/root/Main/HUD").Connect(Hud.SignalName.Start, Callable.From(
            OnStart));
        GetNode<Main>("/root/Main").Connect(Main.SignalName.GameOver, Callable.From(
            OnGameOver));

        var backgroundItems = new List<WeightedListItem<PackedScene>>()
        {
            DoubleDoors,
            VentNoDoor,
            WindowOnly,
            WindowWithDoor,
            WindowWithDoorNoPosts,
            OnlyPosts,
            SingleDoor
        };
        BackgroundList = new(backgroundItems);
    }

    public override void _Process(double delta)
    {
        if (_isActive)
        {
            Position -= new Vector2((float)(Globals.BaseGameSpeed * delta), 0);

            var relativeCellDistance = Math.Abs((int)(Position.X) / _canvasWidth);
            var currentCellIndex = (float)Math.Floor(relativeCellDistance);

            if (_lastCellWritten != (int)currentCellIndex && relativeCellDistance - currentCellIndex >= 0.5)
            {
                _lastCellWritten = (int)currentCellIndex;

                // Add cell N + 2 % 3 when halfway through cell N
                var randomBackgroundIndex = Math.Abs((int)GD.Randi() % BackgroundList.Count);
                var backgroundCell = BackgroundList[randomBackgroundIndex].Instantiate() as Sprite2D;

                var newPosition = currentCellIndex + 2;
                backgroundCell.Position = new Vector2((newPosition * _canvasWidth) + _initialCanvasOffset, 180);

                AddChild(backgroundCell);

                // Remove cell N - 1 % 3 when halfway through cell N
                var cellIndexToRemove = currentCellIndex - 1;
                if (cellIndexToRemove >= 0)
                {
                    var cellToRemove = GetChild(0);
                    cellToRemove.QueueFree();
                }
            }
        }
    }

    public void OnStart()
    {
        _isActive = true;
    }

    private void OnGameOver()
    {
        _isActive = false;
    }
}
