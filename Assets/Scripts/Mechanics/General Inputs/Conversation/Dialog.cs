using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogType
{
    Talk,
    Question
}

public class Dialog
{
    private DialogType dialogType;
    private string name;
    private string text;
    private string[] answers;
    private int next;
    private int[] nexts;

    //normal dialog
    public Dialog(DialogType dialogType, string text, string name, int next)
    {
        this.dialogType = dialogType;
        this.text = text;
        this.name = name;
        this.next = next;
    }

    //question
    public Dialog(DialogType dialogType, string question, string name, string[] answers, int[] nexts)
    {
        this.dialogType = dialogType;
        text = question;
        this.name = name;
        this.answers = answers;
        this.nexts = nexts;
    }

    public DialogType GetDialogType()
    {
        return dialogType;
    }
    public string GetName()
    {
        return name;
    }

    public string GetText()
    {
        return text;
    }

    public string[] GetAnswers()
    {
        return answers;
    }

    public int GetNextDialog()
    {
        return next;
    }
    public int[] GetNextDialogs()
    {
        return nexts;
    }
    public override string ToString()
    {
        string cad = "";
        switch (dialogType)
        {
            case DialogType.Question:
                cad += "Type: Question \n";
                cad += "Name: " + name + "\n";
                cad += "Question: " + text + "\n";
                for (int i = 0; i < 2; i++)
                {
                    cad += "Answer " + i + " : " + answers[i] + "\n";
                    cad += "Next question " + i + " : " + nexts[i] + "\n";
                }

                break;
            case DialogType.Talk:
                cad += "Type: Text \n";
                cad += "Name: " + name + "\n";
                cad += "Text: " + text + "\n";
                cad += "Answer " + " : " + next + "\n";

                break;
        }

        return cad;
    }
}