using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class ProceduralChunk:Chunk {

    [Range(0, 100)]
    public int groundChance, sourceChance;
    [Range(1, 8)]
    public int groundBirthLimit, sourceBirthLimit;
    [Range(1, 8)]
    public int groundDeathLimit, sourceDeathLimit;
    public int numR=2;

    private int[,,] chunkMap;

    Tile backGround;
    Tile ground;
    Item[] chunkSources;


    public ProceduralChunk(int groundChance, int sourceChance, int groundBirthLimit, int sourceBirthLimit, int groundDeathLimit, int sourceDeathLimit, Vector2Int chunkSize, Item[] chunkSources, Tile ground, Tile backGround)
    {
        this.groundChance = groundChance;
        this.sourceChance = sourceChance;
        this.groundBirthLimit = groundBirthLimit;
        this.sourceBirthLimit = sourceBirthLimit;
        this.groundDeathLimit = groundDeathLimit;
        this.sourceDeathLimit = sourceDeathLimit;
        this.ChunkSize = chunkSize;
        this.chunkSources = chunkSources;
        this.ground = ground;
        this.backGround = backGround;

        Width = chunkSize.x;
        Heigth = chunkSize.y;
        chunkMap = new int[Width, Heigth, chunkSources.Length+1];
        base.TileInfo = GenerateChunk();

    }

    public override Tile[,,] GenerateChunk()
    {
        Tile[,,] chunkTileInfo = new Tile[Width, Heigth, chunkSources.Length+1];
        int layer = 0;
        int[,] groundpositionMap = new int[Width, Heigth];

        chunkTileInfo = GenerateLayer(groundpositionMap, chunkTileInfo, ground,layer);

        layer = 1;
        int[,] itempositionMap = new int[Width, Heigth];
        ItemTile tile= new ItemTile(Item.ItemsList[0]);
        chunkTileInfo = GenerateLayer(itempositionMap, chunkTileInfo, tile, layer);

        return chunkTileInfo;

    }

    Item ChanceRandomItem()
    {
        Item item = null;
        while(true)//posible error
        {
            Item posibleItem = chunkSources[Random.Range(0, chunkSources.Length)];
            item = Random.Range(1, 101) < posibleItem.chance ? posibleItem : null;

            if (item != null)
                break;
        }
        return item;
        
    }
    
    public int[,] GetMapInLayer(int layer)
    {
        int[,] newMap = new int[Width, Heigth];
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Heigth; j++)
            {
                newMap[i, j] = chunkMap[i, j, layer];
            }
        }
        return newMap;
    }

    public Tile[,,] setTilesInLayer(Tile[,,] tilechunk , Tile tile, int layer)
    {
        for (int y = 0; y < Heigth; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if(layer==0)
                {
                    if (chunkMap[x, y, layer] == 1)
                        tilechunk[x, y, layer] = tile;
                    else if(chunkMap[x, y, layer] == 0)
                        tilechunk[x, y, layer] = backGround;
                }
                else if (chunkMap[x, y, layer] == 1)
                {
                    if (tile is ItemTile)
                    {
                        tile = new ItemTile(ChanceRandomItem());
                    }

                    tilechunk[x, y, layer] = tile;
                }
            }
        }
        return tilechunk;
    }

    public Tile[,,] GenerateLayer(int [,] _layerpPositionsMap, Tile[,,] chunkTileInfo, Tile tile, int layer)
    {
        //Debug.Log("generando capa en el chunk ");
        if (!(tile is ItemTile))
            InitWallPos(_layerpPositionsMap);
        else
            InitResourcePos(_layerpPositionsMap, layer);

        for (int i = 0; i < numR; i++)
        {
             if(!(tile is ItemTile))
                _layerpPositionsMap = GenerateWallPos(_layerpPositionsMap);
            else
                _layerpPositionsMap = GenerateRosourcePos(_layerpPositionsMap);
        }

        for (int x = 0; x < _layerpPositionsMap.GetLength(0); x++)
        {
            for (int y = 0; y < _layerpPositionsMap.GetLength(1); y++)
            {
                chunkMap[x, y, layer] = _layerpPositionsMap[x, y];
            }
        }

        chunkTileInfo = setTilesInLayer(chunkTileInfo, tile, layer);

        return chunkTileInfo;
    }

    public int[,] GenerateWallPos(int[,] oldMap)
    {
        int[,] newMap = new int[Width, Heigth];
        int neighb;
        BoundsInt myB = new BoundsInt(-1, -1, 0, 3, 3, 1);

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Heigth; y++)
            {
                neighb = 0;
                foreach (var b in myB.allPositionsWithin)
                {
                    if (b.x == 0 && b.y == 0) continue;
                    if (x + b.x >= 0 && x + b.x < Width && y + b.y >= 0 && y + b.y < Heigth)
                    {
                        neighb += oldMap[x + b.x, y + b.y];
                    }
                    
                }

                if (oldMap[x, y] == 1)
                {
                    if (neighb < groundDeathLimit) newMap[x, y] = 0;
                    else
                        newMap[x, y] = 1;
                }

                if (oldMap[x, y] == 0)
                {
                    if (neighb > groundBirthLimit) newMap[x, y] = 1;
                    else
                        newMap[x, y] = 0;
                }
            }

        }
        return newMap;
    }

    public int[,] GenerateRosourcePos(int[,] oldMap)
    {
        int[,] newMap = new int[Width, Heigth];
        int neighb;
        BoundsInt myB = new BoundsInt(-1, -1, 0, 3, 3, 1);

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Heigth; y++)
            {
                neighb = 0;
                foreach (var b in myB.allPositionsWithin)
                {
                    if (b.x == 0 && b.y == 0) continue;
                    if (x + b.x >= 0 && x + b.x < Width && y + b.y >= 0 && y + b.y < Heigth)
                    {
                        neighb += oldMap[x + b.x, y + b.y];
                    }
                    
                }

                if (oldMap[x, y] == 1 && chunkMap[x, y, 0] == 1)
                {
                    if (neighb < sourceDeathLimit) newMap[x, y] = 0;
                    else 
                        newMap[x, y] = 1;
                }

                if (oldMap[x, y] == 0  && chunkMap[x, y, 0] == 1)
                {
                    if (neighb > sourceBirthLimit ) newMap[x, y] = 1;
                    else
                        newMap[x, y] = 0;
                }
            }

        }
        return newMap;
    }

    public void InitWallPos(int[,] _positionsMap)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Heigth; y++)
            {
                _positionsMap[x, y] = Random.Range(1, 101)< groundChance ? 1:0;//es un if que asigana 1 o 0 
            }
        }
    }

    public void InitResourcePos(int[,] _positionsMap, int layer)
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Heigth; y++)
            {
                if (chunkMap[x, y,0] == 1)
                {
                    int chance = Random.Range(1, 101);
                    _positionsMap[x, y] = chance < sourceChance ? 1:0;//es un if que asigana 1 o 0 
                }
            }
        }

    }



    /*
    public void ClearMap(Tilemap _tileMap)
    {
        _tileMap.ClearAllTiles();
    }

    public void ClearMap(Tilemap _tileMap, int[,] positionsMap)
    {
        _tileMap.ClearAllTiles();
        positionsMap = null; 


    }

    void Update () {
        //if (Input.GetKeyDown(KeyCode.K))
        //    Genarete();
        //if (Input.GetKeyDown(KeyCode.C))
        //    ClearMap(true);
        //if (Input.GetKeyDown(KeyCode.S))
            //SaveAssetMap();
        


    }
    
    public void SaveAssetMap()
    {
        string saveName = "tmapXY_";
        var mf = GameObject.Find("Grid");

        if (mf)
        {
            var savePath = "Assets/" + saveName + ".prefab";
            if (PrefabUtility.CreatePrefab(savePath, mf))
            {
                EditorUtility.DisplayDialog("Tilemap saved", "Your Tilemap was saved under" + savePath, "Continue");
            }
            else
            {
                EditorUtility.DisplayDialog("Tilemap NOT saved", "An ERROR occured while trying to saveTilemap under" + savePath, "Continue");
            }

        }

    }
*/

   

}
