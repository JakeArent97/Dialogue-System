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
            string dir = AssetDatabase.GetAssetPath(dga.GetInstanceID());
            dir = dir.Replace(dga.name + ".asset", "");
            dg.RequestDataOperation(false, dga.name,directory:dir);
            dg.GetDGV().FrameOrigin();
            return true;
        }
        return false;
    }
}
