using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceValue
{
    private Move cubeValue;
    private string keyValue;

    private string[] alphabet =
    {
        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v",
        "w", "x", "y", "z"
    };

    public SequenceValue()
    {
        int direction = Random.Range(0, 2);
        direction = direction % 2 == 0 ? -1 : 1;
        int index = Random.Range(0, alphabet.Length);
        cubeValue = new Move(Move.GetRandomFace(), direction);
        keyValue = alphabet[index];
    }

    public Move GetCubeValue()
    {
        return cubeValue;
    }

    public string GetKeyValue()
    {
        return keyValue;
    }
}