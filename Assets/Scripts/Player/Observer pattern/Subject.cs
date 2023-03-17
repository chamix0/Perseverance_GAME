using System;
using System.Collections;
using System.Collections.Generic;
using Player.Observer_pattern;
using UnityEngine;

public abstract class Subject : MonoBehaviour
{
    private List<IObserver> _observers = new List<IObserver>();

    public void AddObserver(IObserver observer)
    {
        _observers.Add(observer);
    }

    public void RemoveOverserver(IObserver observer)
    {
        _observers.Remove(observer);
    }

    protected void NotifyObservers(PlayerActions playerAction)
    {
        _observers.ForEach((_observer) => { _observer.OnNotify(playerAction); });
    }
}