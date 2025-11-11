using Godot;
using System;

public partial class Hud : CanvasLayer
{
    [Signal]
    public delegate void StartEventHandler();

    [Signal]
    public delegate void RespawnEventHandler();

    [Signal]
    public delegate void PauseEventHandler();

    private HBoxContainer _lifeContainer;
    private ScoreManager _scoreManager;
    private TextureRect _baseSprite;

    [Export]
    private AudioStream[] _themes;
    private int _currentThemePlayer = 0;
    private AudioStreamPlayer2D _soundtrack;

    private bool _isPaused = false;
    private bool _isRunning = false;
    private bool _isFirstStart = true;

    private int _initialContinueTImeout = 10;
    private int _continueTimeout = 10;

    public override void _Ready()
    {
        _lifeContainer = GetNode<HBoxContainer>("GameUI/LifeCountGrid/HBoxContainer");
        _scoreManager = GetNode<ScoreManager>("/root/Main/Utilities/ScoreManager");
        _baseSprite = GetNode<TextureRect>("GameUI/LifeCountGrid/HBoxContainer/LifeSprite");
        _soundtrack = GetNode<AudioStreamPlayer2D>("Soundtrack");
        _soundtrack.Finished += _OnSoundtrackFinished;

        GetNode<Main>("/root/Main").Connect(Main.SignalName.GameOver, Callable.From(
    OnGameOver));
        UpdateLifeCounter();

        UpdateContinueCountdown();
        RenderTitleScreen();
    } 

    private void _OnSoundtrackFinished()
    {
        _currentThemePlayer++;
        StartSoundtrack();
    }

    public void OnStartButtonClicked()
    {
        

        RenderDefault();
        _isRunning = true;
        _continueTimeout = _initialContinueTImeout;
        GetNode<Timer>("ContinueMenu/Timer").Stop();
        UpdateContinueCountdown();

        if (_isFirstStart)
        {
            _isFirstStart = false;
            StartSoundtrack();
            EmitSignal(SignalName.Start);
        }
        else
        {
            GetNode<Timer>("RespawnTimer").Start(Globals.RespawnTimeout);
            EmitSignal(SignalName.Respawn);
        }
    }

    private void OnRespawnTimeout()
    {
        EmitSignal(SignalName.Start);
        StartSoundtrack();
    }

    private void StartSoundtrack()
    {
        if (_currentThemePlayer > _themes.Length) 
            {
                _currentThemePlayer = 0;
            }

        _soundtrack.Stream = _themes[_currentThemePlayer];
        _soundtrack.Play();
    }

    public override void _Process(double delta)
    {
        var currentScore = GetNode<ScoreManager>("/root/Main/Utilities/ScoreManager").GetScore();
        var highScore = GetNode<ScoreManager>("/root/Main/Utilities/ScoreManager").GetHighScore();

        GetNode<Label>("GameUI/Score").Text = currentScore.ToString() + "m";

        if (highScore > 0 && currentScore > highScore)
        {
            GetNode<Label>("GameUI/HighScore").Text = "NEW HIGH SCORE!";
        }
        else
        {
            GetNode<Label>("GameUI/HighScore").Text = "Best: " + highScore.ToString() + "m";
        }

        var coinCountLabel = GetNode<Label>("GameUI/CoinCount");
        coinCountLabel.Text = GetNode<ScoreManager>("/root/Main/Utilities/ScoreManager").GetCoinCount().ToString();
        var coinCountIcon = GetNode<Sprite2D>("GameUI/CoinCount/CoinCountIcon");
        coinCountIcon.Position = new Vector2(coinCountLabel.GetMinimumSize().X, coinCountIcon.Position.Y);

        UpdateLifeCounter();

        if (_isRunning && Input.IsActionJustPressed("pause"))
        {
            if (_isPaused) {
                Unpause();
            }
            else
            {
                PauseGame();
            }
        }
    }


    public void PauseGame()
    {
        RenderPauseScreen();
        _isPaused = true;
        _isRunning = false;
        GetTree().Paused = true;
    }

    public void Unpause()
    {
        RenderDefault();
        _isPaused = false;
        _isRunning = true;
        GetTree().Paused = false;
    }

    public void OnGameOver()
    {
        var currLives = _scoreManager.GetLivesLeft();
        _isRunning = false;
        _soundtrack.Stop();

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
        GetNode<Node2D>("ContinueMenu").Show();
        GetNode<Node2D>("GameUI").Show();
        GetNode<Node2D>("TitleUI").Hide();
        GetNode<Node2D>("GameOverMenu").Hide();
        GetNode<Node2D>("PauseMenu").Hide();

        GetNode<Timer>("ContinueMenu/Timer").Start();
    }

    public void OnContinueTimerTimeout()
    {
        _continueTimeout--;

        if (_continueTimeout < 0 )
        {
            OnQuitToTitle();
        }

        UpdateContinueCountdown();
    }

    private void UpdateContinueCountdown()
    {
        // TODO: Play countdown sound
        GetNode<Label>("ContinueMenu/HBoxContainer/VBoxContainer/ContinueTextContainer/CountDown").Text = _continueTimeout.ToString();
    }

    public void RenderPauseScreen()
    {
        GetNode<Node2D>("PauseMenu").Show();
        GetNode<Node2D>("GameUI").Show();
        GetNode<Node2D>("TitleUI").Hide();
        GetNode<Node2D>("GameOverMenu").Hide();
        GetNode<Node2D>("ContinueMenu").Hide();
    }

    public void RenderTitleScreen()
    {
        GetNode<Node2D>("TitleUI").Show();
        GetNode<Node2D>("ContinueMenu").Hide();
        GetNode<Node2D>("GameUI").Hide();
        GetNode<Node2D>("GameOverMenu").Hide();
        GetNode<Node2D>("PauseMenu").Hide();
    }

    public void RenderGameOverScreen()
    {
        GetNode<Node2D>("GameOverMenu").Show();
        GetNode<Node2D>("GameUI").Show();
        GetNode<Node2D>("ContinueMenu").Hide();
        GetNode<Node2D>("TitleUI").Hide();
        GetNode<Node2D>("PauseMenu").Hide();
    }

    public void RenderDefault()
    {
        GetNode<Node2D>("GameUI").Show();
        GetNode<Node2D>("TitleUI").Hide();
        GetNode<Node2D>("GameOverMenu").Hide();
        GetNode<Node2D>("ContinueMenu").Hide();
        GetNode<Node2D>("PauseMenu").Hide();
    }

    public void OnQuit()
    {
        Unpause();
        GetTree().Quit();
    }

    public void OnQuitToTitle()
    {
        Unpause();
        GetTree().ReloadCurrentScene();
    }
}
