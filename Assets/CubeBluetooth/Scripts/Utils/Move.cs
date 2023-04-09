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

    public static FACES GetRandomFace(int diff)
    {
        int aux = Random.Range(0, diff + 3);

        if (aux == 0)
            return FACES.R;
        if (aux == 1)
            return FACES.U;
        if (aux == 2)
            return FACES.F;
        if (aux == 3)
            return FACES.L;
        if (aux == 4)
            return FACES.D;
        return FACES.B;
    }

    public string ToString()
    {
        if (direction == -1)
            return face + "'";
        if (direction == 1)
            return face + "";
        return "";
    }

    public bool IsOppositeFace(Move face2)
    {
        switch (face)
        {
            case FACES.L:
                return face2.face == FACES.R;
            case FACES.R:
                return face2.face == FACES.L;
            case FACES.U:
                return face2.face == FACES.D;
            case FACES.D:
                return face2.face == FACES.U;
            case FACES.F:
                return face2.face == FACES.B;
            case FACES.B:
                return face2.face == FACES.F;
            default:
                return false;
        }
    }

    public bool IsSameDirection(Move move2)
    {
        return direction == move2.direction;
    }

    public bool IsMiddleLayer(Move move2)
    {
        return IsOppositeFace(move2) && !IsSameDirection(move2);
    }

    public bool Equals(Move move2)
    {
        return face == move2.face && direction == move2.direction;
    }
}