using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class TargetDoor : MonoBehaviour
{
    // Start is called before the first frame update
    private DoorManager door;
    private List<Target> targets;
    private int finishedTargets = 0;
    private bool _minigameFinished = false;
    private string targetsLeftCad;
    [SerializeField] private TMP_Text screenText;

    private void Awake()
    {
         targets = new List<Target>();
                targets.AddRange(GetComponentsInChildren<Target>());
    }

    void Start()
    {
        door = GetComponentInChildren<DoorManager>();
        door.CloseDoor();
       
        targetsLeftCad = "";
        StartCoroutine(WaitForTargetsCoroutine());
    }

    private void Update()
    {
        if (!_minigameFinished && !targetsLeftCad.Equals(GetMissingTargetsCad()))
        {
            targetsLeftCad = GetMissingTargetsCad();
            screenText.text = targetsLeftCad;
            if (GetMissingTargets() <= 0)
                _minigameFinished = true;
        }
    }

    int GetMissingTargets()
    {
        int count = 0;
        foreach (var target in targets)
        {
            if (target.shot)
                count++;
        }

        return targets.Count - count;
    }

    string GetMissingTargetsCad()
    {
        return "Targets missing : " + GetMissingTargets();
    }


    bool AllTargetsShot()
    {
        foreach (var target in targets)
        {
            if (!target.shot)
                return false;
        }

        return true;
    }


    IEnumerator WaitForTargetsCoroutine()
    {
        yield return new WaitUntil(() => AllTargetsShot());
        door.OpenDoor();
    }
}