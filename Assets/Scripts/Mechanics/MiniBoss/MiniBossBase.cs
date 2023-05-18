using System.Collections;
using UnityEngine;

public class MiniBossBase : MonoBehaviour
{
    //components
    private PlayerValues _playerValues;
    private GameObject _snapPos;
    private CameraChanger _cameraChanger;
    private OrbitCameraController _cameraController;
    [SerializeField] private GameObject _door;
    private RigidbodyConstraints _rigidbodyOriginalConstraints;

    //minigame
    private MiniBossManager miniBossManager;

    //variables
    private bool _minigameFinished, _inside;
    private bool _openDoor, _closeDoor;
    private float _closeY;
    private float _openY;
    private bool minigamePlaying;


    //parameters
    public string bossName;
    public Sprite bossSprite;
    public int openHeight = 5;
    public int bossTurnTime = 10;
    public int gameTime = 20;
    public int sequenceLength = 50;
    public float bossMaxHealth = 100;

    [Range(0, 3)] public int gameDifficulty = 0;

    void Start()
    {
        _cameraController = FindObjectOfType<OrbitCameraController>();
        miniBossManager = FindObjectOfType<MiniBossManager>();
        var parent = transform.parent;
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _snapPos = transform.gameObject.transform.Find("snap pos").gameObject;
        var position = _door.transform.position;
        _closeY = position.y;
        _openY = position.y + openHeight;
        _playerValues = FindObjectOfType<PlayerValues>();
    }


    private void Update()
    {
        if (_openDoor)
        {
            if (_door.transform.position.y < _openY)
                _door.transform.position += new Vector3(0, 0.1f, 0);
            else
                _openDoor = false;
        }

        if (_closeDoor)
        {
            if (_door.transform.position.y > _closeY)
                _door.transform.position -= new Vector3(0, 0.1f, 0);
            else
                _closeDoor = false;
        }
    }


    private void OpenDoor()
    {
        _openDoor = true;
        _closeDoor = false;
    }

    public void CloseDoor()
    {
        _openDoor = false;
        _closeDoor = true;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_minigameFinished && !_inside)
        {
            _inside = true;
            _playerValues.snapRotationTo(_snapPos.transform.eulerAngles.y);
            _playerValues.SnapPositionTo(_snapPos.transform.position);
            _playerValues.Sit();
            StartCoroutine(ChangeCameraCoroutine());
            minigamePlaying = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !minigamePlaying && _inside)
        {
            _inside = false;
        }
    }

    public void EndFight()
    {
        _minigameFinished = true;
        ExitBase();
        OpenDoor();
    }

    IEnumerator ChangeCameraCoroutine()
    {
        yield return new WaitForSeconds(3f);
        miniBossManager.StartMinigame(bossName, bossSprite, bossTurnTime, gameTime, sequenceLength, gameDifficulty,
            bossMaxHealth, this);
        _cameraChanger.SetScreenCamera();
    }

    public void ExitBase()
    {
        _cameraChanger.SetOrbitCamera();
        _playerValues.StandUp(true, 2.5f);
        minigamePlaying = false;
    }
}