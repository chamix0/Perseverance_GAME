using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Arcade.Mechanics.Bullets;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class DecallPool : MonoBehaviour
{
    private Queue<Decal> usedDeacal;

    private Transform container;

    [SerializeField] private GameObject decalTemplate;


    private void Awake()
    {
        usedDeacal = new Queue<Decal>();
    }


    public Decal GetDecal()
    {
        Decal decal = FindDecall();
        if (decal!=null)
            return decal;
        GameObject newDecal = Instantiate(decalTemplate, transform);
        Decal decalComp = newDecal.GetComponent<Decal>();
        InsertNewDecal(decalComp);
        return decalComp;
    }

    private Decal FindDecall()
    {
        int bulletCount = usedDeacal.Count;
        for (int i = 0; i < bulletCount; i++)
        {
            Decal decal = usedDeacal.Dequeue();
            usedDeacal.Enqueue(decal);
            if (decal.GetIsReadyToUse())
            {
                decal.SetIsReadyToUse(false);
                return decal;
            }
        }

        return null;
    }

    private void InsertNewDecal(Decal newDecal) =>
        usedDeacal.Enqueue(newDecal);
}