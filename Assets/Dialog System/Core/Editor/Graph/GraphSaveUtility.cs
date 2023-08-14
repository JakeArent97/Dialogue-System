using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
    private DialogGraphView targetGraph;
    private DialogGraphAsset cachedContainer;

    private List<Edge> edges => targetGraph.edges.ToList();
    private List<DialogGraphNode> nodes => targetGraph.nodes.ToList().Cast<DialogGraphNode>().ToList();

    public static GraphSaveUtility GetInstance(DialogGraphView targetView)
    {
        return new GraphSaveUtility
        {
            targetGraph = targetView
        };
    }

    public void SaveGraph(string fileName)
    {
        if (!edges.Any())
            return;
        DialogGraphAsset asset = ScriptableObject.CreateInstance<DialogGraphAsset>();
        string filePath = "Assets/Dialog System/Dialogs/" + fileName + ".asset";
        bool updating = false;
        if (AssetDatabase.LoadAssetAtPath(filePath, typeof(DialogGraphAsset)) != null)
        {
            updating = true;
            asset = (DialogGraphAsset)AssetDatabase.LoadAssetAtPath(filePath, typeof(DialogGraphAsset));
        }
        asset.Links.Clear();
        var connectedPorts = edges.Where(x => x.input.node != null).ToArray();
        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as DialogGraphNode;
            var inputNode = connectedPorts[i].input.node as DialogGraphNode;

            asset.Links.Add(new NodeLinkData()
            {
                BaseNodeGUID = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                PortNum = outputNode.outputContainer.IndexOf(connectedPorts[i].output),
                TargetNodeGUID = inputNode.GUID
            });
        }

        asset.Nodes.Clear();
        foreach (DialogGraphNode n in nodes)
        {
            
            if (n.EntryPoint)
            {
                asset.EntryGUID = n.GUID;
            }
            else
            {
                if (n is DialogGraphStandardNode)
                {
                    DialogGraphStandardNode stan = n as DialogGraphStandardNode;

                    stan.dialogSegment.Choices.Clear();
                    foreach (VisualElement child in n.outputContainer.Children())
                    {
                        Port childPort = (Port)child;
                        DialogChoice dc = new DialogChoice();
                        dc.ChoiceText = childPort.portName;
                        if (childPort.connected)
                        {
                            NodeLinkData link = asset.Links.First(x => x.BaseNodeGUID == n.GUID && x.PortName == childPort.portName);
                            DialogGraphNode linkedNode = nodes.First(x => x.GUID == link.TargetNodeGUID);
                            dc.Link = linkedNode.GUID;
                        }
                        //Get Validators
                        foreach (VisualElement v in child.contentContainer.Children())
                        {
                            if (v.GetType() == typeof(ObjectField))
                            {
                                ObjectField field = (ObjectField)v;
                                if (field.objectType == typeof(DialogValidator) && field.value != null)
                                {
                                    dc.Validator = (DialogValidator)field.value;
                                }
                            }
                        }
                        stan.dialogSegment.Choices.Add(dc);
                    }


                    NodeData newNodeData = new NodeData()
                    {
                        GUID = n.GUID,
                        StandardNode = true,
                        seg = stan.dialogSegment,
                        position = n.GetPosition().position
                    };
                    asset.Nodes.Add(newNodeData);
                }
                else if (n is DialogGraphLogicNode)
                {
                    DialogGraphLogicNode stan = n as DialogGraphLogicNode;

                    stan.logicSegment.Choices.Clear();
                    foreach (VisualElement child in n.outputContainer.Children())
                    {
                        Port childPort = (Port)child;
                        DialogChoice dc = new DialogChoice();
                        dc.ChoiceText = childPort.portName;
                        if (childPort.connected)
                        {
                            NodeLinkData link = asset.Links.First(x => x.BaseNodeGUID == n.GUID && x.PortName == childPort.portName);
                            DialogGraphNode linkedNode = nodes.First(x => x.GUID == link.TargetNodeGUID);
                            dc.Link = linkedNode.GUID;
                        }
                        //Get Validators
                        foreach (VisualElement v in child.contentContainer.Children())
                        {
                            if (v.GetType() == typeof(ObjectField))
                            {
                                ObjectField field = (ObjectField)v;
                                if (field.objectType == typeof(DialogValidator) && field.value != null)
                                {
                                    dc.Validator = (DialogValidator)field.value;
                                }
                            }
                        }
                        stan.logicSegment.Choices.Add(dc);
                    }


                    NodeData newNodeData = new NodeData()
                    {
                        GUID = n.GUID,
                        StandardNode = true,
                        lSeg = stan.logicSegment,
                        position = n.GetPosition().position
                    };
                    asset.Nodes.Add(newNodeData);
                }
            }
            
        }

        if (updating)
        {
            EditorUtility.SetDirty(asset);
        }
        else
            AssetDatabase.CreateAsset(asset, filePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public void LoadData (string fileName)
    {
        cachedContainer = Resources.Load<DialogGraphAsset>(fileName);
        cachedContainer = AssetDatabase.LoadAssetAtPath<DialogGraphAsset>(fileName);
        if (cachedContainer == null)
        {
            EditorUtility.DisplayDialog("Invalid File", "File name not found", "Ok");
        }
        else
        {
            ClearGraph();
            CreateNodes();
            ConnectNodes();
        }
    }

    private void ConnectNodes()
    {
        /*
        for (int i = 0; i < nodes.Count; i++)
        {
            var conenctions = cachedContainer.Links.Where(x => x.BaseNodeGUID == nodes[i].GUID).ToList();
            for (int j = 0; j < conenctions.Count;j++)
            {
                string targetNodeGuid = conenctions[j].TargetNodeGUID;
                DialogGraphNode targetNode = nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

                targetNode.SetPosition(new Rect(
                    cachedContainer.Nodes.First(x => x.GUID == targetNodeGuid).position,
                    DialogGraphView.defNodeSize
                    ));
            }
        }
        */
        foreach (var link in cachedContainer.Links)
        {
            Port inPort = null, outPort = null;
            foreach (var node in nodes)
            {
                if (node.GUID == link.BaseNodeGUID)
                    outPort = node.outputContainer[link.PortNum].Q<Port>();
                if (node.GUID == link.TargetNodeGUID)
                    inPort = (Port)node.inputContainer[0];
            }
            LinkNodes(outPort, inPort);
        }
    }

    void LinkNodes(Port outputPort, Port inputPort)
    {
        var tempEdge = new Edge
        {
            output = outputPort,
            input = inputPort
        };
        tempEdge?.input.Connect(tempEdge);
        tempEdge?.output.Connect(tempEdge);
        targetGraph.Add(tempEdge);
    }

    private void CreateNodes()
    {
        foreach (NodeData nd in cachedContainer.Nodes)
        {
            //load node
            if (nd.StandardNode)
            {
                //Standard Node
                DialogGraphStandardNode tempNode = targetGraph.CreateDialogNode(nd.seg);
                tempNode.GUID = nd.GUID;
                targetGraph.AddElement(tempNode);

                //Ports
                foreach (DialogChoice dc in nd.seg.Choices)
                {
                    DialogValidator dv = null;
                    if (dc.Validator != null)
                        dv = dc.Validator;
                    targetGraph.AddChoicePort(tempNode, dc.ChoiceText, dv, true);
                }
                //Set Position
                tempNode.SetPosition(new Rect(nd.position, DialogGraphView.defNodeSize));
            }
            else
            {
                //Logic Node
                DialogGraphLogicNode tempNode = targetGraph.CreateDialogNode(nd.lSeg);
                tempNode.GUID = nd.GUID;
                targetGraph.AddElement(tempNode);

                //Ports
                foreach (DialogChoice dc in nd.seg.Choices)
                {
                    DialogValidator dv = null;
                    if (dc.Validator != null)
                        dv = dc.Validator;
                    targetGraph.AddChoicePort(tempNode, dc.ChoiceText, dv, true);
                }
                //Set Position
                tempNode.SetPosition(new Rect(nd.position, DialogGraphView.defNodeSize));
            }
            
        }
    }

    private void ClearGraph()
    {
        nodes.Find(x => x.EntryPoint).GUID = cachedContainer.EntryGUID;
        foreach (var n in nodes)
        {
            if (n.EntryPoint)
                continue;
            //remove all edges
            edges.Where(x => x.input.node == n).ToList().ForEach(edge => targetGraph.RemoveElement(edge));
            //remove node
            targetGraph.RemoveElement(n);

        }
    }
}
