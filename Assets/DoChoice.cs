using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoChoice : MonoBehaviour
{
    public DialogChoice choice;
    public TextMeshProUGUI text;

    public void ExecuteChoice()
    {
        
        if (choice != null)
            DialogParser.GetDialogParser().DoDialogChoice(choice);
            
    }

    public void SetText(string s)
    {
        text.text = s;
    }
    public void SetChocice (DialogChoice dc)
    {
        choice = dc;
    }
}
