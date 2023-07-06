using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Main script, it is in charge of establishing and maintaining the communication between the cube and the application. 
/// </summary>
[DefaultExecutionOrder(2)]
public class CubeConectionManager : MonoBehaviour
{
    private MovesQueue _movesQueue;
    private RunProcess _process;
    private CubeInputs _cubeInputs;
    private string currentDevice = "";
    private bool reEstablish = false, connected = false, refresh = false;


    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private Button continueButton, _connectButton, _cancelButton, _refreshButton;

    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        _process = GetComponent<RunProcess>();
        _movesQueue = GetComponent<MovesQueue>();
        _cubeInputs = FindObjectOfType<CubeInputs>().GetComponent<CubeInputs>();
        _connectButton.onClick.AddListener(ConnectButton);
        _cancelButton.onClick.AddListener(CancelButton);
        _refreshButton.onClick.AddListener(RefreshProcessButton);
        StartCoroutine(GetDevices());
        _cancelButton.interactable = false;
        _connectButton.interactable = false;
        continueButton.interactable = false;
        
    }

    private void Update()
    {
        //its done like this since you cant call startcorutine away from the main thread
        if (reEstablish)
        {
            reEstablish = false;
            StartCoroutine(RestablishComunicationRoutine());
        }
        if (connected)
        {
            connected = false;
            if (continueButton)
                continueButton.interactable = true;
            if (_refreshButton)
                _refreshButton.interactable = false;
            if (_cancelButton)
                _cancelButton.interactable = false;
            if (_dropdown)
                _dropdown.interactable = false;
            _cancelButton = null;
            _refreshButton = null;
            _dropdown = null;
        }

        if (refresh)
        {
            refresh = false;
            RefreshProcessButton();
        }
    }

    public void ConnectButton()
    {
        currentDevice = _dropdown.options[_dropdown.value].text;
        _process.SendMessageProcess(currentDevice);
        _connectButton.interactable = false;
        _refreshButton.interactable = false;
    }

    private void ShowDropdownLabel(int index)
    {
        _dropdown.itemText.text = _dropdown.options[index].text;
    }

    /// <summary>
    /// reads from the messages queue the number of available devices and the devices
    /// </summary>
    /// <returns></returns>
    IEnumerator GetDevices()
    {
        _dropdown.options.Clear();
        _cubeInputs.isActive = false;
        yield return new WaitForSeconds(1);

        Move msg = _movesQueue.Dequeue();
        if (msg.msg == "Device not found")
            msg = _movesQueue.Dequeue();

        int count = int.Parse(msg.msg); //the first message will always be the number of available devices to connect  
        _dropdown.options.Add(new TMP_Dropdown.OptionData("..."));
        for (int i = 0; i < count; i++)
            _dropdown.options.Add(new TMP_Dropdown.OptionData(_movesQueue.Dequeue().msg));
        
        
        _cubeInputs.gameObject.SetActive(true);
        _cubeInputs.isActive = true;
        _refreshButton.interactable = true;
        _connectButton.interactable = true;
    }

    public void ReEstablish()
    {
        reEstablish = true;
    }

    public void Connected()
    {
        connected = true;
    }

    public void Refresh()
    {
        refresh = true;
    }

    public void CancelButton()
    {
        currentDevice = null;
        _cancelButton.interactable = false;
    }

    /// <summary>
    /// if communication fails it will retry establishing communication to the previously selected device.
    /// </summary>
    /// <returns></returns>
    public IEnumerator RestablishComunicationRoutine()
    {
        _process.EndProcess();
        _process.StartProcess();
        //show cancel button
        if (_cancelButton)
        {
            _cancelButton.interactable = true;
        }

        yield return new WaitForSecondsRealtime(1);
        if (currentDevice != null)
            _process.SendMessageProcess(currentDevice);
        else
        {
            if (continueButton)
                _connectButton.interactable = true;
            if (_cancelButton)
                _cancelButton.interactable = false;
        }
    }

    public void RefreshProcessButton()
    {
        _process.SendMessageProcess("j");
        _refreshButton.interactable = false;
        _connectButton.interactable = true;
        StartCoroutine(GetDevices());
    }
}