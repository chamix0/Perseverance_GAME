using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data structure based on a Queue which is in charge of storing and manging the messages received by the process 
/// </summary>
[DefaultExecutionOrder(0)]
public class MovesQueue : MonoBehaviour
{
    private Queue<Move> messages;
    public Move lastMove;

    // Start is called before the first frame update
    private void Awake()
    {
        messages = new Queue<Move>();
    }

    public bool HasMessages()
    {
        return messages.Count > 0;
    }

    /// <summary>
    /// add message to the queue
    /// </summary>
    /// <param name="msg"></param>
    public void EnqueueMsg(string msg) => messages.Enqueue(new Move(msg));

    public void Enqueue(Move move)
    {
        messages.Enqueue(move);
    }

    /// <summary>
    /// take out the next message on the queue
    /// </summary>
    /// <returns>MOVE in next message on the queue</returns>
    public Move Dequeue()
    {
        return messages.Dequeue();
    }

    /// <summary>
    /// see which is the next message without dequeueing
    /// </summary>
    /// <returns>MOVE next message</returns>
    public Move Peek()
    {
        return messages.Peek();
    }
}