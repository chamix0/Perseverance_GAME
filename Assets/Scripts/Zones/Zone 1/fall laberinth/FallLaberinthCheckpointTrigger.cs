using UnityEngine;

public class FallLaberinthCheckpointTrigger : MonoBehaviour
{
    [SerializeField] private FallTriggerLaberinth _fallTriggerLaberinth;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _fallTriggerLaberinth.secondFase = true;
            enabled = false;
        }
    }
}