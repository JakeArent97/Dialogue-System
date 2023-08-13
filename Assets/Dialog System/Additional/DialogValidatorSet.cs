using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogValidatorSet : DialogValidator
{
    public List<DialogValidator> Validators = new List<DialogValidator>();
    public override bool Validate()
    {
        foreach (DialogValidator dv in Validators)
            if (!dv.Validate())
                return false;
        return true;
    }
}
