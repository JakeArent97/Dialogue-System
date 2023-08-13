using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Set", menuName = "Dialog/Actions/Create Action Set")]
public class ActionEffectSet : ActionEffect
{
    public List<ActionEffect> Effects = new List<ActionEffect>();
    public override void DoEffect()
    {
        foreach (ActionEffect ae in Effects)
        {
            ae.DoEffect();
        }
    }
}