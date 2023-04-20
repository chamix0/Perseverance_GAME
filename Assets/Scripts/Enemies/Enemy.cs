using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public int lives;
    
    public abstract void RecieveDamage();

    public abstract void Die();
}
