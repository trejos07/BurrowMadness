using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DarkAmbientController : MonoBehaviour
{

    [SerializeField] Tilemap darkTilemap;  
    [SerializeField] Tilemap bluredTilemap;

    [SerializeField] Tile dark;
    [SerializeField] Tile blur;

	private void Awake()
	{
        darkTilemap.origin = bluredTilemap.origin;


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
