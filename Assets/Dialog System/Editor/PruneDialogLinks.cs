using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class PruneDialogLinks : EditorWindow
{
    DialogGraphAsset target;

    [MenuItem("Tools/Custom/Dialog/PruneDialogLinks")]
    static void Init()
    {
        PruneDialogLinks window = (PruneDialogLinks)GetWindow(typeof(PruneDialogLinks));
    }

    private void OnGUI()
    {
        target = (DialogGraphAsset)EditorGUILayout.ObjectField("Target Dialog", target, typeof(DialogGraphAsset), true);
        if (target != null)
        {
            List<NodeLinkData> listOfLinks = target.GetOrphanedLinks();
            GUILayout.Label(listOfLinks.Count + " possible orphaned links found, out of " + target.Links.Count + " total links.");
            if (GUILayout.Button("Click to prune them."))
            {
                target.PruneLinks();
            }
        }
    }
}
