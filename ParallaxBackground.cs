using Godot;
using System;

public partial class ParallaxBackground : Godot.ParallaxBackground
{
    private bool _isActive = false;
    private float _runRate = Globals.BaseGameSpeed;

    public override void _Process(double delta)
    {
        if (_isActive)
        {
            ScrollBaseOffset -= new Vector2((float)(_runRate * delta), 0);
        }
    }

    private void OnStart()
    {
        _isActive = true;
    }

    private void OnGameOver()
    {
        _isActive = false;
    }
}
