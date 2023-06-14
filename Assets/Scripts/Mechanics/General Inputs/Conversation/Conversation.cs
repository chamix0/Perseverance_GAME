using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.YamlDotNet.Core;
using UnityEngine;
[DefaultExecutionOrder(11)]
public class Conversation : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextAsset txtConversation;
    private List<Dialog> dialogs;
    private int dialogIndex;

    private void Awake()
    {
        dialogs = new List<Dialog>();
    }

    void Start()
    {
        ParseConversation();
    }

    public void ResetConversation()
    {
        dialogIndex = 0;
    }

    public Dialog GetCurrentDialog()
    {
        return dialogs[dialogIndex];
    }

    public Dialog NextDialog()
    {
        dialogIndex = dialogs[dialogIndex].GetNextDialog();
        if (dialogIndex==-1)
        {
            return null;
        }
        return dialogs[dialogIndex];
    }

    public Dialog NextDialog(int index)
    {
        dialogIndex = dialogs[dialogIndex].GetNextDialogs()[index];
        if (dialogIndex == -1)
            return null;
        return dialogs[dialogIndex];
    }

    private void ParseConversation()
    {
        StringReader reader = new StringReader(txtConversation.text);

        string line = reader.ReadLine();
        while (line != null)
        {
            if (line.Substring(0, 1).Equals("-"))
            {
                //dialog type
                line = reader.ReadLine();
                DialogType dialogType = line.Substring(0, 1).Equals("t") ? DialogType.Talk : DialogType.Question;

                //name
                string name = reader.ReadLine();

                //message
                string message = reader.ReadLine();

                if (dialogType is DialogType.Question)
                {
                    string[] answers = new string[2];
                    int[] nextDialog = new int[2];
                    for (int i = 0; i < 2; i++)
                        answers[i] = reader.ReadLine();
                    
                    for (int i = 0; i < 2; i++)
                    {
                        line = reader.ReadLine();
                        nextDialog[i] = int.Parse(line ?? "-1");
                    }

                    dialogs.Add(new Dialog(dialogType, message, name, answers, nextDialog));
                }
                else
                {
                    int nextDialog;
                    nextDialog = int.Parse(reader.ReadLine() ?? "-1");
                    dialogs.Add(new Dialog(dialogType, message, name, nextDialog));
                }
            }

            line = reader.ReadLine();
        }
    }
}