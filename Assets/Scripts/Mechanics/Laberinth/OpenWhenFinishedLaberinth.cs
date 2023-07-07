using System.Collections;
using UnityEngine;
[DefaultExecutionOrder(10)]
public class OpenWhenFinishedLaberinth : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private LaberinthManager laberinthManager;
    [SerializeField] private GameObject trigger;

    void Start()
    {
        StartCoroutine(EnableTriggerCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator EnableTriggerCoroutine()
    {
        yield return new WaitUntil(() => laberinthManager.GetMissingTerminals() == 0);
        trigger.SetActive(true);
    }
}