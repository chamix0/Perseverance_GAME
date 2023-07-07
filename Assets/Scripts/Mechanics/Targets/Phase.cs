using System.Collections.Generic;
using UnityEngine;

public class Phase : MonoBehaviour
{
    [SerializeField] private List<Pattern> patterns;

    public void StartPhase()
    {
        foreach (var pattern in patterns)
        {
            pattern.StartPattern();
        }
    }

    public int EndPhase()
    {
        int count = 0;
        foreach (var pattern in patterns)
        {
            count += pattern.EndPattern();
        }

        return count;
    }
}