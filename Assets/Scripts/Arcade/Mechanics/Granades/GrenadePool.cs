using System.Collections.Generic;
using UnityEngine;

public class GrenadePool : MonoBehaviour
{
    private Queue<Grenade> usedGrenades;

    private Transform container;

    [SerializeField] private GameObject grenadeTemplate;


    private void Awake()
    {
        usedGrenades = new Queue<Grenade>();
    }

    public Grenade GetGrenade()
    {
        Grenade grenade = FindGrenade();
        if (grenade!=null)
            return grenade;
        GameObject newGrenade = Instantiate(grenadeTemplate, transform);
        Grenade grenadeComp = newGrenade.GetComponent<Grenade>();
        InsertNewGrenade(grenadeComp);
        return grenadeComp;
    }

    private Grenade FindGrenade()
    {
        int grenadesCount = usedGrenades.Count;
        for (int i = 0; i < grenadesCount; i++)
        {
            Grenade grenade = usedGrenades.Dequeue();
            usedGrenades.Enqueue(grenade);
            if (grenade.GetIsReadyToUse())
            {
                grenade.SetIsReadyToUse(false);
                return grenade;
            }
        }

        return null;
    }

    private void InsertNewGrenade(Grenade newGrenade) =>
        usedGrenades.Enqueue(newGrenade);
}
