using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LogicChoice
{
    public string ChoiceText;
    public string Link;
    public DialogLogicSegment.LogicTypes l_Compare = DialogLogicSegment.LogicTypes.Integer;
    public DialogLogicSegment.IntCompTypes i_Compare = DialogLogicSegment.IntCompTypes.Equals;
}
