using System.Diagnostics;
using UnityEngine;

public class ArduinoManager : MonoBehaviour
{
    private ArduinoConectionManager _conection;
    private RunProcessArduino _processArduino;
    public bool isActive = false;
    private Stopwatch shakeTimer;
    [SerializeField] float minShakes = 50;
    [SerializeField] float minTime = 2;
    private int shakeCount = 0;
    private bool Shaked = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        _conection = FindObjectOfType<ArduinoConectionManager>();
        _processArduino = FindObjectOfType<RunProcessArduino>();
        shakeTimer = new Stopwatch();
        shakeTimer.Start();
    }

    public void ProcessMessages(MovesQueue movesQueue)
    {
        if (isActive && movesQueue.HasMessages())
        {
            Move move = movesQueue.Dequeue();
            //check if connection lost
            if (move.msg == "connection failed")
            {
                print("connection has failed");
                _conection.ReEstablish();
            }
            else if (move.msg == "Device not found")
            {
                print("device not found");
                _conection.Refresh();
            }
            else if (move.msg == "connection successful")
            {
                print("connection successful");
                _conection.Connected();
            }
            else
            {
                if (!Shaked)
                    IsThereShake(move.msg);
            }
        }
    }

    #region Shake

    public bool GetShake()
    {
        return Shaked;
    }

    public void SetShakeToFalse()
    {
        Shaked = false;
        shakeTimer.Restart();
    }

    private void IsThereShake(string val)
    {
        if (val == "Motion sensor: 1")
        {
            shakeCount++;
        }

        if (shakeTimer.Elapsed.TotalSeconds > minTime && shakeCount < minShakes)
        {
            shakeCount = 0;
            shakeTimer.Restart();
            Shaked = false;
        }
        else if (shakeTimer.Elapsed.TotalSeconds > minTime && shakeCount >= minShakes)
        {
            shakeCount = 0;
            shakeTimer.Stop();
            shakeTimer.Reset();
            Shaked = true;
        }
    }

    #endregion

    #region Actions

    public void StopVibration()
    {
        _processArduino.SendMessageProcess("0");
    }

    public void StartVibration()
    {
        _processArduino.SendMessageProcess("1");
    }

    public void SetNormalFace()
    {
        _processArduino.SendMessageProcess("2");
    }

    public void SetScaredFace()
    {
        _processArduino.SendMessageProcess("3");
    }

    public void SetBlinkFace()
    {
        _processArduino.SendMessageProcess("4");
    }

    public void SetDeadFace()
    {
        _processArduino.SendMessageProcess("5");
    }

    public void SetLights()
    {
        _processArduino.SendMessageProcess("6");
    }

    public void SetPauseIcon()
    {
        _processArduino.SendMessageProcess("7");
    }

    public void SetAsteroids()
    {
        _processArduino.SendMessageProcess("8");
    }

    public void SetColorMinigame()
    {
        _processArduino.SendMessageProcess("9");
    }

    public void SetMemorizeMinigame()
    {
        _processArduino.SendMessageProcess("10");
    }

    public void SetWaitMinigame()
    {
        _processArduino.SendMessageProcess("11");
    }

    public void SetWallsMinigame()
    {
        _processArduino.SendMessageProcess("12");
    }

    public void SetPuzzleMinigame()
    {
        _processArduino.SendMessageProcess("13");
    }

    public void SetRollMinigame()
    {
        _processArduino.SendMessageProcess("14");
    }

    public void SetPushMinigame()
    {
        _processArduino.SendMessageProcess("15");
    }

    public void SetAdjustMinigame()
    {
        _processArduino.SendMessageProcess("16");
    }

    #endregion
}