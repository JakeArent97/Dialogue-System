using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogLogicBase : ScriptableObject
{
    public abstract T GetResult<T>();
}
