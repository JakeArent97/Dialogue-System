using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogGraphLogicNode : DialogGraphNode
{
    public DialogLogicSegment logicSegment;

    public void BuildOutputContainer(GraphView gv)
    {
        //Add Condition
        Label ConditionLabel = new Label();
        Label cl2 = new Label();
        ConditionLabel.text = "Condition to Test:";
        cl2.text = " ";
        outputContainer.Add(ConditionLabel);
        ObjectField oField = new ObjectField();
        oField.objectType = typeof(DialogLogicBase);
        if (logicSegment.ConditionToTest != null)
            oField.value = logicSegment.ConditionToTest;
        oField.RegisterValueChangedCallback(evt => logicSegment.ConditionToTest = (DialogLogicBase)evt.newValue);
        ConditionLabel.Add(cl2);
        ConditionLabel.Add(oField);
        //Add on field Select
        EnumField ef = new EnumField("Type of Condition: ", logicSegment.lTypeUsed);
        ef.RegisterValueChangedCallback(evt =>
        {
            logicSegment.lTypeUsed = (DialogLogicSegment.LogicTypes)evt.newValue;
            RebuildAll(gv);
        });
        ConditionLabel.Add(ef);

        //Switch based on Type
        if (logicSegment.lTypeUsed == DialogLogicSegment.LogicTypes.Boolean)
        {
            //Add True
            Port TrueOption = DialogGraphView.GeneratePort(this, Direction.Output);
            TrueOption.portName = "True";
            TrueOption.contentContainer.Q<Label>("type").text = "True";
            outputContainer.Add(TrueOption);

            //Add False
            Port FalseOption = DialogGraphView.GeneratePort(this, Direction.Output);
            FalseOption.portName = "False";
            FalseOption.contentContainer.Q<Label>("type").text = "False";
            outputContainer.Add(FalseOption);

        }
        else
        {
            //Add Default
            Port DefaultOption = DialogGraphView.GeneratePort(this, Direction.Output);
            DefaultOption.portName = "Default";
            //DefaultOption.title = "Default";
            DefaultOption.contentContainer.Q<Label>("type").text = "Default";
            outputContainer.Add(DefaultOption);

            //Add Button
            Button button = new Button(() => { AddOptionPort(gv); });
            button.text = "New Choice";
            titleContainer.Add(button);
        }
    }

    public void RebuildAll(GraphView gv)
    {
        //Clear
        outputContainer.Clear();
        int index = -1;
        foreach (VisualElement child in titleContainer.Children())
            if (child is Button)
            {
                Button b = child as Button;
                if (b.text == "New Choice")
                    index = titleContainer.IndexOf(b);
            }
        if (index >= 0)
            titleContainer.RemoveAt(index);

        //Rebuild
        BuildOutputContainer(gv);
        RebuildPorts(gv);
    }

    public void RebuildPorts(GraphView gv)
    {
        foreach (LogicChoice lc in logicSegment.Choices)
        {
            if (lc.l_Compare == logicSegment.lTypeUsed && logicSegment.lTypeUsed != DialogLogicSegment.LogicTypes.Boolean)
                AddOptionPort(gv, lc.ChoiceText,lc.i_Compare);
        }
    }

    private void AddOptionPort(GraphView gv, string portName = "", DialogLogicSegment.IntCompTypes iComp = DialogLogicSegment.IntCompTypes.Equals)
    {
        Port genPort = DialogGraphView.GeneratePort(this, Direction.Output);
        
        if (string.IsNullOrEmpty(portName))
        {
            var portCount = outputContainer.childCount;
            genPort.portName = "Choice " + portCount;
        }
        else
            genPort.portName = portName;

        //Setup the Foldout
        Foldout fold = new Foldout
        {
            text = genPort.portName + "- Info",
            value = false
        };

        //Add port name customization and deletion
        Label bufferLabel = new Label();
        bufferLabel.text = "  -  ";
        TextField textField = new TextField
        {
            name = string.Empty,
            value = genPort.portName
        };
        textField.style.minWidth = 30;
        textField.RegisterValueChangedCallback(evt =>
        {
            genPort.portName = evt.newValue;
            fold.text = evt.newValue + "- Info";
        });

        //Delete Button
        var deleteButton = new Button(() => RemovePort(this, genPort,gv))
        {
            text = "Delete"
        };

        //Int Compare Type
        EnumField ef = new EnumField(iComp);
        
        fold.Add(textField);
        if (logicSegment.lTypeUsed == DialogLogicSegment.LogicTypes.Integer)
        {
            fold.Add(ef);
        }
        fold.Add(deleteButton);
        

        //Add port content
        outputContainer.Add(genPort);
        outputContainer.Add(fold);
        RefreshExpandedState();
        RefreshPorts();
    }

    private void RemovePort(DialogGraphNode dialogNode, Port generatedPort, GraphView gv)
    {
        var targetEdge = gv.edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == dialogNode);
        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            gv.RemoveElement(edge);
        }
        if (dialogNode is DialogGraphStandardNode)
        {
            DialogGraphStandardNode dsn = dialogNode as DialogGraphStandardNode;
            if (dialogNode.outputContainer.childCount < 2)
            {
                dsn.choicesBool.value = false;
            }
        }

        //Find the corresponding foldout:
        VisualElement targ = null;
        foreach (VisualElement v in dialogNode.outputContainer.Children())
        {
            if (v is Foldout)
            {
                Foldout f = v as Foldout;
                if (f.text == generatedPort.portName + "- Info")
                    targ = v;
            }
        }
        dialogNode.outputContainer.Remove(targ);
        dialogNode.outputContainer.Remove(generatedPort);
        dialogNode.RefreshExpandedState();
        dialogNode.RefreshPorts();
    }
}
