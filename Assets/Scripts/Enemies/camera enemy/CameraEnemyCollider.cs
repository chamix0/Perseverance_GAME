using UnityEngine;

public class CameraEnemyCollider : MonoBehaviour
{
    private CameraEnemy cameraEnemy;

    private void Start()
    {
        cameraEnemy = GetComponentInParent<CameraEnemy>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            cameraEnemy.RecieveDamage(1);
        }
    }
}
