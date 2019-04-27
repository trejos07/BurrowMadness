using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldInfo
{
    public string name;
    public WorldDificulty dificulty= WorldDificulty.easy;
    public Vector2Int worldSizeInChunks;
    public int[] worldSources;
    public Color color;
    public ChunkInfo[][] chunks;
    public int nroSpawners;
    public int nroDungeons;
    public List<Vector3> spawnersPos = new List<Vector3>();
    public float gravity;

    public WorldInfo(string name, int maxSpawners, int nroDungeons, Vector2Int worldSizeInChunks, int[] worldSources, ChunkInfo[][] chunks, Color color, float gravity)
    {
        this.name = name;
        this.nroSpawners = maxSpawners;
        this.worldSizeInChunks = worldSizeInChunks;
        this.worldSources = worldSources;
        this.chunks = chunks;
        this.color = color;
        this.gravity = gravity;
        this.nroDungeons = nroDungeons;
    }

    public WorldInfo()
    { }

    public void Save()
    {
        XMLManager.SaveData(this, XMLManager.WORLDINFO_FOLDER_NAME+ "GeneratedWorlds/" + name + ".xml");
    }
}
