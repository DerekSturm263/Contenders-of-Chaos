using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnGems))]
public class EditorGem : Editor
{
    public override void OnInspectorGUI()
    {
        SpawnGems myTarget = (SpawnGems) target;

        DrawDefaultInspector();

        if (GUILayout.Button("Copy Position"))
        {
            myTarget.CopyPosition();
        }

        if (GUILayout.Button("Reset All Gems"))
        {
            myTarget.ResetPoints();
        }
    }
}
