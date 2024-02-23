using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerValues _playerValues;
    private Rigidbody _rigidbody;
    private float speed, _turnSmoothVel;
    public float stompUmbral, rayOffset;
    private SoundManager _soundManager;

    public float staminaUsage = 1f, staminaRecovery = 0.5f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private LayerMask rayLayers;
    private CameraChanger cameraChanger;
    private PlayerNewInputs _playerNewInputs;
    private PlayerAnimations _playerAnimations;


    private void Start()
    {
        cameraChanger = FindObjectOfType<CameraChanger>();
        _rigidbody = GetComponent<Rigidbody>();
        _playerValues = GetComponent<PlayerValues>();
        _playerNewInputs = FindObjectOfType<PlayerNewInputs>();
        _playerAnimations = FindObjectOfType<PlayerAnimations>();
        _soundManager = FindObjectOfType<SoundManager>();
    }

    private void Update()


    {
        if (_playerValues.GetGear() > 1 && CheckIfStopm())
        {
            _playerValues.stomp = true;
            _playerValues.StopMovement();
        }
        else if (_playerValues.GetGear() == 1 && CheckIfExitedFromStopm())
        {
            _playerValues.stomp = false;
        }


        if (_playerValues.GetPos().y < -100)
            _playerValues.ResetPos();
    }

    private void FixedUpdate()
    {
        //move on ground
        if (_playerValues.GetCanMove() && _playerValues.GetGear() != 1)
        {
            if (_playerValues.allStaminaUsed)
                _playerValues.allStaminaUsed = !(_playerValues.stamina >= 100);

            if (_playerValues.GetGear() == 4)
            {
                if (_playerValues.stamina > 0)
                {
                    _playerValues.stamina = Mathf.Max(_playerValues.stamina - staminaUsage, 0);
                }
                else
                {
                    _playerValues.allStaminaUsed = true;
                    _playerValues.DecreaseGear();
                }
            }
            else
            {
                if (_playerValues.GetGear() == 3)
                    _playerValues.stamina =
                        Mathf.Min(_playerValues.stamina + staminaRecovery / 4, _playerValues.maxStamina);
                else
                    _playerValues.stamina =
                        Mathf.Min(_playerValues.stamina + staminaRecovery, _playerValues.maxStamina);
            }

            Vector3 moveDirection = GetMoveDirection() * _playerValues.gampadAddedSpeed;
            _rigidbody.AddForce(moveDirection * _playerValues.GetSpeed() - _rigidbody.velocity,
                ForceMode.VelocityChange);
        }
        else
        {
            _playerValues.stamina = Mathf.Min(_playerValues.stamina + staminaRecovery, _playerValues.maxStamina);
        }
    }

    private bool CheckIfExitedFromStopm()
    {
        Vector3 playerDir = GetMoveDirectionValue();

        return Vector3.Dot(transform.forward, playerDir.normalized) < 0.7f;
    }

    private bool CheckIfStopm()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + new Vector3(0, rayOffset, 0),
            transform.forward);
        Vector3 playerDir = GetMoveDirectionValue();

        print(playerDir);
        print(Vector3.Dot(ray.direction.normalized, playerDir.normalized));
        if (Physics.Raycast(ray, out hit, stompUmbral, rayLayers) &&
            Vector3.Dot(ray.direction.normalized, playerDir.normalized) >= 0.7f)
        {
            print("Collision");
            _playerValues.NotifyAction(PlayerActions.Stomp);
            return true;
        }

        return false;
    }


    private Vector3 GetMoveDirection()
    {
        Vector3 direction = DesiredDirection();


        float targetAngle = Mathf.Atan2(direction.x, direction.z) *
                            Mathf.Rad2Deg +
                            cameraChanger.GetActiveCam().transform.eulerAngles.y;

        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVel, turnSmoothTime);

        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        return moveDirection;
    }

    private Vector3 GetMoveDirectionValue()
    {
        Vector3 direction = DesiredDirection();


        float targetAngle = Mathf.Atan2(direction.x, direction.z) *
                            Mathf.Rad2Deg +
                            cameraChanger.GetActiveCam().transform.eulerAngles.y;

        Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        return moveDirection;
    }

    private Vector3 DesiredDirection()
    {
        Vector3 direction = Vector3.zero;
        float speed = 0;

        if (_playerNewInputs.currentDevice is MyDevices.Cube)
        {
            direction = new Vector3(0, 0f, 1).normalized;
            speed = 1;
        }
        else if (_playerNewInputs.currentDevice is MyDevices.GamePad)
        {
            Vector3 aux = _playerNewInputs.GetMovementAxis();
            direction.x = aux.x;
            direction.z = aux.y;
            speed = direction.magnitude;
        }
        else
        {
            if (_playerNewInputs.GearUpPressed())
                direction.z = 1;
            else if (_playerNewInputs.GearDownPressed())
                direction.z = -1;

            if (_playerNewInputs.GearLeftPressed())
                direction.x = -1;
            else if (_playerNewInputs.GearRightPressed())
                direction.x = 1;
            speed = 1;
        }

        _playerValues.gampadAddedSpeed = speed;
        _playerAnimations.UpdateGamePadSpeedAnim(speed);
        _soundManager.SetMoveSoundSpeed(speed);
        return direction;
    }
}