using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

[CustomEditor(typeof(DialogGraphAsset))]
public class DialogGraphAsset_Inspector : Editor
{
    [OnOpenAsset]
    public static bool OnDoubleclick(int instID, int line)
    {
        DialogGraphAsset dga = UnityEditor.EditorUtility.InstanceIDToObject(instID) as DialogGraphAsset;
        if (dga != null)
        {
            DialogGraph dg = EditorWindow.GetWindow<DialogGraph>();
            dg.RequestDataOperation(false, dga.name);
            dg.GetDGV().FrameOrigin();
            return true;
        }
        return false;
    }
}
