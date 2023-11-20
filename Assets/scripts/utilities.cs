using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class utilities 
{
   public static T[] Shuffle<T>(T[]array,int seed)
    {
        System.Random r = new System.Random(seed);
        for(int i = 0; i < array.Length; i++)
        {
            int num = r.Next(i, array.Length);
            T t = array[i];
            array[i] = array[num];
            array[num] = t;
        }   
        return array;
    }
        
}
