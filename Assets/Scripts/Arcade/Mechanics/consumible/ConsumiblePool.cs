using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumiblePool : MonoBehaviour
{
    private Queue<Consumible> usedConsumibles;

    private Transform container;

    [SerializeField] private GameObject consumibleTemplate;


    private void Awake()
    {
        usedConsumibles = new Queue<Consumible>();
    }

    public Consumible GetConsumible()
    {
        Consumible consumible = FindConsumible();
        if (consumible != null)
            return consumible;
        GameObject newConsumible = Instantiate(consumibleTemplate, transform);
        Consumible consumibleComp = newConsumible.GetComponent<Consumible>();
        InsertNewConsumible(consumibleComp);
        return consumibleComp;
    }

    private Consumible FindConsumible()
    {
        int consumiblesCount = usedConsumibles.Count;
        for (int i = 0; i < consumiblesCount; i++)
        {
            Consumible consumible = usedConsumibles.Dequeue();
            usedConsumibles.Enqueue(consumible);
            if (consumible.GetIsReadyToUse())
            {
                consumible.SetIsReadyToUse(false);
                return consumible;
            }
        }

        return null;
    }

    private void InsertNewConsumible(Consumible newConsumible) =>
        usedConsumibles.Enqueue(newConsumible);
}