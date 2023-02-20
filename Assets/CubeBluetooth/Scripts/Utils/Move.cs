using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
        face = FACES.NULL;
        color = Color.clear;
        direction = 0;
        time = DateTime.Now.TimeOfDay;
    }

    public bool Equals(Move move2)
    {
        return face == move2.face && direction == move2.direction;
    }
}