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
        var currentScore = GetNode<ScoreManager>("/root/Main/Utilities/ScoreManager").GetScore();
        var highScore = GetNode<ScoreManager>("/root/Main/Utilities/ScoreManager").GetHighScore();

        GetNode<Label>("Score").Text = currentScore.ToString() + "m";

        if (highScore > 0 && currentScore > highScore)
        {
            GetNode<Label>("HighScore").Text = "NEW HIGH SCORE!";
        }
        else
        {
            GetNode<Label>("HighScore").Text = "Best: " + highScore.ToString() + "m";
        }

        var coinCountLabel = GetNode<Label>("CoinCount");
        coinCountLabel.Text = GetNode<ScoreManager>("/root/Main/Utilities/ScoreManager").GetCoinCount().ToString();
        var coinCountIcon = GetNode<Sprite2D>("CoinCountIcon");
        coinCountIcon.Position = new Vector2(coinCountLabel.GetMinimumSize().X, coinCountIcon.Position.Y);
    }

    public void OnGameOver()
    {
        GetNode<Button>("StartButton").Show(); 
        // TODO: Should show button to let respawn. Start button doesnt do it but should be equivalent
    }
}
