using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Pattern : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private List<Transform> fixedPoints;
    [SerializeField] private int numPatrolTargets;
    private List<Target> patrolTargets;
    private List<Target> fixedTargets;
    [SerializeField] private Transform targetContainer;
    [SerializeField] private GameObject targetTemplate;
    [SerializeField] private float speed = 5;
    [SerializeField] private float timeBetweenTargets;
    private Stopwatch stopwatch;
    private List<float> _tP;
    private List<int> targetDest;
    private bool updatePos;

    void Start()
    {
        patrolTargets = new List<Target>();
        fixedTargets = new List<Target>();
        _tP = new List<float>();
        targetDest = new List<int>();
        stopwatch = new Stopwatch();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (updatePos)
            UpdatePosition();
    }

    private void AsignTargets()
    {
        for (int i = 0; i < numPatrolTargets; i++)
        {
            GameObject targetObj = Instantiate(targetTemplate, targetContainer);
            Target target = targetObj.GetComponent<Target>();
            _tP.Add(0);
            targetDest.Add(1);
            patrolTargets.Add(target);
            target.DisableTarget();
        }

        for (int i = 0; i < fixedPoints.Count; i++)
        {
            GameObject targetObj = Instantiate(targetTemplate, targetContainer);
            Target target = targetObj.GetComponent<Target>();
            fixedTargets.Add(target);
            target.DisableTarget();
        }
    }

    private void SetTargetsPos()
    {
        for (int i = 0; i < fixedPoints.Count; i++)
        {
            fixedTargets[i].transform.position = fixedPoints[i].position;
            fixedTargets[i].EnableTarget();
        }

        for (int i = 0; i < numPatrolTargets; i++)
        {
            patrolTargets[i].transform.position = patrolPoints[0].position;
            targetDest[i] = 1;
        }
    }

    public void StartPattern()
    {
        if (patrolTargets.Count <= 0)
            AsignTargets();
        StartCoroutine(DelayStartCoroutine());
    }


    public int EndPattern()
    {
        int count = 0;
        foreach (var tar in fixedTargets)
        {
            count += tar.DisableTarget();
        }

        foreach (var target in patrolTargets)
        {
            count += target.DisableTarget();
        }

        updatePos = false;
        return count;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < patrolPoints.Count; i++)
        {
            Debug.DrawLine(patrolPoints[i].position, patrolPoints[(i + 1) % patrolPoints.Count].position);
        }
    }


    private void UpdatePosition()
    {
        for (int i = 0; i < numPatrolTargets; i++)
        {
            if (stopwatch.Elapsed.TotalSeconds > timeBetweenTargets * i)
            {
                if (!patrolTargets[i].deployed)
                    patrolTargets[i].EnableTarget();
                _tP[i] += Time.deltaTime * speed;
                patrolTargets[i].transform.position = Vector3.MoveTowards(patrolTargets[i].transform.position,
                    patrolPoints[targetDest[i]].position, Time.deltaTime * speed);

                if (Vector3.Distance(patrolTargets[i].transform.position,
                        patrolPoints[targetDest[i]].position) < 0.01)
                {
                    targetDest[i] = (targetDest[i] + 1) % patrolPoints.Count;
                    _tP[i] = 0;
                }
            }
        }
    }

    IEnumerator DelayStartCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        SetTargetsPos();
        updatePos = true;
        stopwatch.Restart();
    }
}