using System.Collections;
using System.Collections.Generic;
using Player.Observer_pattern;
using UnityEngine;

public class FinalBoss : Enemy,IObserver
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void RecieveDamage()
    {
        throw new System.NotImplementedException();
    }

    public override void Hide()
    {
        throw new System.NotImplementedException();
    }

    public override void Spawn(int node)
    {
        throw new System.NotImplementedException();
    }

    public override bool GetEnemyDead()
    {
        throw new System.NotImplementedException();
    }

    public override void ResetEnemy()
    {
        throw new System.NotImplementedException();
    }

    public void OnNotify(PlayerActions playerAction)
    {
        throw new System.NotImplementedException();
    }
}
