using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogChoice
{
    public string ChoiceText;
    public string Link;
    public DialogValidator Validator;
}
