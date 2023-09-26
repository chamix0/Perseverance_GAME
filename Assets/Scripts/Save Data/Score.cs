using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Score : IComparer<Score>
{
    [SerializeField] private string name;
    [SerializeField] private int round, points;

    public Score(string name, int round, int points)
    {
        this.name = name;
        this.round = round;
        this.points = points;
    }

    public string GetName()
    {
        return name;
    }

    public int GetRound()
    {
        return round;
    }

    public int GetPoints()
    {
        return points;
    }


    public int Compare(Score a, Score b)
    {
        if (a.points < b.points)
            return -1;

        if (a.points > b.points)
            return 1;

        if (a.round < b.round)
            return -1;

        if (a.round > b.round)
            return 1;
        return 0;
    }
}