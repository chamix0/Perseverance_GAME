using Mechanics.General_Inputs;
using Player.Observer_pattern;
using UnityEngine;

public class DontTouchWallsInputs : MonoBehaviour, IObserver
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private DontTouchWallsManager _dontTouchWallsManager;
    private PlayerNewInputs _newInputs;
    private GuiManager _guiManager;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        _dontTouchWallsManager = FindObjectOfType<DontTouchWallsManager>();
        _newInputs = FindObjectOfType<PlayerNewInputs>();
        _guiManager = FindObjectOfType<GuiManager>();
        _playerValues.AddObserver(this);
    }

    // Update is called once per frame
    private void Update()
    {
        if (_playerValues.GetCurrentInput() == CurrentInput.DontTouchTheWallsMinigame &&
            _playerValues.GetInputsEnabled() && !_playerValues.GetPaused())
        {
            Vector2 dir = Vector2.zero;
            float speed = _dontTouchWallsManager._speed;
            if (_newInputs.CheckInputChanged())
            {
                UpdateTutorial();
                _dontTouchWallsManager.SetGearsZero();
                _dontTouchWallsManager.ShowKeyTutorial();
            }


            if (_newInputs.Sprint())
                speed *= 1.25f;

            if (_newInputs.UpHold())
            {
                dir.y += 1;
                _dontTouchWallsManager.VerticalMovement(1, speed);
            }

            if (_newInputs.DownHold())
            {
                dir.y -= 1;
                _dontTouchWallsManager.VerticalMovement(-1, speed);
            }

            if (_newInputs.LeftHold())
            {
                dir.x -= 1;
                _dontTouchWallsManager.HorizontalMovement(-1, speed);
            }

            if (_newInputs.RightHold())
            {
                dir.x += 1;
                _dontTouchWallsManager.HorizontalMovement(1, speed);
            }

            _dontTouchWallsManager.SpriteRotation(dir);
        }
    }

    public void PerformAction(Move move)
    {
        _dontTouchWallsManager.ShowCubeTutorial();
        if (_playerValues.GetInputsEnabled())
        {
            _newInputs.SetCubeAsDevice();
            if (_newInputs.CheckInputChanged())
                UpdateTutorial();

            if (move.face == FACES.R)
                _dontTouchWallsManager.VerticalMovementCube(move.direction);


            else if (move.face == FACES.U)
                _dontTouchWallsManager.HorizontalMovementCube(move.direction);
        }
    }

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.ChangeInputMode&& _playerValues.GetCurrentInput() == CurrentInput.DontTouchTheWallsMinigame)
            UpdateTutorial();
    }

    private void UpdateTutorial()
    {
        _guiManager.ShowTutorial();
        _guiManager.SetTutorial(
            _newInputs.AllDirectionsText() + "- Movement ");
    }
}