using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrefabChunk))]
public class PrefabChunkEditor : Editor
{

    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PrefabChunk chunk = (PrefabChunk)target;
        if (GUILayout.Button("Save"))
        {
            chunk.GetTileInfo();
            string localPath = "Assets/" + chunk.name + ".prefab";chunk.GetTileInfo();
            PrefabUtility.SaveAsPrefabAsset(chunk.gameObject, localPath);

        }
    }


}
