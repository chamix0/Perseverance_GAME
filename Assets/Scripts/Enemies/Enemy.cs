using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public int lives, totalLives;
    public bool isDead;
    public abstract void RecieveDamage(int damage);

    public abstract void RecieveDamage(int damage, int points);


    public abstract void Hide();

    public abstract void Spawn(int node);

    public abstract bool GetEnemyDead();
    public abstract void ResetEnemy();
    public abstract void ResetEnemy(int maxLivesAux, float speed, int damage, Vector3 pos);
    public abstract void HitSlow();
    public abstract void Freeze();
    public abstract void Burn();
    public abstract void GrenadeFreezeDamage();
    public abstract void GrenadeDamage();
    public abstract void GrenadeSmoke(Vector3 decoyPos, float time);

    public abstract void RecieveLaserDamage();
}