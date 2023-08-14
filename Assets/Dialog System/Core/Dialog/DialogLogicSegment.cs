using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogLogicSegment 
{
    public enum LogicTypes { String, Integer, Boolean};
    [SerializeField] public DialogChoice Default;
    [SerializeField] public List<DialogChoice> Choices = new List<DialogChoice>();
    [SerializeField] public ActionEffect EffectsOnEnter;
    [SerializeField] public DialogLogicBase ConditionToTest;
}
