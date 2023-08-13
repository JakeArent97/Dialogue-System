using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class DialogGraphNode : Node
{
    public string GUID;

    public DialogSegment dialogSegment;

    public bool EntryPoint = false;

    public Toggle choicesBool;
}
