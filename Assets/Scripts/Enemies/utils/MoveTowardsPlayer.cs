using UnityEngine;

public class MoveTowardsPlayer : MonoBehaviour
{
    private PlayerValues playerValues;
    public float speed;
    public bool stopMoving;
    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopMoving)
        {
              Vector3 aux = new Vector3(playerValues.GetPos().x, transform.position.y, playerValues.GetPos().z);
                    transform.position = Vector3.MoveTowards(transform.position, aux, Time.deltaTime * speed);
        }
      
    }

    public void StopMoving()
    {
        stopMoving = true;
    }
    public void StartMoving()
    {
        stopMoving = false;
    }
}