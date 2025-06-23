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
    public string directory;

    public DialogSegment GetFirstSegment()
    {
        NodeLinkData firstLink = Links.First(x => x.BaseNodeGUID == EntryGUID);
        return Nodes.First(x => firstLink.TargetNodeGUID == x.GUID).seg;
    }

    public DialogSegment GetSegment(string nodeID)
    {
        NodeData nd = Nodes.First(x => x.GUID == nodeID);
        if (nd.StandardNode)
            return nd.seg;
        else
        {
            DialogLogicSegment dls = nd.lSeg;
            //Bool
            if (dls.lTypeUsed == DialogLogicSegment.LogicTypes.Boolean)
            {
                bool res = dls.ConditionToTest.GetResult<bool>();
                if (res)
                    return GetSegment(dls.b_True.Link);
                else
                    return GetSegment(dls.b_False.Link);
            }
            //Integer
            else if (dls.lTypeUsed == DialogLogicSegment.LogicTypes.Integer)
            {
                int res = dls.ConditionToTest.GetResult<int>();
                //Loop through the options
                foreach (LogicChoice lc in dls.Choices)
                {
                    int parsed = -1;
                    //Parse the input
                    if (int.TryParse(lc.ChoiceText, out parsed))
                    {
                        //Perform the check based on the selected type
                        switch (lc.i_Compare)
                        {
                            case DialogLogicSegment.IntCompTypes.Equals:
                                if (res == parsed)
                                    return GetSegment(lc.Link);
                                break;
                            case DialogLogicSegment.IntCompTypes.GreaterThanOrEqual:
                                if (res >= parsed)
                                    return GetSegment(lc.Link);
                                break;
                            case DialogLogicSegment.IntCompTypes.LessThan:
                                if (res < parsed)
                                    return GetSegment(lc.Link);
                                break;
                        }
                    }
                }
                return GetSegment(dls.Default.Link);
            }
            else if (dls.lTypeUsed == DialogLogicSegment.LogicTypes.String)
            {
                string res = dls.ConditionToTest.GetResult<string>();
                foreach (LogicChoice lc in dls.Choices)
                {
                    if (res == lc.ChoiceText)
                        return GetSegment(lc.Link);
                }
                return GetSegment(dls.Default.Link);
            }
            return GetSegment(dls.Default.Link);
        }
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
