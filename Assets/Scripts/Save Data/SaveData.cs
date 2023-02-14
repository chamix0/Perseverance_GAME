using UnityEngine;

public class SaveData
{
    [SerializeField] private GameData[] slots;
    [SerializeField] private int lastSesionSlotIndex;
    private const int MAX_SLOTS = 4;

    //general settings

    public SaveData()
    {
        slots = new GameData[] { null, null, null, null };
        lastSesionSlotIndex = -1;
    }

    #region Methods

    public bool StartNewGame()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = new GameData();
                lastSesionSlotIndex = i;
                return true;
            }
        }

        return false;
    }

    public void EraseSlot(int index)
    {
        slots[index] = null;
        bool empty = true;
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            if (slots[i] != null)
                empty = false;
        }

        if (empty)
            lastSesionSlotIndex = -1;
    }

    public bool AreThereEmptySlots()
    {
        foreach (var slot in slots)
            if (slot == null)
                return true;
        return false;
    }

    public GameData GetGameData(int index)
    {
        return slots[index];
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