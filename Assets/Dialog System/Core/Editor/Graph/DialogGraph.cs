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
        t.Add(new Button(() => RequestDataOperation(true, filename,directory:dialogGraphView.FileDirectory)) { text = "Save Data"});
        t.Add(new Button(() => RequestDataOperation(false, filename,directory:dialogGraphView.FileDirectory)) { text = "Load Data"});

        //Node Button
        Button NodeCreateButton = new Button(() => 
        {
            dialogGraphView.CreateNode(new DialogSegment());
            RequestDataOperation(true, filename, true,dialogGraphView.FileDirectory);
        });
        NodeCreateButton.text = "Create Node";
        t.Add(NodeCreateButton);

        //Logic Node Button
        Button LogicNodeCreateButton = new Button(() => 
        {
            dialogGraphView.CreateNode(new DialogLogicSegment());
            RequestDataOperation(true, filename, true,dialogGraphView.FileDirectory);
        });
        LogicNodeCreateButton.text = "Create Logic Node";
        t.Add(LogicNodeCreateButton);

        //
        rootVisualElement.Add(t);
    }

    public void RequestDataOperation(bool save, string fileName, bool autosave = false, string directory = "Assets/Dialog System/Dialogs/")
    {
        dialogGraphView.FileDirectory = directory;
        if (string.IsNullOrEmpty(fileName))
            EditorUtility.DisplayDialog("No Name!","Plese put in a file name","Ok.");

        string newFilename = fileName;
        if (newFilename.Contains("-autosave"))
        {
            newFilename = newFilename.Replace("-autosave", "");
        }
        namefield.SetValueWithoutNotify(newFilename);
        filename = newFilename;

        GraphSaveUtility su = GraphSaveUtility.GetInstance(dialogGraphView);
        if (save)
        {
            su.SaveGraph(fileName,autosave);
        }
        else
        {
            if (fileName.Contains("-autosave"))
            {
                su.LoadData(dialogGraphView.FileDirectory + fileName + ".asset");
            }
            else
            {
                su.LoadData(dialogGraphView.FileDirectory + fileName + ".asset");
            }
        }
    }

    private void RenderGraph()
    {
        dialogGraphView = new DialogGraphView
        {
            FileName = filename,
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
