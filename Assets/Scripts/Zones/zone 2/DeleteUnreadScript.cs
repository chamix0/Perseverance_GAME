using UnityEngine;

public class DeleteUnreadScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject dialog;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(dialog);
        }
    }
}