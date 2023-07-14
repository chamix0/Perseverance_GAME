using System.Diagnostics;
using Player.Observer_pattern;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class PlayerLives : MonoBehaviour,IObserver
{
    // Start is called before the first frame update
    public float cooldown = 5;
    private Stopwatch timer;
    private PlayerValues playerValues;

    private void Awake()
    {
        timer = new Stopwatch();
        timer.Start();
        playerValues = FindObjectOfType<PlayerValues>();
        playerValues.AddObserver(this);

    }

    // Update is called once per frame
    void Update()
    {
        if (playerValues.GetPaused() && timer.IsRunning)
            timer.Stop();
        else if (!playerValues.GetPaused() && !timer.IsRunning)
            timer.Start();

        if (timer.Elapsed.TotalSeconds > cooldown)
        {
            timer.Restart();
            playerValues.AddLive();
            playerValues.NotifyCameraLives();
        }
    }

   

    public void OnNotify(PlayerActions playerAction)
    {
        if (playerAction is PlayerActions.MediumDamage or PlayerActions.LowDamage or PlayerActions.HighDamage)
        {
            timer.Restart();
        }
    }
}