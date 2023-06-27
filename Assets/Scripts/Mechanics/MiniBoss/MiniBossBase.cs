using System.Collections;
using UnityEngine;

public class MiniBossBase : MonoBehaviour
{
    //components
    private PlayerValues _playerValues;
    private GameObject _snapPos;
    private CameraChanger _cameraChanger;
    private RigidbodyConstraints _rigidbodyOriginalConstraints;

    //minigame
    private MiniBossManager miniBossManager;

    //variables
    private bool _minigameFinished, _inside;
    private bool minigamePlaying;


    //parameters
    public string bossName;
    public Sprite bossSprite;
    public int bossTurnTime = 10;
    public int gameTime = 20;
    public int sequenceLength = 50;
    public float bossMaxHealth = 100;
    [SerializeField] private DoorManager _doorManager;

    [Range(0, 3)] public int gameDifficulty = 0;

    void Start()
    {
        miniBossManager = FindObjectOfType<MiniBossManager>();
        var parent = transform.parent;
        _cameraChanger = FindObjectOfType<CameraChanger>();
        _snapPos = transform.gameObject.transform.Find("snap pos").gameObject;
        _playerValues = FindObjectOfType<PlayerValues>();
    }


    private void Update()
    {
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
        _doorManager.OpenDoor();
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