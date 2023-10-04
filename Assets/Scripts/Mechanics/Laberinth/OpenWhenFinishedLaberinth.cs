using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(10)]
public class OpenWhenFinishedLaberinth : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private LaberinthManager laberinthManager;
    [SerializeField] private GameObject trigger;
    private bool finished;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!finished && laberinthManager.GetMissingTerminals() <= 0)
        {
            trigger.SetActive(true);
        }
    }
}