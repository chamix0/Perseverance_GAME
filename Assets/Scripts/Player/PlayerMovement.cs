using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //components
    private PlayerValues _playerValues;
    private Rigidbody _rigidbody;
    private SoundManager _soundManager;
    private CameraChanger cameraChanger;
    private PlayerNewInputs _playerNewInputs;
    private PlayerAnimations _playerAnimations;

    //values
    private float speed, _turnSmoothVel;
    public float stompUmbral, rayOffset;
    public float staminaUsage = 1f, staminaRecovery = 0.5f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private LayerMask rayLayers;


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
        //check if its moving and stomped then it should stop
        if (_playerValues.GetGear() > 1 && CheckIfStopm())
        {
            _playerValues.stomp = true;
            _playerValues.StopMovement();
        }

        //check if its not moving and it exited from stomp
        else if (_playerValues.GetGear() == 1 && CheckIfExitedFromStopm())
        {
            _playerValues.stomp = false;
        }

        //if it fell for any reason then reset its pos
        if (_playerValues.GetPos().y < -100)
        {
            _playerValues.ResetPos();
        }
    }

    private void FixedUpdate()
    {
        //moving on ground
        if (_playerValues.GetCanMove() && _playerValues.GetGear() != 1)
        {
            //all the stamina was used reset the value when it reaches full stamina so it can be used again
            if (_playerValues.allStaminaUsed)
            {
                _playerValues.allStaminaUsed = !(_playerValues.stamina >= 100);
            }

            //goes on gear 4 max speed
            if (_playerValues.GetGear() == 4)
            {
                //reduce stamina
                if (_playerValues.stamina > 0)
                {
                    _playerValues.stamina = Mathf.Max(_playerValues.stamina - staminaUsage, 0);
                }
                //all stamina has been used so stop gear 4 and mark all stamina used so it cans refill
                else
                {
                    _playerValues.allStaminaUsed = true;
                    _playerValues.DecreaseGear();
                }
            }
            //goes at any other gear
            else
            {
                //recover stamina slower as its running
                if (_playerValues.GetGear() == 3)
                {
                    _playerValues.stamina =
                        Mathf.Min(_playerValues.stamina + staminaRecovery / 4, _playerValues.maxStamina);
                }
                //recover stamina faster as is stopped or walking
                else
                {
                    _playerValues.stamina =
                        Mathf.Min(_playerValues.stamina + staminaRecovery, _playerValues.maxStamina);
                }
            }

            //get move direction
            Vector3 moveDirection = GetMoveDirection() * _playerValues.gampadAddedSpeed;
            
            //apply movement
            _rigidbody.AddForce(moveDirection * _playerValues.GetSpeed() - _rigidbody.velocity,
                ForceMode.VelocityChange);
        }

        //recover stamina
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
            {
                direction.z = 1;
            }
            else if (_playerNewInputs.GearDownPressed())
            {
                direction.z = -1;
            }

            if (_playerNewInputs.GearLeftPressed())
            {
                direction.x = -1;
            }
            else if (_playerNewInputs.GearRightPressed())
            {
                direction.x = 1;
            }

            speed = 1;
        }

        _playerValues.gampadAddedSpeed = speed;
        _playerAnimations.UpdateGamePadSpeedAnim(speed);
        _soundManager.SetMoveSoundSpeed(speed);
        return direction;
    }
}