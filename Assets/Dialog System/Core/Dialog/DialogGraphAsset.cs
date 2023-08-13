using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class DialogGraphAsset : ScriptableObject
{
    public string EntryGUID;
    public List<NodeData> Nodes = new List<NodeData>();
    public List<NodeLinkData> Links = new List<NodeLinkData>();

    public DialogSegment GetFirstSegment()
    {
        NodeLinkData firstLink = Links.First(x => x.BaseNodeGUID == EntryGUID);
        return Nodes.First(x => firstLink.TargetNodeGUID == x.GUID).seg;
    }

    public DialogSegment GetSegment(string nodeID)
    {
        return Nodes.First(x => x.GUID == nodeID).seg;
    }

    public List<NodeLinkData> GetOrphanedLinks()
    {
        List<NodeLinkData> returnList = new List<NodeLinkData>();
        foreach (NodeLinkData link in Links)
        {
            string baseID = link.BaseNodeGUID;
            string targetID = link.TargetNodeGUID;
            bool baseMatch = false;
            bool targetMatch = false;
            //Gather lists of how many nodes match the base node or the target node.
            foreach (NodeData node in Nodes)
            {
                if (node.GUID == baseID)
                    baseMatch = true;
                else if (node.GUID == targetID)
                    targetMatch = true;
            }
            //IF either list is empty, then its probably an oprhaned link
            if (!baseMatch || !targetMatch)
            {
                returnList.Add(link);
            }
        }
        return returnList;
    }

    public void PruneLinks()
    {
        foreach (NodeLinkData link in GetOrphanedLinks())
        {
            Links.Remove(link);
        }
    }

}
