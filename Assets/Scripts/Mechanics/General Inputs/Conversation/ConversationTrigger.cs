using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public enum ObetiveType
{
    Add,
    Remove,
    None
}

public class ConversationTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Conversation conversation;
    [SerializeField] private Transform focus;
    private ConversationManager conversationManager;
    private Stopwatch stopwatch;
    private float conversationCooldown = 2;
    public bool destroyAfterUse = false;
    public bool showAlice = false;
    [SerializeField] private GameObject alice;
    private PlayerValues playerValues;
    private Objetives objetives;
    [SerializeField] private ObetiveType objetiveType;
    [SerializeField] private string text;

    private void Awake()
    {
        stopwatch = new Stopwatch();
    }

    void Start()
    {
        playerValues = FindObjectOfType<PlayerValues>();
        conversationManager = FindObjectOfType<ConversationManager>();
        objetives = FindObjectOfType<Objetives>();
        alice.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (stopwatch.Elapsed.TotalSeconds > conversationCooldown || !stopwatch.IsRunning)
            {
                if (showAlice)
                {
                    alice.SetActive(true);
                }

                playerValues.transform.LookAt(new Vector3(focus.position.x, playerValues.transform.position.y,
                    focus.position.z));

                conversation.ResetConversation();
                conversationManager.StartConversation(conversation, focus);
                if (!stopwatch.IsRunning)
                    stopwatch.Start();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (objetiveType is ObetiveType.Add)
                objetives.SetNewObjetive(text);
            else if (objetiveType is ObetiveType.Remove)
                objetives.RemoveObjetive();

            if (showAlice)
            {
                alice.SetActive(false);
            }

            if (destroyAfterUse)
            {
                gameObject.SetActive(false);
            }
            else
            {
                if (!stopwatch.IsRunning)
                    stopwatch.Start();
                else
                    stopwatch.Restart();
            }
        }
    }


    // Update is called once per frame
}