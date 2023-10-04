using Mechanics.General_Inputs;
using Player.Observer_pattern;
using UnityEngine;

public class AsteroidsInputs : MonoBehaviour, IObserver
{
    private PlayerValues _playerValues;
    private AsteroidsManager _asteroidsManager;
    private PlayerNewInputs _newInputs;
    private GuiManager _guiManager;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _asteroidsManager = FindObjectOfType<AsteroidsManager>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _guiManager = FindObjectOfType<GuiManager>();
        _playerValues.AddObserver(this);
    }

    // Update is called once per frame
    private void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.AsteroidMinigame && _playerValues.GetInputsEnabled() &&
            !_playerValues.GetPaused())
        {
            Vector2 dir = Vector2.zero;
            if (_newInputs.CheckInputChanged())
            {
                UpdateTutorial();
                _asteroidsManager.SetGearsZero();
            }

            if (_newInputs.UpHold())
            {
                dir.y += 1;
                _asteroidsManager.VerticalMovement(1, _asteroidsManager._speed);
            }

            if (_newInputs.DownHold())
            {
                dir.y -= 1;
                _asteroidsManager.VerticalMovement(-1, _asteroidsManager._speed);
            }

            if (_newInputs.LeftHold())
            {
                dir.x -= 1;
                _asteroidsManager.HorizontalMovement(-1, _asteroidsManager._speed);
            }

            if (_newInputs.RightHold())
            {
                dir.x += 1;
                _asteroidsManager.HorizontalMovement(1, _asteroidsManager._speed);
            }

            _asteroidsManager.SpriteRotation(dir);
        }
    }

    public void PerformAction(Move move)
    {
        if (_playerValues.GetInputsEnabled())
        {
            _newInputs.SetCubeAsDevice();
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();

            if (move.face == FACES.R)
                _asteroidsManager.VerticalMovementCube(move.direction);

            if (move.face == FACES.U)
                _asteroidsManager.HorizontalMovementCube(move.direction);
        }
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode&& _playerValues.GetCurrentInput() == CurrentInput.AsteroidMinigame)
            UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        _guiManager.ShowTutorial();
        _guiManager.SetTutorial(
            _newInputs.AllDirectionsText() + "- Movement ");
    }
}