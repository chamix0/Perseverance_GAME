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
        if (collision.gameObject.layer==8)
        {
            cameraEnemy.RecieveDamage();
        }
    }
}
