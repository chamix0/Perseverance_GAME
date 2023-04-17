using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    private Queue<Bullet> usedBullets;
    private Transform container;
    [SerializeField] private GameObject bulletTemplate;

    private void Awake()
    {
        usedBullets = new Queue<Bullet>();
    }

    private void Start()
    {
        transform.parent = null;
    }


    public Bullet GetBullet()
    {
        int bulletCount = usedBullets.Count;

        for (int i = 0; i < bulletCount; i++)
        {
            Bullet bullet = usedBullets.Dequeue();
            usedBullets.Enqueue(bullet);
            if (bullet.IsReadyToUse())
            {
                bullet.SetIsReadyToUse(false);
                return bullet;
            }
        }

        GameObject newBullet = Instantiate(bulletTemplate, transform);
        Bullet bulletComp = newBullet.GetComponent<Bullet>();
        usedBullets.Enqueue(bulletComp);
        return bulletComp;
    }
}