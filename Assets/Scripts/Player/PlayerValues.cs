using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerValues : MonoBehaviour
{
    #region DATA

    [Header("VALUES")] [SerializeField] private List<float> movementSpeeds;
    public bool canMove = true, isGrounded;
    [SerializeField] [Range(0, 4)] private int gear = 1;
    public Camera mainCamera;

    [Header("COMPONENTES")] public CinemachineFreeLook thirdPersonCamera;

    #endregion

    private void Awake()
    {
        movementSpeeds = new List<float> { -1, 0, 1, 2, 3 };
    }

    private void Start()
    {
        canMove = true;
        gear = 1;
    }

    #region GEAR

    public int GetGear()
    {
        return gear;
    }

    public void RiseGear()
    {
        gear = Mathf.Min(gear + 1, 4);
    }

    public void DecreaseGear()
    {
        gear = Mathf.Max(gear - 1, 0);
    }

    #endregion

    public float GetSpeed()
    {
        return movementSpeeds[gear];
    }
}