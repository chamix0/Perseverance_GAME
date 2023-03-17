using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Move
{
    public string msg;
    public TimeSpan time;
    public int direction;
    public FACES face;
    public Color color;

    public Move(string value)
    {
        msg = value;
        face = FACES.Null;
        color = Color.clear;
        direction = 0;
        time = DateTime.Now.TimeOfDay;
    }

    public Move(FACES faceVal, int directionVal)
    {
        msg = "";
        face = faceVal;
        color = Color.clear;
        direction = directionVal;
        time = DateTime.Now.TimeOfDay;
    }

    public static FACES GetRandomFace()
    {
        int aux = Random.Range(0, 6);

        if (aux == 0)
            return FACES.B;
        if (aux == 1)
            return FACES.F;
        if (aux == 2)
            return FACES.U;
        if (aux == 3)
            return FACES.D;
        if (aux == 4)
            return FACES.L;
        return FACES.R;
    }

    public string ToString()
    {
        if (direction == -1)
            return face + "'";
        if (direction == 1)
            return face + "";
        return "";
    }

    public bool Equals(Move move2)
    {
        return face == move2.face && direction == move2.direction;
    }
}