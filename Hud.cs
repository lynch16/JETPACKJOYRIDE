using Godot;
using System;

public partial class Hud : CanvasLayer
{
    [Signal]
    public delegate void StartEventHandler();

    private HBoxContainer _lifeContainer;
    private ScoreManager _scoreManager;
    private TextureRect _baseSprite;

    public override void _Ready()
    {
        _lifeContainer = GetNode<HBoxContainer>("LifeCountGrid/HBoxContainer");
        _scoreManager = GetNode<ScoreManager>("/root/Main/Utilities/ScoreManager");
        _baseSprite = GetNode<TextureRect>("LifeCountGrid/HBoxContainer/LifeSprite");
        GetNode<Main>("/root/Main").Connect(Main.SignalName.GameOver, Callable.From(
    OnGameOver));
        UpdateLifeCounter();

        RenderTitleScreen();
    }

    public void OnStartButtonClicked()
    {
        EmitSignal(SignalName.Start);
        RenderDefault();
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
        var coinCountIcon = GetNode<Sprite2D>("CoinCount/CoinCountIcon");
        coinCountIcon.Position = new Vector2(coinCountLabel.GetMinimumSize().X, coinCountIcon.Position.Y);

        UpdateLifeCounter();
    }

    public void OnGameOver()
    {
        var currLives = _scoreManager.GetLivesLeft();

        if (currLives < 0)
        {
            RenderGameOverScreen();
            GD.Print("Game Over");
        }
        else
        {
            RenderRestartScreen();
        }

    }

    public void UpdateLifeCounter()
    {
        var lives = _scoreManager.GetLivesLeft();
        var lifeSpriteCount = _lifeContainer.GetChildCount();
        var lifeSpriteDiff = lifeSpriteCount - lives;

        // Only update HUD for valid values
        if (lives >= 0)
        {
            // Too many sprites
            if (lifeSpriteDiff > 0)
            {
                for (var i = lifeSpriteDiff - 1; i >= 0; i--)
                {
                    _lifeContainer.GetChild(i).QueueFree();
                }
                // Got a new life
            }
            else if (lifeSpriteDiff < 0)
            {
                for (var i = lifeSpriteDiff; i < 0; i++)
                {
                    _lifeContainer.AddChild(_baseSprite.Duplicate());
                }
            }
        }
    }

    public void RenderRestartScreen()
    {
        // TODO: Add Quit button
        GetNode<Button>("StartButton").Text = "Restart?";
        GetNode<Button>("StartButton").Show();
    }

    public void RenderTitleScreen()
    {
        GetNode<Button>("StartButton").Show();
        GetNode<Label>("Score").Hide();
        GetNode<Label>("HighScore").Hide();
        GetNode<Label>("CoinCount").Hide();
        GetNode<GridContainer>("LifeCountGrid").Hide();
    }

    public void RenderGameOverScreen()
    {
        // TODO: No restart possible. Just game over
    }

    public void RenderDefault()
    {
        GetNode<Button>("StartButton").Hide();
        GetNode<Label>("Score").Show();
        GetNode<Label>("HighScore").Show();
        GetNode<Label>("CoinCount").Show();
        GetNode<GridContainer>("LifeCountGrid").Show();
    }
}
