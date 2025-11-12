using Godot;
using System;

public partial class ScoreManager : Node
{
    private int _score;
    private int _coinsCollected;
    private double _currentRunTimeSec;
    private int _highScore;
    private int _lives = 2;
    private int _coinLifeThreshold = 100;

    private bool _gameStarted = false;

    private SaveManager _saveManager;

    private double _speed = 10f; // In m/s to calculate score represented in meters

    public override void _Ready()
    {
        _saveManager = new SaveManager();
        _highScore = _saveManager.LoadHighScore();

        GetNode<Hud>("/root/Main/HUD").Connect(Hud.SignalName.Start, Callable.From(
            OnStart));
        GetNode<Main>("/root/Main").Connect(Main.SignalName.GameOver, Callable.From(
            OnGameOver));
    }

    // Using physics process instead of physics as score is
    // based on distance which is a physical property that should not 
    // change with clock speed
    public override void _PhysicsProcess(double delta)
    {
        if (_gameStarted)
        {
            _currentRunTimeSec += delta;
            var totalDistance = _currentRunTimeSec * _speed;
            _score = (int)Mathf.Round(totalDistance);
        }
    }

    public int GetScore()
    {
        return _score;
    }

    public int GetLivesLeft()
    {
        return _lives;
    }

    public void RemoveLife()
    {
        _lives--;
    }

    public void OnCoinHit()
    {
        _coinsCollected++;
        
        // Every 100 coins, give extra life and reset coin count
        if (_coinsCollected == _coinLifeThreshold)
        {
            _lives++;
            _coinsCollected = 0;
        }
    }

    public int GetCoinCount()
    {
        return _coinsCollected;
    }

    public int GetHighScore()
    {
        return _highScore;
    }

    public void OnGameOver()
    {
        _gameStarted = false;

        if (_score > _highScore)
        {
            _highScore = _score;
            _saveManager.SaveHighScore(_highScore);
        }
    }

    public void OnStart()
    {
        _gameStarted = true;
    }
}
