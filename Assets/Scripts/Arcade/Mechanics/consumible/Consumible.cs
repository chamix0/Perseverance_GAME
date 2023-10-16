using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UTILS;
using Random = UnityEngine.Random;

public enum ConsumibleType
{
    PointMultiplier,
    FullAmmo
}

public class Consumible : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _readyToUse;
    private bool _used;

    [SerializeField] private ConsumibleType consumibleType;

    //mesh
    private GameObject _selecctedMesh;
    [SerializeField] private GameObject doublePointsMesh, maxAmmoMesh;
    private GuiManager _guiManager;
    private ArcadePlayerData _playerData;

    //timer
    [SerializeField] private MyStopWatch _wasteTimer;
    private float _wasteTime = 30;

    //audio
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip maxAmmoClip, doublePointsClip;

    //colider
    [SerializeField] private BoxCollider boxCollider;

    void Start()
    {
        _playerData = FindObjectOfType<ArcadePlayerData>();
        _guiManager = FindObjectOfType<GuiManager>();
        _selecctedMesh = consumibleType is ConsumibleType.FullAmmo ? maxAmmoMesh : doublePointsMesh;
        UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_used && !_readyToUse)
        {
            if (_wasteTimer.GetElapsedSeconds() > _wasteTime)
                Waste();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_readyToUse && !_used && other.gameObject.CompareTag("Player"))
        {
            UseConsumible();
        }
    }

    public void DeployConsumible(Vector3 pos)
    {
        transform.position = pos;
        _wasteTimer.StartStopwatch();
        _readyToUse = false;
        _used = false;
        //colider
        boxCollider.enabled = true;
        //mesh
        int ranIndex = Random.Range(0, 2);
        consumibleType = ranIndex == 0 ? ConsumibleType.FullAmmo : ConsumibleType.PointMultiplier;

        if (this.consumibleType is ConsumibleType.FullAmmo)
            _selecctedMesh = maxAmmoMesh;
        else if (this.consumibleType is ConsumibleType.PointMultiplier)
            _selecctedMesh = doublePointsMesh;
        UpdateMesh();
    }

    public void UseConsumible()
    {
        _used = true;
        _readyToUse = false;
        //timer
        _wasteTimer.Stop();
        _wasteTimer.ResetStopwatch();
        //hide
        boxCollider.enabled = false;

        //action
        if (consumibleType is ConsumibleType.FullAmmo)
        {
            _playerData.FillAmmo();
            //play audio
            _audioSource.clip = maxAmmoClip;
            _audioSource.Play();
            Waste();
        }
        else if (consumibleType is ConsumibleType.PointMultiplier)
        {
            StartCoroutine(PointCoroutine());
            HideMesh();
            //play audio
            _audioSource.clip = doublePointsClip;
            _audioSource.Play();
        }
    }

    private void UpdateMesh()
    {
        if (_selecctedMesh == maxAmmoMesh)
        {
            maxAmmoMesh.SetActive(true);
            doublePointsMesh.SetActive(false);
        }
        else if (_selecctedMesh == doublePointsMesh)
        {
            maxAmmoMesh.SetActive(false);
            doublePointsMesh.SetActive(true);
        }
        else
        {
            maxAmmoMesh.SetActive(false);
            doublePointsMesh.SetActive(false);
        }
    }

    private void Waste()
    {
        _used = false;
        _readyToUse = true;
        //timer
        _wasteTimer.Stop();
        _wasteTimer.ResetStopwatch();
        //hide
        HideMesh();
        boxCollider.enabled = false;
    }

    IEnumerator PointCoroutine()
    {
        _guiManager.ShowConsumible();
        _playerData.SetConsumibleMultipler(2);
        yield return new WaitForSeconds(45);
        _playerData.SetConsumibleMultipler(1);
        _guiManager.HideConsumible();
        Waste();
    }

    public void SetIsReadyToUse(bool val)
    {
        _readyToUse = val;
    }

    public bool GetIsReadyToUse()
    {
        return _readyToUse;
    }

    private void HideMesh()
    {
        maxAmmoMesh.SetActive(false);
        doublePointsMesh.SetActive(false);
    }
}