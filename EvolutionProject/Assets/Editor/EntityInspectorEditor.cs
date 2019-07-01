using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(CreatureEntity))]
public class EntityInspectorEditor : Editor 
{
    CreatureEntity e;
    
    void OnEnable()
    {
        e = (CreatureEntity)target;
    }

    public override void OnInspectorGUI()
    {
       if (GUILayout.Button("Randomise Values")) {
           e.Randomise();
       }
       
       DrawDefaultInspector();
    }
}
