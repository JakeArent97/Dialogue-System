using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CustomEdgeListener : IEdgeConnectorListener
{
    public void OnDrop(GraphView graphView, Edge edge)
    {
        DialogGraphView dgv = graphView as DialogGraphView;
        dgv.SaveOnLink();
    }

    public void OnDropOutsidePort(Edge edge, Vector2 position)
    {
        //Do nothing
    }
}
