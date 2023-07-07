using UnityEngine;

public class StartBeeFightTrigger : MonoBehaviour
{
    // Start is called before the first frame update
   [SerializeField] private BeeFight _beeFight;

   
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _beeFight.StartBattle();
        }
    }

   

    // Update is called once per frame
    void Update()
    {
        
    }
}
