using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(GeneticEntity))]
public class EntityInspectorEditor : Editor 
{
    GeneticEntity e;
    
    void OnEnable()
    {
        e = (GeneticEntity)target;
    }

    public override void OnInspectorGUI()
    {
       if (GUILayout.Button("Randomise Values")) {
           e.Randomise();
       }
       
       DrawDefaultInspector();
    }
}
