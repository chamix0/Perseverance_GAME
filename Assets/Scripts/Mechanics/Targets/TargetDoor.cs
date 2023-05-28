using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TargetDoor : MonoBehaviour
{
    // Start is called before the first frame update
    private DoorManager door;
    private List<Target> targets;
    private int finishedTargets = 0;

    void Start()
    {
        door = GetComponentInChildren<DoorManager>();
        door.CloseDoor();
        targets = new List<Target>();
        targets.AddRange(GetComponentsInChildren<Target>());

        StartCoroutine(WaitForTargetsCoroutine());
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
        print("AAAAAAAAA");
        door.OpenDoor();
    }
}