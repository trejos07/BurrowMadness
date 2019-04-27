using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Tilemaps;
# if UNITY_EDITOR
using UnityEditor;
# endif


public class TerrainTile : Tile {

    public static int idCount=4;

    public static List<TerrainTile> tileList = new List<TerrainTile>();
    public int id;
    public Sprite[] m_Sprites;

    public int Id
    {
        get
        {
            return id;
        }
    }

    private void Awake()
    {
        if (tileList != null && !tileList.Contains(this))
        {
            tileList.Add(this);
        }
        

            
    }


    private void OnDestroy()
    {
        tileList.Remove(this);
    }

    public static TerrainTile GetTerrainTileOfID(int id)
    {
        TerrainTile tile = null;
        for (int i = 0; i < tileList.Count; i++)
        {
            if(tileList[i].id == id)
            {
                tile = tileList[i];
            }
        }
        return tile;
    }

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

    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        int mask = HasNeighbTile(tilemap, location + new Vector3Int(0, 1, 0)) ? 1 : 0;
        mask += HasNeighbTile(tilemap, location + new Vector3Int(1, 0, 0)) ? 2 : 0;
        mask += HasNeighbTile(tilemap, location + new Vector3Int(0, -1, 0)) ? 4 : 0;
        mask += HasNeighbTile(tilemap, location + new Vector3Int(-1, 0, 0)) ? 8 : 0;

        mask += HasNeighbTile(tilemap, location + new Vector3Int(1, 1, 0)) ? 16 : 0;
        mask += HasNeighbTile(tilemap, location + new Vector3Int(1, -1, 0)) ? 32 : 0;
        mask += HasNeighbTile(tilemap, location + new Vector3Int(-1, -1, 0)) ? 64 : 0;
        mask += HasNeighbTile(tilemap, location + new Vector3Int(-1, 1, 0)) ? 128 : 0;



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
            

        }
        else
        {
            Debug.LogWarning("Not enough sprites in SpriteSheet instance");
        }
    }

    private bool HasNeighbTile(ITilemap tilemap, Vector3Int position)
    {
        return tilemap.GetTile(position) == this;
    }

    private int GetIndex(byte mask)
    {
        switch (mask)
        {
            case 0: return 3;
            case 1: return 17;
            case 2: return 9;
            case 3: return 29;
            case 4: return 4;
            case 5: return 18;
            case 6: return 23;
            case 7: return 25;
            case 8: return 11;
            case 9: return 30;
            case 10: return 5;
            case 11: return 26;
            case 12: return 24;
            case 13: return 20;
            case 14: return 19;
            case 15: return 10;
            case 16: return 3;
            case 17: return 17;
            case 18: return 9;
            case 19: return 14;
            case 20: return 6;
            case 21: return 18;
            case 22: return 23;
            case 23: return 31;
            case 24: return 11;
            case 25: return 30;
            case 26: return 5;
            case 27: return 44;
            case 28: return 24;
            case 29: return 20;
            case 30: return 19;
            case 31: return 42;
            case 32: return 3;
            case 33: return 17;
            case 34: return 9;
            case 35: return 29;
            case 36: return 6;
            case 37: return 18;
            case 38: return 0;
            case 39: return 45;
            case 40: return 11;
            case 41: return 16;
            case 42: return 5;
            case 43: return 26;
            case 44: return 24;
            case 45: return 20;
            case 46: return 38;
            case 47: return 36;
            case 48: return 3;
            case 49: return 17;
            case 50: return 9;
            case 51: return 14;
            case 52: return 6;
            case 53: return 18;
            case 54: return 0;
            case 55: return 6;
            case 56: return 11;
            case 57: return 16;
            case 58: return 5;
            case 59: return 44;
            case 60: return 24;
            case 61: return 20;
            case 62: return 38;
            case 63: return 13;
            case 64: return 3;
            case 65: return 17;
            case 66: return 9;
            case 67: return 29;
            case 68: return 4;
            case 69: return 18;
            case 70: return 23;
            case 71: return 25;
            case 72: return 11;
            case 73: return 16;
            case 74: return 6;
            case 75: return 26;
            case 76: return 2;
            case 77: return 46;
            case 78: return 35;
            case 79: return 37;
            case 80: return 3;
            case 81: return 17;
            case 82: return 9;
            case 83: return 14;
            case 84: return 6;
            case 85: return 18;
            case 86: return 23;
            case 87: return 31;
            case 88: return 11;
            case 89: return 30;
            case 90: return 4;
            case 91: return 44;
            case 92: return 2;
            case 93: return 46;
            case 94: return 35;
            case 95: return 40;
            case 96: return 3;
            case 97: return 17;
            case 98: return 9;
            case 99: return 29;
            case 100: return 4;
            case 101: return 18;
            case 102: return 0;
            case 103: return 45;
            case 104: return 11;
            case 105: return 16;
            case 106: return 5;
            case 107: return 26;
            case 108: return 2;
            case 109: return 46;
            case 110: return 1;
            case 111: return 39;
            case 112: return 3;
            case 113: return 17;
            case 114: return 9;
            case 115: return 14;
            case 116: return 6;
            case 117: return 18;
            case 118: return 0;
            case 119: return 6;
            case 120: return 11;
            case 121: return 30;
            case 122: return 5;
            case 123: return 44;
            case 124: return 2;
            case 125: return 46;
            case 126: return 1;
            case 127: return 28;
            case 128: return 3;
            case 129: return 17;
            case 130: return 9;
            case 131: return 29;
            case 132: return 5;
            case 133: return 18;
            case 134: return 0;
            case 135: return 25;
            case 136: return 11;
            case 137: return 16;
            case 138: return 5;
            case 139: return 41;
            case 140: return 24;
            case 141: return 32;
            case 142: return 19;
            case 143: return 43;
            case 144: return 3;
            case 145: return 17;
            case 146: return 9;
            case 147: return 14;
            case 148: return 6;
            case 149: return 18;
            case 150: return 23;
            case 151: return 31;
            case 152: return 11;
            case 153: return 16;
            case 154: return 5;
            case 155: return 15;
            case 156: return 24;
            case 157: return 32;
            case 158: return 19;
            case 159: return 33;
            case 160: return 3;
            case 161: return 17;
            case 162: return 9;
            case 163: return 29;
            case 164: return 5;
            case 165: return 18;
            case 166: return 0;
            case 167: return 45;
            case 168: return 11;
            case 169: return 16;
            case 170: return 5;
            case 171: return 41;
            case 172: return 24;
            case 173: return 32;
            case 174: return 38;
            case 175: return 34;
            case 176: return 3;
            case 177: return 17;
            case 178: return 9;
            case 179: return 14;
            case 180: return 6;
            case 181: return 18;
            case 182: return 0;
            case 183: return 6;
            case 184: return 11;
            case 185: return 16;
            case 186: return 5;
            case 187: return 15;
            case 188: return 24;
            case 189: return 32;
            case 190: return 38;
            case 191: return 22;
            case 192: return 3;
            case 193: return 17;
            case 194: return 9;
            case 195: return 29;
            case 196: return 6;
            case 197: return 18;
            case 198: return 23;
            case 199: return 25;
            case 200: return 11;
            case 201: return 16;
            case 202: return 5;
            case 203: return 41;
            case 204: return 2;
            case 205: return 8;
            case 206: return 35;
            case 207: return 12;
            case 208: return 3;
            case 209: return 17;
            case 210: return 9;
            case 211: return 14;
            case 212: return 6;
            case 213: return 18;
            case 214: return 23;
            case 215: return 31;
            case 216: return 11;
            case 217: return 16;
            case 218: return 5;
            case 219: return 15;
            case 220: return 2;
            case 221: return 8;
            case 222: return 35;
            case 223: return 21;
            case 224: return 3;
            case 225: return 17;
            case 226: return 9;
            case 227: return 29;
            case 228: return 6;
            case 229: return 18;
            case 230: return 0;
            case 231: return 45;
            case 232: return 11;
            case 233: return 16;
            case 234: return 5;
            case 235: return 41;
            case 236: return 2;
            case 237: return 8;
            case 238: return 1;
            case 239: return 27;
            case 240: return 3;
            case 241: return 17;
            case 242: return 9;
            case 243: return 14;
            case 244: return 6;
            case 245: return 18;
            case 246: return 0;
            case 247: return 6;
            case 248: return 11;
            case 249: return 16;
            case 250: return 5;
            case 251: return 15;
            case 252: return 2;
            case 253: return 8;
            case 254: return 1;
            case 255: return 7;

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
        TerrainTile tile = CreateInstance<TerrainTile>();
        tile.id = idCount++;
        AssetDatabase.CreateAsset(tile, path);

        
        
    }
# endif



}
