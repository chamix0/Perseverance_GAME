using System.Collections.Generic;
using Mechanics.Laberinth;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(4)]
public class LaberinthManager : MonoBehaviour
{
    //components
    private MinigameManager _minigameManager;
    private RigidbodyConstraints _rigidbodyOriginalConstraints;
    [SerializeField] private TMP_Text screenText;
    [SerializeField] private DoorManager doorManager;
    [SerializeField] private GameObject terminalsObject;

    //variables
    private bool _minigameFinished, _inside;

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
        var parent = transform.parent;
        _terminalLaberinthList.AddRange(terminalsObject.GetComponentsInChildren<TerminalLaberinth>());
        terminalsLeftCad = "";
    }

    private void Update()
    {
        if (!_minigameFinished && !terminalsLeftCad.Equals(GetMissingTerminalsCad()))
        {
            terminalsLeftCad = GetMissingTerminalsCad();
            screenText.text = terminalsLeftCad;
            if (GetMissingTerminals() <= 0)
            {
                doorManager.OpenDoor();
                _minigameFinished = true;
            }
        }
    }


    private int GetFinishedTerminals()
    {
        int sum = 0;
        foreach (var terminal in _terminalLaberinthList)
            sum = terminal.GetMinigameFinished() ? sum + 1 : sum;
        return sum;
    }

    public int GetMissingTerminals()
    {
        return _terminalLaberinthList.Count - GetFinishedTerminals();
    }

    private string GetMissingTerminalsCad()
    {
        return "Terminals left : " + GetMissingTerminals();
    }
}