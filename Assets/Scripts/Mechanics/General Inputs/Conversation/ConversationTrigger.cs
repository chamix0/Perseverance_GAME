using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

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

    private void Awake()
    {
        stopwatch = new Stopwatch();
    }

    void Start()
    {
        conversationManager = FindObjectOfType<ConversationManager>();
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
            if (showAlice)
            {
                alice.SetActive(false);
            }

            if (destroyAfterUse)
            {
                Destroy(gameObject);
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