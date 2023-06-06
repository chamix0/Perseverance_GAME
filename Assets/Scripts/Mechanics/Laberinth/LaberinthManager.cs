using System.Collections;
using System.Collections.Generic;
using Mechanics.Laberinth;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(4)]
public class LaberinthManager : MonoBehaviour
{
    //components
    private PlayerValues _playerValues;
    private MinigameManager _minigameManager;
    private CameraChanger cameraChanger;
    private GameObject door;
    private RigidbodyConstraints _rigidbodyOriginalConstraints;
    private GenericScreenUi _genericScreenUi;
    [SerializeField] private TMP_Text screenText;

    //variables
    private bool _minigameFinished, _inside;
    private bool _openDoor, _closeDoor;
    private float _closeY;

    private string terminalsLeftCad;

    //values
    private float _openY;

    //lists
    private List<TerminalLaberinth> _terminalLaberinthList;

    private void Awake()
    {
        _terminalLaberinthList = new List<TerminalLaberinth>();
    }

    void Start()
    {
        _genericScreenUi = FindObjectOfType<GenericScreenUi>();
        var parent = transform.parent;
        _terminalLaberinthList.AddRange(parent.parent.GetComponentsInChildren<TerminalLaberinth>());
        cameraChanger = FindObjectOfType<CameraChanger>();
        door = parent.Find("Door").gameObject;
        var position = door.transform.position;
        _openY = position.y + 5;
        _closeY = position.y;
        _playerValues = FindObjectOfType<PlayerValues>();
        terminalsLeftCad = "";
    }

    private void Update()
    {
        if (_openDoor)
        {
            if (door.transform.position.y < _openY)
                door.transform.position += new Vector3(0, 0.1f, 0);
            else
                _openDoor = false;
        }

        if (_closeDoor)
        {
            if (door.transform.position.y > _closeY)
                door.transform.position -= new Vector3(0, 0.1f, 0);
            else
                _closeDoor = false;
        }

        if (!_minigameFinished&&!terminalsLeftCad.Equals(GetMissingTerminalsCad()))
        {
            terminalsLeftCad = GetMissingTerminalsCad();
            screenText.text = terminalsLeftCad;
            if (GetMissingTerminals() <= 0)
            {
                OpenDoor();
                _minigameFinished = true;
            }
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

    private int GetFinishedTerminals()
    {
        int sum = 0;
        foreach (var terminal in _terminalLaberinthList)
            sum = terminal.GetMinigameFinished() ? sum + 1 : sum;
        return sum;
    }
    
    private int GetMissingTerminals()
    {
        return _terminalLaberinthList.Count - GetFinishedTerminals();
    }

    private string GetMissingTerminalsCad()
    {
        return "Terminals left : " + GetMissingTerminals();
    }
}