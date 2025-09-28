using Godot;
using System;

public partial class Hud : CanvasLayer
{
    [Signal]
    public delegate void StartEventHandler();

    public void OnStartButtonClicked()
    {
        EmitSignal(SignalName.Start);
        GetNode<Button>("StartButton").Hide();
    }

    public override void _Process(double delta)
    {
        GetNode<Label>("Score").Text = GetNode<ScoreManager>("/root/Main/Utilities/ScoreManager").GetScore().ToString() + "m";
        GetNode<Label>("HighScore").Text = "Best: " + GetNode<ScoreManager>("/root/Main/Utilities/ScoreManager").GetHighScore().ToString() + "m";
    }

    public void OnGameOver()
    {
        GetNode<Button>("StartButton").Show(); 
        // TODO: Should show button to let respawn. Start button doesnt do it but should be equivalent
    }
}
