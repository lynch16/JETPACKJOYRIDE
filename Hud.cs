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

    private GridContainer _lifeContainer;
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
        _lifeContainer = GetNode<GridContainer>("GameUI/MarginContainer/VBoxContainer/HBoxContainer/LifeCountGrid");
        _scoreManager = GetNode<ScoreManager>("/root/Main/Utilities/ScoreManager");
        _baseSprite = _lifeContainer.GetNode<TextureRect>("LifeSprite");
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

        GetNode<Label>("GameUI/MarginContainer/VBoxContainer/HBoxContainer/Score").Text = currentScore.ToString() + "m";

        if (_isRunning && currentScore > highScore)
        {
            GetNode<Label>("GameUI/MarginContainer/VBoxContainer/HighScore").Text = "NEW HIGH SCORE!";
            GetNode<Label>("GameOverMenu/HBoxContainer/VBoxContainer/GameoverLabel").Text = "NEW HIGH SCORE!";
        }
        else
        {
            GetNode<Label>("GameUI/MarginContainer/VBoxContainer/HighScore").Text = "Best: " + highScore.ToString() + "m";
        }

        var coinCountLabel = GetNode<Label>("GameUI/MarginContainer/VBoxContainer/CoinCount");
        coinCountLabel.Text = GetNode<ScoreManager>("/root/Main/Utilities/ScoreManager").GetCoinCount().ToString();
        var coinCountIcon = GetNode<Sprite2D>("GameUI/MarginContainer/VBoxContainer/CoinCount/CoinCountIcon");
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
        }
        else
        {
            RenderRestartScreen();
        }
    }

    public void UpdateLifeCounter()
    {
        var targetLifeCount = _scoreManager.GetLivesLeft();
        
        // Only update HUD for valid values
        if (targetLifeCount >= 0)
        {
            var currentLifeSpriteCount = _lifeContainer.GetChildCount();

            if (currentLifeSpriteCount == targetLifeCount) { return; }

            _lifeContainer.Columns = targetLifeCount == 0 ? 1 : targetLifeCount % 6;

            // Too many sprites
            if (currentLifeSpriteCount > targetLifeCount)
            {
                for (var i = currentLifeSpriteCount; i > targetLifeCount; i--)
                {
                    var childIndex = i - 1;

                    // Hide instead of remove last so that extra lives can duplicate
                    if (childIndex == 0)
                    {
                        _lifeContainer.GetChild<TextureRect>(childIndex).Hide();
                    }
                    else
                    {
                       _lifeContainer.GetChild(childIndex).QueueFree();
                    }
                }
            }
            // Got a new life
            else
            {
                for (var i = currentLifeSpriteCount; i < targetLifeCount; i++)
                {
                    // First sprite always exists so show instead of add
                    if (i == 0)
                    {
                        _lifeContainer.GetChild<TextureRect>(i).Show();
                    }

                    _lifeContainer.AddChild(_baseSprite.Duplicate());
                }
            }
        }
    }

    public void RenderRestartScreen()
    {
        GetNode<Node2D>("ContinueMenu").Show();
        GetNode<BoxContainer>("GameUI").Show();
        GetNode<Node2D>("TitleUI").Hide();
        GetNode<Node2D>("GameOverMenu").Hide();
        GetNode<Node2D>("PauseMenu").Hide();

        GetNode<Timer>("ContinueMenu/Timer").Start();
        GetNode<AudioStreamPlayer2D>("ContinueMenu/AudioStreamPlayer2D").Play();
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
        GetNode<Label>("ContinueMenu/HBoxContainer/VBoxContainer/ContinueTextContainer/CountDown").Text = _continueTimeout.ToString();
    }

    public void RenderPauseScreen()
    {
        GetNode<Node2D>("PauseMenu").Show();
        GetNode<BoxContainer>("GameUI").Show();
        GetNode<Node2D>("TitleUI").Hide();
        GetNode<Node2D>("GameOverMenu").Hide();
        GetNode<Node2D>("ContinueMenu").Hide();
    }

    public void RenderTitleScreen()
    {
        GetNode<Node2D>("TitleUI").Show();
        GetNode<Node2D>("ContinueMenu").Hide();
        GetNode<BoxContainer>("GameUI").Hide();
        GetNode<Node2D>("GameOverMenu").Hide();
        GetNode<Node2D>("PauseMenu").Hide();

        GetNode<AudioStreamPlayer2D>("TitleUI/AudioStreamPlayer2D").Play();
    }

    public void RenderGameOverScreen()
    {
        GetNode<Node2D>("GameOverMenu").Show();
        GetNode<BoxContainer>("GameUI").Show();
        GetNode<Node2D>("ContinueMenu").Hide();
        GetNode<Node2D>("TitleUI").Hide();
        GetNode<Node2D>("PauseMenu").Hide();
    }

    public void RenderDefault()
    {
        GetNode<BoxContainer>("GameUI").Show();
        GetNode<Node2D>("TitleUI").Hide();
        GetNode<Node2D>("GameOverMenu").Hide();
        GetNode<Node2D>("ContinueMenu").Hide();
        GetNode<Node2D>("PauseMenu").Hide();

        GetNode<AudioStreamPlayer2D>("TitleUI/AudioStreamPlayer2D").Stop();
        GetNode<AudioStreamPlayer2D>("ContinueMenu/AudioStreamPlayer2D").Stop();
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
