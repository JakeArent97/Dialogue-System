using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class DialogGraphNode : Node
{
    public string GUID;

    public bool EntryPoint = false;

    public DialogGraphNode() //: base("Assets/Dialog System/NodeUI.uxml")
    {
        
    }

    //public bool StandardNode = true;
}
