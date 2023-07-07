using UnityEngine;

public class ObjetiveTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    private Objetives objetives;
    [SerializeField] private ObetiveType objetiveType;
    [SerializeField] private string text;

    void Start()
    {
        objetives = FindObjectOfType<Objetives>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (objetiveType is ObetiveType.Add)
                objetives.SetNewObjetive(text);
            else if (objetiveType is ObetiveType.Remove)
                objetives.RemoveObjetive();

            Destroy(gameObject);
        }
    }
}