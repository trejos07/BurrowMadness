using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Chunk
{
    public Vector2Int ChunkSize { get; set; }
    public Tile[,,] TileInfo { get; set; }
    public int Width { get; set; }
    public int Heigth { get; set; }

    public abstract Tile[,,] GenerateChunk();
}
