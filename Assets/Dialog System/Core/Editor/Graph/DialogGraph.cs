using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class DialogGraph : EditorWindow
{
    private DialogGraphView dialogGraphView;
    private TextField namefield;

    [MenuItem("Graph/Dialog Graph")]
    public static void OpenWindow()
    {
        DialogGraph window = GetWindow<DialogGraph>();
        window.titleContent = new GUIContent("Dialog Graph");
    }

    public DialogGraphView GetDGV()
    {
        return dialogGraphView;
    }

    private void OnEnable()
    {
        RenderGraph();
        GenerateToolbar();
    }

    private string filename = "New Dialog";
    private void GenerateToolbar()
    {
        Toolbar t = new Toolbar();


        //FIle name
        TextField name = new TextField("Filename: ");
        namefield = name;
        name.SetValueWithoutNotify(filename);
        name.MarkDirtyRepaint();
        name.RegisterValueChangedCallback(evt => filename = evt.newValue);
        t.Add(name);

        //Save and Load buttons
        t.Add(new Button(() => RequestDataOperation(true, filename)) { text = "Save Data"});
        t.Add(new Button(() => RequestDataOperation(false, filename)) { text = "Load Data"});

        //Node Button
        Button NodeCreateButton = new Button(() => { dialogGraphView.CreateNode(new DialogSegment()); });
        NodeCreateButton.text = "Create Node";
        t.Add(NodeCreateButton);

        //Logic Node Button
        Button LogicNodeCreateButton = new Button(() => { dialogGraphView.CreateNode(new DialogLogicSegment()); });
        LogicNodeCreateButton.text = "Create Logic Node";
        t.Add(LogicNodeCreateButton);

        //
        rootVisualElement.Add(t);
    }

    public void RequestDataOperation(bool save, string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            EditorUtility.DisplayDialog("No Name!","Plese put in a file name","Ok.");
        namefield.SetValueWithoutNotify(fileName);
        filename = fileName;

        GraphSaveUtility su = GraphSaveUtility.GetInstance(dialogGraphView);
        if (save)
        {
            su.SaveGraph(fileName);
        }
        else
        {
            su.LoadData("Assets/Dialog System/Dialogs/" + fileName + ".asset");
        }
    }

    private void RenderGraph()
    {
        dialogGraphView = new DialogGraphView
        {
            name = "Dialog Graph"
        };
        dialogGraphView.StretchToParentSize();
        rootVisualElement.Add(dialogGraphView);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(dialogGraphView);   
    }
}
