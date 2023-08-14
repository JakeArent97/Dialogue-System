using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogLogicSegment 
{
    [SerializeField] public DialogChoice Default;
    [SerializeField] public List<DialogChoice> Choices = new List<DialogChoice>();
    [SerializeField] public ActionEffect EffectsOnEnter;
}
