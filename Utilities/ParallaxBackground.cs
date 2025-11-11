using Godot;
using System;

public partial class ParallaxBackground : Godot.Parallax2D
{
    private bool _isActive = false;
    private float _runRate = Globals.BaseGameSpeed;

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
