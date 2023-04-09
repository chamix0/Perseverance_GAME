using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 
    
    
    SequenceValue
{
    private Move cubeValue;
    private string keyValue;

    private string[] dif0 = { "q", "w", "e", "r", "t" };

    private string[] dif1 =
    {
        "q", "w", "e", "r", "t", "y", "u", "i", "o", "p"
    };

    private string[] dif2 =
    {
        "a", "d", "e", "f", "g", "h", "i", "j", "k", "l", "o", "p", "q", "r", "s", "t", "u",
        "w", "y"
    };

    private string[] dif3 =
    {
        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v",
        "w", "x", "y", "z"
    };

    public SequenceValue(int dif)
    {
        int direction = Random.Range(0, 2);
        direction = direction % 2 == 0 ? -1 : 1;
        int index;
        switch (dif)
        {
            case 0:
                index = Random.Range(0, dif0.Length);
                keyValue = dif0[index];
                break;
            case 1:
                index = Random.Range(0, dif1.Length);
                keyValue = dif1[index];
                break;
            case 2:
                index = Random.Range(0, dif2.Length);
                keyValue = dif2[index];
                break;
            case 3:
                index = Random.Range(0, dif3.Length);
                keyValue = dif3[index];
                break;
            default:
                index = Random.Range(0, dif0.Length);
                keyValue = dif0[index];
                break;
        }

        cubeValue = new Move(Move.GetRandomFace(dif), direction);
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