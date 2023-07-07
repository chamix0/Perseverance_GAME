using UnityEngine;

public class SaveData
{
    [SerializeField] private GameData[] slots;
    [SerializeField] private int lastSesionSlotIndex;
    private const int MAX_SLOTS = 4;

    //general settings

    public SaveData()
    {
        slots = new GameData[MAX_SLOTS];
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

    #endregion
}