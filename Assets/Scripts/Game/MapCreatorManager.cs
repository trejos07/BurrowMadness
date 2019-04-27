using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapCreatorManager : MonoBehaviour
{
    Sprite[] sprites;
    List<WorldIcon> icons = new List<WorldIcon> ();
    [SerializeField] List<GameObject> spawns;

    private void Awake()
    {
        sprites = Resources.LoadAll<Sprite>("Sprites/MapIcons");
        SetIcons(GameManager.ins.Worlds);
        DrawIcons();
        transform.localScale = Vector3.one * 0.3f;
    }

    public void ClearIcons()
    {
        foreach (Transform child in transform)
        {
            if (transform != child)
                DestroyImmediate(child);
        }

    }

    public void DrawIcons()
    {
        //ClearIcons();
        for (int i = 0; i < icons.Count; i++)
        {
            WorldInfo info = GameManager.ins.GetWorldInfo(icons[i].worldName);
            
            GameObject iconGO = new GameObject(info.name + "_Icon");

            iconGO.AddComponent<RectTransform>();
            iconGO.transform.SetParent(transform);
            Image img = iconGO.AddComponent<Image>();
            img.sprite = sprites[icons[i].spriteId];
            img.color = info.color;
            RectTransform rt = (RectTransform)iconGO.transform;
            rt.anchoredPosition = icons[i].mapPos;
            
            iconGO.AddComponent<Button>().onClick.AddListener(()=>GameManager.ins.SelectedWorld =info);
        }
    }

    public void SetIcons(List<WorldInfo> worlds)
    {
        for (int i = 0; i < worlds.Count; i++)
        {
            WorldIcon icon = new WorldIcon
            {
                worldName = worlds[i].name,
                mapPos = GetIconPos(),
                spriteId = Random.Range(0, sprites.Length)
            };
            icons.Add(icon);
        }

    }

    public Vector2 GetIconPos()
    {
        //int i = Random.Range(0, spawns.Count);
        //Vector2 pos = spawns[i].transform.localPosition;
        //spawns.Remove(spawns[i]);
        //return pos;

        RectTransform transform = (RectTransform)this.transform;
        Vector3[] corners = new Vector3[4];
        transform.GetWorldCorners(corners);


        float offsetX = transform.rect.width * 0.4f;
        float offsetY = transform.rect.height * 0.4f;

        Vector3 rand = new Vector3(Random.Range(transform.anchorMin.x + offsetX, transform.anchorMax.x - offsetX), Random.Range(transform.anchorMin.y + offsetY, transform.anchorMax.y - offsetY), 0);
        Vector2 pos = rand;
        return pos;

    }

}

public struct WorldIcon
{
    public string worldName;
    public Vector2 mapPos;
    public int spriteId;

}
