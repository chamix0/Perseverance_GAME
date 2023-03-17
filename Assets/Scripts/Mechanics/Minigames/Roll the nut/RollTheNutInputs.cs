using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollTheNutInputs : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update  
    private PlayerValues _playerValues;
    private RollTheNutManager rollTheNutManager;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        rollTheNutManager = FindObjectOfType<RollTheNutManager>();
    }

    // Update is called once per frame

    public void PerformAction(Move move)
    {
        if (move.face == FACES.F)
        {
            if (move.direction == 1)
            {
                rollTheNutManager.RollClockWise();
            }
            else
            {
                rollTheNutManager.RollCounterClockWise();
            }
        }
    }
}