using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Tilemaps;
# if UNITY_EDITOR
using UnityEditor;
# endif


public class TerrainTile : Tile {

    public Sprite[] m_Sprites;
    


    // This refreshes itself and other RoadTiles that are orthogonally and diagonally adjacent
    public override void RefreshTile(Vector3Int location, ITilemap tilemap)
    {
        for (int yd = -1; yd <= 1; yd++)
            for (int xd = -1; xd <= 1; xd++)
            {
                Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                if (HasNeighbTile(tilemap, position))
                {
                    tilemap.RefreshTile(position);
                }
                    
            }
       
    }


    // This determines which sprite is used based on the RoadTiles that are adjacent to it and rotates it to fit the other tiles.
    // As the rotation is determined by the RoadTile, the TileFlags.OverrideTransform is set for the tile.

    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        int mask = HasNeighbTile(tilemap, location + new Vector3Int(0, 1, 0)) ? 1 : 0;
        mask += HasNeighbTile(tilemap, location + new Vector3Int(1, 0, 0)) ? 2 : 0;
        mask += HasNeighbTile(tilemap, location + new Vector3Int(0, -1, 0)) ? 4 : 0;
        mask += HasNeighbTile(tilemap, location + new Vector3Int(-1, 0, 0)) ? 8 : 0;
        int index = GetIndex((byte)mask);
        if (index >= 0 && index < m_Sprites.Length)
        {
            tileData.sprite = m_Sprites[index];
            tileData.color = color;
            var m = tileData.transform;
            m.SetTRS(Vector3.zero, Quaternion.identity, Vector3.one);
            tileData.transform = m;
            tileData.flags = TileFlags.LockTransform;
            tileData.colliderType = colliderType;

            //MPosition = location;
            //BIsWall = (colliderType != ColliderType.None);
            //setNeighbNodes(tilemap, location);

        }
        else
        {
            Debug.LogWarning("Not enough sprites in RoadTile instance");
        }
    }
    // This determines if the Tile at the position is the same RoadTile.
    private bool HasNeighbTile(ITilemap tilemap, Vector3Int position)
    {
        return tilemap.GetTile(position) == this;
    }
    
    //void setNeighbNodes(ITilemap tilemap, Vector3Int position)
    //{
    //    List<Node> nodes = new List<Node>();
    //    nodes.Add(GetNeighbNode(tilemap, position + new Vector3Int(0, 1, 0)));
    //    nodes.Add(GetNeighbNode(tilemap, position + new Vector3Int(1, 0, 0)));
    //    nodes.Add(GetNeighbNode(tilemap, position + new Vector3Int(0, -1, 0)));
    //    nodes.Add(GetNeighbNode(tilemap, position + new Vector3Int(-1, 0, 0)));

    //    for (int i = 0; i < nodes.Count; i++)
    //    {
    //        if (nodes[i] == null)
    //        nodes.Remove(nodes[i]);
    //    }
    //    NeighbNodes = nodes;
    //}
    
    //public Node GetNeighbNode(ITilemap tilemap,Vector3Int position)
    //{
        
    //    TileBase posibleNode = tilemap.GetTile(position);
    //    return posibleNode is Node ? posibleNode as Node : null;

    //}


    // The following determines which sprite to use based on the number of adjacent RoadTiles
    private int GetIndex(byte mask)
    {
        switch (mask)
        {
            case 0: return 4;
            case 3: return 6;
            case 6: return 0;
            case 9: return 8;
            case 12: return 2;
            case 1: return 4;
            case 2: return 1;
            case 4: return 1;
            case 5: return 4;
            case 10: return 1;
            case 8: return 2;
            case 7: return 3;
            case 11: return 7;
            case 13: return 5;
            case 14: return 1;
            case 15: return 4;
        }
        return -1;
    }
    // The following determines which rotation to use based on the positions of adjacent RoadTiles


#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a RoadTile Asset
    [MenuItem("Assets/Create/TerrainTile")]
    public static void CreateRoadTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Terrain Tile", "New Terrain Tile", "Asset", "Save Road Tile", "Assets/Tiles");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TerrainTile>(), path);
    }
# endif



}
