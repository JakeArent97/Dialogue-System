using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogGraphView : GraphView
{
    public static Vector2 defNodeSize = new Vector2(100, 150);
    public DialogGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        //Grid
        GridBackground gb = new GridBackground();
        Insert(0, gb);
        gb.StretchToParentSize();

        AddElement(GenerateEntryNode());
    }
    

    private Port GeneratePort(DialogGraphNode node, Direction direction, Port.Capacity cap = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, direction, cap, typeof(float));
    }

    private DialogGraphNode GenerateEntryNode()
    {
        DialogGraphNode node = new DialogGraphNode()
        {
            title = "Start",
            GUID = Guid.NewGuid().ToString(),
            EntryPoint = true
        };

        Port genPort = GeneratePort(node, Direction.Output);
        genPort.portName = "Next";
        node.outputContainer.Add(genPort);

        //refresh
        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(100, 100, 100, 150));
        return node;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compPorts = new List<Port>();
        ports.ForEach(port=>
            {
                if (startPort != port && startPort.node != port.node)
                    compPorts.Add(port);
        });
        return compPorts;
    }

    public void CreateNode(DialogSegment ds)
    {
        DialogGraphNode n = CreateDialogNode(ds);
        Vector2 pos = new Vector2(viewTransform.position.x, viewTransform.position.y) * -1;
        pos += new Vector2(viewport.contentRect.width / 2, viewport.contentRect.height / 2);
        n.SetPosition(new Rect(pos,defNodeSize));
        AddElement(n);
    }

    public DialogGraphNode CreateDialogNode(DialogSegment ds)
    {
        DialogGraphNode node = new DialogGraphNode
        {
            title = "Node",
            dialogSegment = ds,
            GUID = Guid.NewGuid().ToString()
        };

        Port inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
        node.inputContainer.Add(inputPort);

        //Add Button
        Button button = new Button(() => { AddChoicePort(node); });
        button.text = "New Choice";
        node.titleContainer.Add(button);

        //Add Text
        TextField IntroText = new TextField();
        IntroText.multiline = true;
        if (!string.IsNullOrEmpty(ds.Intro))
            IntroText.value = ds.Intro;
        IntroText.RegisterValueChangedCallback(evt => node.dialogSegment.Intro = evt.newValue);
        Label IntroLabel = new Label();
        Label l1 = new Label();
        IntroLabel.text = "Intro";
        l1.text = " ";
        node.extensionContainer.Add(IntroLabel);
        IntroLabel.Add(l1);
        IntroLabel.Add(IntroText);
        node.expanded = true;

        //Add Enter Effect
        Label EnterEffectLabel = new Label();
        Label l2 = new Label();
        EnterEffectLabel.text = "Entry Effects";
        l2.text = " ";
        node.extensionContainer.Add(EnterEffectLabel);
        ObjectField tc = new ObjectField();
        tc.objectType = typeof(ActionEffect);
        if (ds.EffectsOnEnter != null)
            tc.value = ds.EffectsOnEnter;
        tc.RegisterValueChangedCallback(evt => ds.EffectsOnEnter = (ActionEffect)evt.newValue);
        EnterEffectLabel.Add(l2);
        EnterEffectLabel.Add(tc);

        //Add HasChoices Toggle
        Toggle tog = new Toggle("Has Choices:");
        node.choicesBool = tog;
        tog.value = ds.hasChoices;
        tog.RegisterValueChangedCallback(evt => ds.hasChoices = evt.newValue);
        node.extensionContainer.Add(tog);

        //Add Speaker and color
        Label SpeakerLabel = new Label();
        Label l4 = new Label();
        TextField SpeakerInput = new TextField();
        if (!string.IsNullOrEmpty(ds.Speaker))
            SpeakerInput.value = ds.Speaker;
        SpeakerLabel.text = "Speaker";
        SpeakerInput.RegisterValueChangedCallback(evt => ds.Speaker = evt.newValue);
        l4.text = " ";
        node.extensionContainer.Add(SpeakerLabel);
        SpeakerLabel.Add(l4);
        SpeakerLabel.Add(SpeakerInput);
        ColorField colorPick = new ColorField();
        colorPick.value = ds.SpeakerColor;
        colorPick.RegisterValueChangedCallback(evt => ds.SpeakerColor = evt.newValue);
        SpeakerLabel.Add(colorPick);


        //Refresh
        node.RefreshExpandedState();
        node.RefreshPorts();
        node.SetPosition(new Rect(new Vector2(100, 100), defNodeSize));

        return node;
    }

    public void AddChoicePort(DialogGraphNode node, string portName = "", DialogValidator dv = null,bool loadFunc = false)
    {
        Port genPort = GeneratePort(node, Direction.Output);
        //Remove old label
        Label oldLabel = genPort.contentContainer.Q<Label>("type");
        genPort.contentContainer.Remove(oldLabel);
        if (string.IsNullOrEmpty(portName))
        {
            var portCount = node.outputContainer.childCount;
            genPort.portName = "Choice " + portCount;
        }
        else
            genPort.portName = portName;

        //Add port name customization and deletion
        Label bufferLabel = new Label();
        bufferLabel.text = "  ";
        TextField textField = new TextField
        {
            name = string.Empty,
            value = genPort.portName
        };
        textField.RegisterValueChangedCallback(evt => genPort.portName = evt.newValue);

        //Add Validators
        ObjectField validators = new ObjectField();
        validators.objectType = typeof(DialogValidator);
        if (dv != null)
        {
            validators.value = dv;
        }

        var deleteButton = new Button(() => RemovePort(node, genPort))
        {
            text = "x"
        };
        //Check if more than 2 ports
        if (node.outputContainer.childCount >= 1 && !loadFunc)
        {
            node.choicesBool.value = true;
        }
        genPort.contentContainer.Add(bufferLabel);
        genPort.contentContainer.Add(textField);
        genPort.contentContainer.Add(validators);
        genPort.contentContainer.Add(deleteButton);



        //Add port content
        node.outputContainer.Add(genPort);
        node.RefreshExpandedState();
        node.RefreshPorts();
    }

    private void RemovePort(DialogGraphNode dialogNode, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == dialogNode);
        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(edge);
        }

        if (dialogNode.outputContainer.childCount < 2)
        {
            dialogNode.choicesBool.value = false;
        }

        dialogNode.outputContainer.Remove(generatedPort);
        dialogNode.RefreshExpandedState();
        dialogNode.RefreshPorts();
    }
}
