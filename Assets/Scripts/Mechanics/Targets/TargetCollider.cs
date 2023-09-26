using UnityEngine;

public class TargetCollider : MonoBehaviour
{
    [SerializeField] private Target target;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
            target.Shot();
    }
}