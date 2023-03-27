using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    private List<Bullet> usedBullets;
    private Transform container;
    [SerializeField] private GameObject bulletTemplate;

    public BulletPool()
    {
        usedBullets = new List<Bullet>();
    }

    public Bullet GetBullet()
    {
        foreach (var bullet in usedBullets)
        {
            if (bullet.IsReadyToUse())
                return bullet;
        }

        GameObject newBullet = Instantiate(bulletTemplate, this.transform);
        Bullet bulletComp = newBullet.GetComponent<Bullet>();
        usedBullets.Add(bulletComp);
        return bulletComp;
    }
}