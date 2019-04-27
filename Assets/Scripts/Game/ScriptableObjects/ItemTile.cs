using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class ItemTile : Tile
{
    [SerializeField]
    Item item;

    public ItemTile(Item item)
    {
        Item = item;
    }

    public Item Item
    {
        get
        {
            return item;
        }

        set
        {
            item = value;
        }
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        //base.GetTileData(position, tilemap, ref tileData);
        tileData.sprite= item.groundSprite;
        tileData.color = item.color;
        var m = tileData.transform;
        m.SetTRS(Vector3.zero, Quaternion.identity, Vector3.one);
        tileData.transform = m;
        tileData.flags = TileFlags.LockTransform;
        tileData.colliderType = ColliderType.Sprite;
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
        if (!HasTile(tilemap, position))
        {
            item.Piked();
        }

    }

    private bool HasTile(ITilemap tilemap, Vector3Int position)
    {
        return tilemap.GetTile(position) == this;
    }

}
