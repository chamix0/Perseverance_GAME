using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mechanics.General_Inputs;
using UnityEngine;

public class ConversationManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Conversation conversation;
    private GuiManager guiManager;
    private OrbitCameraController cameraController;
    private PlayerValues playerValues;
    [SerializeField] private List<Sprite> avatarSprites;

    private SoundDialogs soundDialogs;

    //previus values
    private Transform playerFocus;
    private CurrentInput prevInput;

    //values
    private char[] letters;
    private string fullText;

    //answer
    private int answerIndex;

    //write text
    private string writtenCad;
    private bool updateText;
    private int wordIndex;
    private Stopwatch timer;
    [SerializeField] private float cooldown = 100;


    private void Awake()
    {
        timer = new Stopwatch();
        timer.Start();
    }

    void Start()
    {
        soundDialogs = GetComponent<SoundDialogs>();
        guiManager = FindObjectOfType<GuiManager>();
        cameraController = FindObjectOfType<OrbitCameraController>();
        playerValues = FindObjectOfType<PlayerValues>();
    }

    // Update is called once per frame
    void Update()
    {
        if (updateText)
        {
            WriteText();
        }
    }

    public void StartConversation(Conversation conversation, Transform newFocus)
    {
        Time.timeScale = 0;
        this.conversation = conversation;
        guiManager.ShowDialog();
        playerFocus = cameraController.GetFocus();
        cameraController.ChangeFocus(newFocus);
        playerValues.StopAllMovement(true);
        prevInput = playerValues.GetCurrentInput();
        playerValues.SetCurrentInput(CurrentInput.Conversation);
        ShowDialog();
        //alice things
    }

    private void EndConversation()
    {
        Time.timeScale = 1;
        guiManager.HideDialog();
        cameraController.ChangeFocus(playerFocus);
        playerValues.ContinueAllMovement();
        playerValues.SetCurrentInput(prevInput);
    }

    private void CompleteText()
    {
        updateText = false;
        writtenCad = "";
        wordIndex = 0;
        guiManager.SetDialogMesasge(fullText);
        if (conversation.GetCurrentDialog().GetDialogType() is DialogType.Question)
        {
            guiManager.SetDialogAnswers(conversation.GetCurrentDialog().GetAnswers());
            guiManager.HighlightAnswer(answerIndex);
        }

        soundDialogs.StopTypeing();
    }

    public void ChangeAnswer()
    {
        if (conversation.GetCurrentDialog().GetDialogType() is DialogType.Question)
        {
            answerIndex = (answerIndex + 1) % 2;
            guiManager.HighlightAnswer(answerIndex);
            soundDialogs.PlayChange();
        }
    }

    public void ShowDialog()
    {
        letters = conversation.GetCurrentDialog().GetText().ToCharArray();
        fullText = conversation.GetCurrentDialog().GetText();
        updateText = true;
        answerIndex = 0;
        wordIndex = 0;
        writtenCad = "";
        guiManager.SetDialogName(conversation.GetCurrentDialog().GetName());
        if (conversation.GetCurrentDialog().GetDialogType() is DialogType.Talk)
            guiManager.SetDialogAnswers(new[] { "", "" });
        soundDialogs.PlayTypeing();
    }

    public void NextDialog()
    {
        if (updateText)
            CompleteText();
        else
        {
            if (conversation.GetCurrentDialog().GetDialogType() is DialogType.Question)
            {
                if (conversation.NextDialog(answerIndex) == null)
                    EndConversation();
                else
                {
                    soundDialogs.PlaySelect();
                    ShowDialog();
                }
            }
            else
            {
                if (conversation.NextDialog() == null)
                    EndConversation();
                else
                {
                    ShowDialog();
                }
            }
        }
    }

    private void SetAvatarImage(string nameCad)
    {
        nameCad = nameCad.ToLower();
        switch (nameCad)
        {
            case "alice":
                guiManager.SetAvatarImage(avatarSprites[0]);
                break;
            case "evil alice":
                guiManager.SetAvatarImage(avatarSprites[1]);
                break;
            default:
                guiManager.SetAvatarImage(avatarSprites[0]);
                break;
        }
    }

    private void WriteText()
    {
        if (wordIndex + 1 > letters.Length - 1)
        {
            CompleteText();
        }
        else
        {
            if (timer.Elapsed.TotalMilliseconds > cooldown)
            {
                if (letters[wordIndex].Equals('<'))
                {
                    int count = 0;
                    while (count < 2)
                    {
                        if (letters[wordIndex].Equals('>'))
                            count++;
                        writtenCad += letters[wordIndex];
                        wordIndex++;
                    }
                }

                writtenCad += letters[wordIndex];
                wordIndex++;
                guiManager.SetDialogMesasge(writtenCad);
                timer.Restart();
            }
        }
    }

    private void ShowGui()
    {
    }

    private void HideGui()
    {
    }
}