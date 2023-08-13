using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DialogValidator : ScriptableObject
{
    public abstract bool Validate();
}
