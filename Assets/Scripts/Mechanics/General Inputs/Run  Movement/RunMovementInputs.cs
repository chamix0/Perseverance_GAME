using Mechanics.General_Inputs;
using UnityEngine;

public class RunMovementInputs : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerValues _playerValues;
    private CameraController _cameraController;
    private Stamina stamina;
    private GuiManager _guiManager;
    [SerializeField] private ParticleSystem turboParticles;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        stamina = FindObjectOfType<Stamina>();
        _cameraController = FindObjectOfType<CameraController>();
        _guiManager = FindObjectOfType<GuiManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //keyboard inputs
        //accelerate and decelerate
        if (_playerValues.GetCurrentInput() == CurrentInput.RaceMovement && _playerValues.GetInputsEnabled()&&!_playerValues.GetPaused())
        {
            if (Input.anyKey)
            {
                _guiManager.SetTutorial(
                    "WS - Increase/Descrease gear    D - Lights");
            }
            if (!stamina.beingShown)
                stamina.ShowStamina();
            if (_playerValues.GetGear() < 4 && turboParticles.isPlaying)
                turboParticles.Stop();
            if (Input.GetKeyDown(KeyCode.W))
            {
                _playerValues.RiseGear();
                if (_playerValues.GetGear() == 4)
                    turboParticles.Play();
                else
                    turboParticles.Stop();
                _playerValues.CheckIfStuck();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _playerValues.DecreaseGear();
                turboParticles.Stop();
                _playerValues.CheckIfStuck();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (_playerValues.GetLights())
                    _playerValues.NotifyAction(PlayerActions.TurnOffLights);
                else
                    _playerValues.NotifyAction(PlayerActions.TurnOnLights);
            }
        }
        else
        {
            if (stamina.beingShown)
                stamina.HideStamina();
            if (turboParticles.isPlaying)
                turboParticles.Stop();
            if (_playerValues.GetGear() == 4)
                _playerValues.DecreaseGear();
        }
    }

    public void PerformAction(Move move)
    {
        _guiManager.SetTutorial(
            "R - Increase/Decrease gear   U - Camera horizonatal axis    L - Camera Vertical Axis     B - Lights");
        //accelerate deccelerate
        if (_playerValues.GetInputsEnabled())
        {
            if (move.face == FACES.R)
            {
                _playerValues.CheckIfStuck();

                if (move.direction == 1)
                    _playerValues.RiseGear();
                else
                    _playerValues.DecreaseGear();

                if (_playerValues.GetGear() == 4)
                    turboParticles.Play();
                else
                    turboParticles.Stop();
            }
        }
        //turn camera in y axis
        if (move.face == FACES.L)
        {
            if (move.direction == 1)
                _cameraController.RotateVerticalDown();
            else
                _cameraController.RotateVerticalUp();
        }
        else if (move.face == FACES.U)
        {
            if (move.direction == 1) _cameraController.RotateClockwise();
            else _cameraController.RotateCounterClockwise();
        }
        else if (move.face == FACES.B)
        {
            if (move.direction == 1)
                if (_playerValues.GetLights())
                    _playerValues.NotifyAction(PlayerActions.TurnOffLights);
                else if (!_playerValues.GetLights())
                    _playerValues.NotifyAction(PlayerActions.TurnOnLights);
        }
    }
}