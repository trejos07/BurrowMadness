using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DarkAmbientController : MonoBehaviour
{

    [SerializeField] Tilemap darkTilemap;  
    [SerializeField] Tilemap bluredTilemap;
    Tilemap bgMap;

    [SerializeField] Tile dark;
    [SerializeField] Tile blur;

    private void Awake()
    {
        
    }

    private void Start()
    {
        bgMap = GameplayManager.Instance.ActiveWorld.GroundTileMap;
        darkTilemap.origin = bluredTilemap.origin=bgMap.origin;
        darkTilemap.size = bluredTilemap.size = bgMap.size;

        foreach (Vector3Int p in darkTilemap.cellBounds.allPositionsWithin)
        {
            darkTilemap.SetTile(p, dark);
        }
        foreach (Vector3Int p in bluredTilemap.cellBounds.allPositionsWithin)
        {
            bluredTilemap.SetTile(p, blur);
        }
    }

}
