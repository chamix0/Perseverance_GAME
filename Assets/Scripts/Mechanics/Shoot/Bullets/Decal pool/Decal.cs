using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mechanics.Shoot.Bullets;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(DecalProjector))]
public class Decal : MonoBehaviour
{
   [SerializeField] private DecalProjector _decalProjector;
    private Stopwatch timer;
    private int decalCooldown = 5;
    private bool readyToUse = true;
    private bool isFading;
    private float opacity = 1;
    private static readonly int Opacity = Shader.PropertyToID("_Opacity");
    [SerializeField] private List<Material> _materials;
    private Dictionary<BulletType, Material> decalMaterials;

    private void Awake()
    {
        timer = new Stopwatch();
        decalMaterials = new Dictionary<BulletType, Material>();
        decalMaterials.Add(BulletType.NormalBullet, _materials[0]);
        decalMaterials.Add(BulletType.FreezeBullet, _materials[1]);
        decalMaterials.Add(BulletType.BurnBullet, _materials[2]);
        decalMaterials.Add(BulletType.GuidedBullet, _materials[3]);
        decalMaterials.Add(BulletType.InstaKillBullet, _materials[4]);
        decalMaterials.Add(BulletType.ExplosiveBullet, _materials[5]);
    }

    private void Start()
    {
        // _decalProjector = GetComponent<DecalProjector>();
    }

    void Update()
    {
        if (timer.Elapsed.TotalSeconds > decalCooldown)
        {
            timer.Stop();
            timer.Reset();
            StartFade();
        }

        if (isFading)
        {
            Fade();
        }
    }

    public void UseDecal(Vector3 position, Vector3 direction, BulletType bulletType)
    {
        readyToUse = false;
        timer.Start();
        SetMaterial(bulletType);
        SetOpacity(1f);

        transform.position = position;
        transform.forward = direction;
        transform.localPosition += new Vector3(0, 0, 0.1f);
    }

    private void SetMaterial(BulletType bulletType)
    {
        _decalProjector.material = decalMaterials[bulletType];
    }

    private void SetOpacity(float value)
    {
        opacity = value;
        _decalProjector.fadeFactor = opacity;
    }

    private void StartFade()
    {
        isFading = true;
    }

    private void Fade()
    {
        if (opacity <= 0)
        {
            opacity = 0;
            isFading = false;
            readyToUse = true;
        }

        SetOpacity(opacity - 0.01f);
    }

    public bool GetIsReadyToUse()
    {
        return readyToUse;
    }

    public void SetIsReadyToUse(bool val)
    {
        readyToUse = val;
    }
}