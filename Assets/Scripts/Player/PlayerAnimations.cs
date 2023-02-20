using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator _animator;
    private bool updateGearAnim;
    private float gearAnim, gearAnimTarget, _tG;

    // Start is called before the first frame update
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (updateGearAnim)
            _animator.SetFloat("Gear", UpdateGearValAnim());
    }

    #region animation

    private float UpdateGearValAnim()
    {
        gearAnim = Mathf.Lerp(gearAnim, gearAnimTarget, _tG);
        _tG += 0.5f * Time.deltaTime;

        if (_tG > 1.0f)
        {
            _tG = 1.0f;
            updateGearAnim = false;
        }

        return gearAnim;
    }

    public void SetSitAnim(bool val)
    {
        _animator.SetBool("Sit", val);
    }


    public void ChangeGearAnim(int oldGear, int newGear)
    {
        updateGearAnim = true;
        _tG = 0.0f;
        gearAnim = oldGear;
        gearAnimTarget = newGear;
    }

    #endregion
}