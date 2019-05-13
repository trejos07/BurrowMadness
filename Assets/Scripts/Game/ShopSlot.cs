using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour
{
    Button m_button;
    TextMeshProUGUI m_name;
    TextMeshProUGUI m_price;
    Image m_image;
    ShopItem item;

    public Button Button
    {
        get
        {
            return m_button;
        }

        set
        {
            m_button = value;
        }
    }
    public Image Image
    {
        get
        {
            return m_image;
        }

        set
        {
            m_image = value;
        }
    }
    public ShopItem Item
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

    public static ShopSlot CreateComponent(GameObject where, ShopItem item)
    {
        where.SetActive(false);
        where.name = item.Name;
        ShopSlot myC = where.AddComponent<ShopSlot>();
        myC.Item = item;
        where.SetActive(true);
        return myC;
    }
    private void Awake()
    {
        m_name = transform.Find("ItenName_Text").GetComponent<TextMeshProUGUI>();
        m_price = transform.Find("ShoItem_Content_Panel/Price_Text").GetComponent<TextMeshProUGUI>();
        Image = transform.Find("ShoItem_Content_Panel/Item_Image").GetComponent<Image>();
        Button = GetComponent<Button>();

        m_name.text = item.Name;
        m_price.text = item.Price.ToString();

        if(item is WeaponShopItem)
        {
            WeaponShopItem weaponItem = item as WeaponShopItem;
            SpriteRenderer renderer = Weapon.Types[weaponItem.WeaponID].Renderer;

            Image.sprite = renderer.sprite;
            Image.material = renderer.sharedMaterial;
        }

    }



}


