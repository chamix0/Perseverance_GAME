using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowMovements : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text text;
    [SerializeField] private MovesQueue _movesQueue;
    private int sameMoveCounter = 1;
    private Move lastMove = null;

    // Update is called once per frame
    void Update()
    {
        if (_movesQueue.HasMessages())
        {
            Move move = _movesQueue.Dequeue();
            sameMoveCounter = lastMove != null && move.Equals(lastMove)
                ? sameMoveCounter + 1
                : 1;
            text.text = "" + move.face + move.direction * sameMoveCounter;
            lastMove = move;
        }
    }
}