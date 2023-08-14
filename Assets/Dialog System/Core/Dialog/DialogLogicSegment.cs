using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogLogicSegment 
{
    public enum LogicTypes { String, Integer, Boolean};
    public enum IntCompTypes { Equals, LessThan, GreaterThanOrEqual};
    [SerializeField] public LogicTypes lTypeUsed = LogicTypes.Integer;
    [SerializeField] public LogicChoice Default;
    [SerializeField] public LogicChoice b_True;
    [SerializeField] public LogicChoice b_False;
    [SerializeField] public List<LogicChoice> Choices = new List<LogicChoice>();
    [SerializeField] public ActionEffect EffectsOnEnter;
    [SerializeField] public DialogLogicBase ConditionToTest;
}
