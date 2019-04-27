using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrefabChunk))]
public class PrefabChunkEditor : Editor
{
        
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PrefabChunk chunk = (PrefabChunk)target;

        if (GUILayout.Button("Save Chunk"))
        {
            chunk.GetTileInfo();
            XMLManager.SaveData(chunk.ChunkInfo,XMLManager.CHUNKINFO_FOLDER_NAME+chunk.name+".xml");
            EditorUtility.DisplayDialog("Guadado de Chunk", "se guardo el Chunk " + chunk.name,"Ok","");

        }

        if (GUILayout.Button("Load Chunk"))
        {
            TerrainTile.tileList = new List<TerrainTile>(Resources.LoadAll<TerrainTile>("Tiles"));
            string path = EditorUtility.OpenFilePanel("Select Chunk Data", Application.dataPath + "/Resources/XML/ChunkInfo","XML");
            path = path.Replace(Application.dataPath + "/Resources/", "");
            path = path.Replace(".xml", "");
            chunk.LoadWorldInfo(XMLManager.LoadData<ChunkInfo>(path));
        }

    }


}
