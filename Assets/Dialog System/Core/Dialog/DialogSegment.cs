using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogSegment
{
    public string Intro;
    public bool hasChoices;
    [SerializeField]private string NextDialogID;
    [SerializeField] public List<DialogChoice> Choices = new List<DialogChoice>();
    [SerializeField] public ActionEffect EffectsOnEnter;
    public string Speaker;
    public Color SpeakerColor;

    public string GetIntro()
    {
        return Intro;
    }

    public void SetNextDialog(string dsID)
    {
        NextDialogID = dsID;
    }

    public string GetNextDialog()
    {
        return NextDialogID;
    }

    public List<DialogChoice> GetDialogChoices()
    {
        return Choices;
    }
}

