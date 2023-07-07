using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public int lives,totalLives;
    
    public abstract void RecieveDamage();

    public abstract void Hide();

    public abstract void Spawn(int node);
    
    public abstract bool GetEnemyDead();
    public abstract void ResetEnemy();
}
