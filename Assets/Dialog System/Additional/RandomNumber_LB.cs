using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNumber_LB : DialogLogicBase
{
    public int maxVal = 20;
    public override T GetResult<T>()
    {
        //Impl
        System.Random r = new System.Random();
        int num = r.Next(maxVal);
        //Typing
        Type typeParameterType = typeof(T); 
        //Return
        return (T)Convert.ChangeType(num,typeParameterType);
    }
}
