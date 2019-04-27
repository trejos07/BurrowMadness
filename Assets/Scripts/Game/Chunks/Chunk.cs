using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Chunk
{
    public Vector2Int ChunkSize { get; set; }
    public ChunkInfo ChunkInfo { get; set; }
    public int Width { get; set; }
    public int Heigth { get; set; }
    public ChunkType type { get; set; }

    public abstract ChunkInfo GenerateChunk(Vector2Int gPos);
}

public enum ChunkType { procedural, landscape, dungeon }

[System.Serializable]
public class ChunkInfo
{
    static int proceduralConsecutive = 0;

    public string name;
    public Vector2Int gPos;
    public ChunkType type;
    public int[][][] tileInfo;

    public ChunkInfo()
    {
    }

    public ChunkInfo(int[][][] tileInfo, Vector2Int gPos)
    {
        proceduralConsecutive++;
        this.tileInfo = tileInfo;
        name = "procedural_" + proceduralConsecutive.ToString();
        this.gPos = gPos;
        type = ChunkType.procedural;
    }

    public ChunkInfo(int[][][] tileInfo, string name, ChunkType type)
    {
        this.tileInfo = tileInfo;
        this.name = name;
        gPos = Vector2Int.zero;
        this.type = type;
    }

}