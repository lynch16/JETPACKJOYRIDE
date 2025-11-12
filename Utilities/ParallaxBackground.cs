using Godot;
using System;

public partial class ParallaxBackground : Godot.Parallax2D
{
    private bool _isActive = false;
    private float _runRate = Globals.BaseGameSpeed;
    private float _canvasWidth = 640f;
    // Using center anchor for sprites means initial position is halfway
    private float _initialCanvasOffset = 320f;

    [Export]
    private PackedScene[] _backgrounds;
    private int _persistentBackgroundFrames = 3;
    private int _lastCellWritten;

    public override void _Ready()
    {
        GetNode<Hud>("/root/Main/HUD").Connect(Hud.SignalName.Start, Callable.From(
            OnStart));
        GetNode<Main>("/root/Main").Connect(Main.SignalName.GameOver, Callable.From(
            OnGameOver));
    }


    public override void _Process(double delta)
    {
        if (_isActive)
        {
            ScrollOffset -= new Vector2((float)(_runRate * delta), 0);

            var relativeCellDistance = Math.Abs((int)(ScrollOffset.X) / _canvasWidth);
            var currentCellIndex = (float)Math.Floor(relativeCellDistance);

            if (_lastCellWritten != (int)currentCellIndex && relativeCellDistance - currentCellIndex >= 0.5)
            {
                _lastCellWritten = (int)currentCellIndex;

            GD.Print("currentCellIndex ", currentCellIndex);
                // Add cell N + 2 % 3 when halfway through cell N
                var randomBackgroundIndex = GD.Randi() % _backgrounds.Length;
                var backgroundCell = _backgrounds[randomBackgroundIndex].Instantiate() as Sprite2D;

                var newPosition = currentCellIndex + 2 % _persistentBackgroundFrames;
            GD.Print("newPosition ", newPosition);
                backgroundCell.Position = new Vector2((newPosition * _canvasWidth) + _initialCanvasOffset, 180);

                AddChild(backgroundCell);

                // Remove cell N - 1 % 3 when halfway through cell N
                var cellIndexToRemove = currentCellIndex - 1;
                GD.Print("cellIndexToRemove ", cellIndexToRemove);
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
        GD.Print("STARTED");
        _isActive = true;
    }

    private void OnGameOver()
    {
        _isActive = false;
    }
}
