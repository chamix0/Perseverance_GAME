using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

/// <summary>
/// This Script is in charge of starting the process which communicates Unity with the Cube.
/// </summary>
[DefaultExecutionOrder(1)]
public class RunProcess : MonoBehaviour
{
    private Process process = null;
    private MovesQueue _messages;
    private CubeInputs _cubeInputs;
    StreamWriter messageStream;


    private void Start()
    {
        StartProcess();
        _messages = GetComponent<MovesQueue>();
        _cubeInputs = FindObjectOfType<CubeInputs>();
    }

    /// <summary>
    /// Kills a previous failed process and starts a new one to reattempt communication.
    /// </summary>
    public void StartProcess()
    {
        if (process != null && !process.HasExited)
            process.Kill();

        try
        {
            process = new Process();
            process.EnableRaisingEvents = false;
            process.StartInfo.WorkingDirectory = Application.dataPath;
            // process.StartInfo.FileName =
            //     Application.dataPath + "/CubeBluetooth/Executable/BluetoothCubo.exe";
            process.StartInfo.FileName =
                Application.dataPath+"/executable/BluetoothCubo.exe"; //change the path to a consistent one
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;
            process.OutputDataReceived += DataReceived;
            process.ErrorDataReceived += ErrorReceived;
            process.Start();
            process.BeginOutputReadLine();
            messageStream = process.StandardInput;

            UnityEngine.Debug.Log("Successfully launched app");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Unable to launch app: " + e.Message);
        }
    }


    /// <summary>
    /// Every time the process writes a message on the standard output, it will be printed on a different colour and stored in the messages
    /// queue.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    void DataReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        string data = eventArgs.Data;
        UnityEngine.Debug.Log($"<color=#00FF00> Process : " + data + "</color>");
        _messages.EnqueueMsg(data);
        _cubeInputs.ProcessMessages(_messages);
    }


    void ErrorReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        UnityEngine.Debug.LogError(eventArgs.Data);
    }

    /// <summary>
    /// method used to write in the the standard input of the process
    /// </summary>
    /// <param name="msg">string to be sent</param>
    public void SendMessageProcess(string msg)
    {
        messageStream.WriteLine(msg);
    }

    /// <summary>
    /// If the aplication quits, it will kill the process first
    /// </summary>
    void OnApplicationQuit()
    {
        EndProcess();
    }

    public void EndProcess()
    {
        if (process != null && !process.HasExited)
        {
            process.Kill();
            print("process dead");
        }
    }
}