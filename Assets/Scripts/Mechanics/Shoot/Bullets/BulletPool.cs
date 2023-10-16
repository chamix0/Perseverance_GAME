using System;
using System.Collections.Generic;
using Mechanics.Shoot.Bullets;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    private Queue<Bullet> usedBullets,
        usedFreezeBullets,
        usedBurnBullets,
        usedGuidedBullets,
        usedInstaKillBullets,
        usedExplosiveBullets;

    private Transform container;

    [SerializeField] private GameObject bulletTemplate,
        bulletTemplateFreeze,
        bulletTemplateBurn,
        bulletTemplateGuided,
        bulletTemplateInstakill,
        bulletTemplateExplosive;


    private void Awake()
    {
        usedBullets = new Queue<Bullet>();
        usedBurnBullets = new Queue<Bullet>();
        usedFreezeBullets = new Queue<Bullet>();
        usedGuidedBullets = new Queue<Bullet>();
        usedInstaKillBullets = new Queue<Bullet>();
        usedExplosiveBullets = new Queue<Bullet>();
    }

    private void Start()
    {
        transform.parent = null;
    }


    public Bullet GetBullet()
    {
        Bullet bullet = FindBullet(usedBullets);
        if (bullet!=null)
            return bullet;
        GameObject newBullet = Instantiate(bulletTemplate, transform);
        Bullet bulletComp = newBullet.GetComponent<Bullet>();
        InsertNewBullet(BulletType.NormalBullet, bulletComp);
        return bulletComp;
    }

    public Bullet GetBullet(BulletType type)
    {
        Bullet bullet = type switch
        {
            BulletType.FreezeBullet => FindBullet(usedFreezeBullets),
            BulletType.BurnBullet => FindBullet(usedBurnBullets),
            BulletType.GuidedBullet => FindBullet(usedGuidedBullets),
            BulletType.InstaKillBullet => FindBullet(usedInstaKillBullets),
            BulletType.NormalBullet => FindBullet(usedBullets),
            BulletType.ExplosiveBullet => FindBullet(usedExplosiveBullets),
            _ => FindBullet(usedBullets)
        };

        if (bullet!=null)
            return bullet;

        GameObject newBullet = type switch
        {
            BulletType.FreezeBullet => Instantiate(bulletTemplateFreeze, transform),
            BulletType.BurnBullet => Instantiate(bulletTemplateBurn, transform),
            BulletType.GuidedBullet => Instantiate(bulletTemplateGuided, transform),
            BulletType.InstaKillBullet => Instantiate(bulletTemplateInstakill, transform),
            BulletType.NormalBullet => Instantiate(bulletTemplate, transform),
            BulletType.ExplosiveBullet => Instantiate(bulletTemplateExplosive, transform),
            _ => Instantiate(bulletTemplateExplosive, transform)
        };
        Bullet bulletComp = newBullet.GetComponent<Bullet>();
        InsertNewBullet(type, bulletComp);
        return bulletComp;
    }

    private Bullet FindBullet(Queue<Bullet> usedBullets)
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

        return null;
    }

    private void InsertNewBullet(BulletType type, Bullet newBullet) =>
        GetBulletTypeQueue(type).Enqueue(newBullet);

    private Queue<Bullet> GetBulletTypeQueue(BulletType type)
    {
        Queue<Bullet> queue = type switch
        {
            BulletType.FreezeBullet => usedFreezeBullets,
            BulletType.BurnBullet => usedBurnBullets,
            BulletType.GuidedBullet => usedGuidedBullets,
            BulletType.InstaKillBullet => usedInstaKillBullets,
            BulletType.NormalBullet => usedBullets,
            BulletType.ExplosiveBullet => usedExplosiveBullets
        };
        return queue;
    }
}