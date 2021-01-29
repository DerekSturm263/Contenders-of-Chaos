using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MoveBetweenTwoPoints))]
public class EditorPlatform : Editor
{
    public override void OnInspectorGUI()
    {
        MoveBetweenTwoPoints myTarget = (MoveBetweenTwoPoints) target;

        DrawDefaultInspector();

        if (GUILayout.Button("Copy Position"))
        {
            myTarget.CopyPosition();
            int position = myTarget.positions.Count - 1;
            Handles.DrawLine(myTarget.positions[position], myTarget.transform.position);
        }

        if (GUILayout.Button("Reset To Default Position"))
        {
            myTarget.transform.position = myTarget.positions[0];
        }

        if (GUILayout.Button("Clear All Positions"))
        {
            myTarget.transform.position = myTarget.positions[0];
            myTarget.positions.Clear();
        }
    }
}
