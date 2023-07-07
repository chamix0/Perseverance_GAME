using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(6)]
public class ShootingRangeManager : MonoBehaviour
{
    private PlayerValues playerValues;
    private bool _challengStarted = false;

    [SerializeField] private TMP_Text maxScore;

    //phases
    private int phase = 0;
    [SerializeField] private List<Phase> Phases;
    [SerializeField] private List<int> phasesTimes;

    // hit count
    private int hitCount = 0;

    //start target
    [SerializeField] private Target startTarget;
    [SerializeField] private TMP_Text startText;
    private Stopwatch timer;
    private float cooldown;


    private void Awake()
    {
        timer = new Stopwatch();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
        maxScore.text = "" + playerValues.gameData.GetMaxShootingScore();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_challengStarted && startTarget.shot)
        {
            _challengStarted = true;
            StartCoroutine(StartChallengeCoroutine());
        }

        if (_challengStarted)
        {
            PhaseCheck();
        }
    }


    public void StartChallenge()
    {
        hitCount = 0;
        _challengStarted = true;
        startTarget.DisableTarget();
        StartPhase();
        timer.Restart();
    }

    private void PhaseCheck()
    {
        if (timer.Elapsed.TotalSeconds > cooldown)
        {
            timer.Restart();
            hitCount += EndPhase();
            NextPhase();
        }
    }

    private int EndPhase()
    {
        return Phases[phase].EndPhase();
    }

    private void StartPhase()
    {
        Phases[phase].StartPhase();
        cooldown = phasesTimes[phase];
    }

    private void NextPhase()
    {
        phase++;
        if (phase < Phases.Count)
        {
            StartPhase();
        }
        else
        {
            //end challenge
            _challengStarted = false;
            startTarget.EnableTarget();
            phase = 0;
            playerValues.gameData.SetMaxShootingScore(hitCount);
            maxScore.text = "" + playerValues.gameData.GetMaxShootingScore();
            playerValues.SaveGame();
            print("end");
            StartCoroutine(EndChallengeCoroutine());
        }
    }

    IEnumerator EndChallengeCoroutine()
    {
        yield return new WaitForSeconds(2);
        startText.text = "SCORE: " + hitCount;
        yield return new WaitForSeconds(2);
        startText.text = "START";
    }

    IEnumerator StartChallengeCoroutine()
    {
        yield return new WaitForSeconds(1);

        for (int i = 3; i > 0; i--)
        {
            startText.text = i + "";
            yield return new WaitForSeconds(1);
        }

        startText.text = "GO!";
        yield return new WaitForSeconds(1);
        startText.text = "";
        StartChallenge();
    }
}