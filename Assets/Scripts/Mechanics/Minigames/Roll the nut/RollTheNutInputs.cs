using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollTheNutInputs : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update  
    private PlayerValues _playerValues;
    private MemoryMingameManager memoryMingame;

    void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        memoryMingame = FindObjectOfType<MemoryMingameManager>();
    }

    // Update is called once per frame

    public void PerformAction(Move move)
    {
        memoryMingame.Select(move.color);
    }
}
