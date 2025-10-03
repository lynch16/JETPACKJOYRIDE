using Godot;
using System;

public partial class ScoreManager : Node
{
    private int _score;
    private int _coinsCollected;
    private double _currentRunTimeSec;
    private int _highScore;

    private bool _gameStarted = false;

    private SaveManager _saveManager;

    private float _speedRamp = 25f; // In m to calculate final speed
    private double _speed = 10f; // In m/s to calculate score represented in meters

    public override void _Ready()
    {
        _saveManager = new SaveManager();
        _highScore = _saveManager.LoadHighScore();
    }

    // Using physics process instead of physics as score is
    // based on distance which is a physical property that should not 
    // change with clock speed
    public override void _PhysicsProcess(double delta)
    {
        if (_gameStarted)
        {
            _currentRunTimeSec += delta;
            var totalGameTime = Mathf.Floor(_currentRunTimeSec);
            var totalDistance = totalGameTime * _speed;
            _speed = totalGameTime / _speedRamp + 10;
            _score = (int)Mathf.Round(totalDistance);
        }
    }

    public int GetScore()
    {
        return _score;
    }

    public void OnCoinHit()
    {
        _coinsCollected++;
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
            GD.Print("NEW _highScore " + _highScore);
            _saveManager.SaveHighScore(_highScore);
        }
    }

    public void OnStart()
    {
        _gameStarted = true;
    }
}
