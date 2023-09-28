using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Score : IComparable<Score>
{
    [FormerlySerializedAs("name")] [SerializeField] private string nameP;
    [SerializeField] private int round, points;

    public Score(string nameP, int round, int points)
    {
        this.nameP = nameP;
        this.round = round;
        this.points = points;
    }

    public string GetName()
    {
        return nameP;
    }

    public int GetRound()
    {
        return round;
    }

    public int GetPoints()
    {
        return points;
    }

    public int CompareTo(Score other)
    {
        if (points < other.points)
            return 1;

        if (points > other.points)
            return -1;

        if (round < other.round)
            return 1;

        if (round > other.round)
            return -1;
        return 0;
    }

    public override string ToString()
    {
        return GetName() + "\t" + round + "\t" + points + "pts";
    }
}