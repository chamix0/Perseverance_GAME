using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    [SerializeField] private GameData[] slots;
    [SerializeField] private int lastSesionSlotIndex;
    [SerializeField] private Score[] leaderBoard;
    private const int MAX_SLOTS = 4;

    //general settings

    public SaveData()
    {
        slots = new GameData[MAX_SLOTS];
        leaderBoard = new Score[10];
        lastSesionSlotIndex = -1;
    }

    #region Methods

    public bool StartNewGame(int model, string name)
    {
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            if (!slots[i].GetGameStarted())
            {
                slots[i] = new GameData(model, name);
                lastSesionSlotIndex = i;
                return true;
            }
        }

        return false;
    }

    public void EraseSlot(int index)
    {
        slots[index] = new GameData();
        bool empty = true;
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            if (slots[i].GetGameStarted())
                empty = false;
        }

        if (empty)
            lastSesionSlotIndex = -1;
    }

    public bool AreThereEmptySlots()
    {
        foreach (var slot in slots)
            if (!slot.GetGameStarted())
                return true;
        return false;
    }

    public GameData LoadGameData(int index)
    {
        lastSesionSlotIndex = index;
        return slots[index];
    }

    public GameData GetGameData(int index)
    {
        if (index != -1)
        {
            return slots[index];
        }

        return new GameData();
    }

    public int GetLastSessionSlotIndex()
    {
        return lastSesionSlotIndex;
    }

    public void SetLastSessionSlotIndex(int value)
    {
        lastSesionSlotIndex = value;
    }

    private void InitLeaderBoard()
    {
        for (int i = 0; i < leaderBoard.Length; i++)
        {
            leaderBoard[0] = new Score("-", 0, 0);
        }
    }

    public void AddScoreToSortedLeaderboard(Score score)
    {
        Score[] aux = new Score[11];
        Array.Copy(leaderBoard, aux, 10);
        aux[10] = score;
        Array.Sort(aux);
        Array.Copy(aux, leaderBoard, 10);
    }

    public Score[] GetLeaderBoard()
    {
        return leaderBoard;
    }

    #endregion
}