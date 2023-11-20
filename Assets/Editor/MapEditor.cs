using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(mapgenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
       
        mapgenerator map = (mapgenerator)target;
        if (DrawDefaultInspector())
        {
            map.GenerateMap();
        }
        if (GUILayout.Button("Éú³ÉµØÍ¼"))
        {
          map.GenerateMap();
        }
    
    }
}
