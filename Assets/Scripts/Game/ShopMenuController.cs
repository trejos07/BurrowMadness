using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopMenuController : MonoBehaviour
{
    public static ShopMenuController Instance;

    [SerializeField] GameObject storeViewPort;
    [SerializeField] GameObject sectionPanel;
    [SerializeField]GameObject itemSlot;
    [SerializeField]GameObject statSlot;
    [SerializeField] int weaponBasePrice=500;
    [SerializeField] int UpgradeBasePrice=100;
    [SerializeField] int UpgradeIncrementPrice=50;

    ShopSlot selectedShopSlot;
    Image itemIcon;
    Transform itemStatsPanel;
    TextMeshProUGUI itemName;

    public delegate void ShopSlotOparation(ShopSlot slot);
    public event ShopSlotOparation OnSlotSelected;

    List<int> soldItems;
    List<ShopSlot> slots;
    List<GameObject> stats= new List<GameObject>();

    public ShopSlot SelectedShopSlot
    {
        get
        {
            return selectedShopSlot;
        }

        set
        {
            if(value!=null&& value != selectedShopSlot)
            {
                selectedShopSlot = value;
                UpadateSelectedItemPanel();
            }
            else selectedShopSlot = value;
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        itemStatsPanel = transform.Find("ItemInfo_Panel/Stats_Details_Panel");
        itemName = transform.Find("ItemInfo_Panel/Item_Name_Text").GetComponent<TextMeshProUGUI>();
        itemIcon = transform.Find("ItemInfo_Panel/Item_ImageSlot/Item_Image").GetComponent<Image>();

        transform.Find("ItemInfo_Panel/ButtonsPanel/Upgrade_Button").GetComponent<Button>().onClick.AddListener(UpgradeValidation);
        transform.Find("ItemInfo_Panel/ButtonsPanel/Equip_Button").GetComponent<Button>().onClick.AddListener(EquipSelectedSlot);

        soldItems = new List<int>();
        
        GameObject Section = Instantiate(sectionPanel, storeViewPort.transform);
        Section.transform.Find("Section_Name").GetComponent<TextMeshProUGUI>().text = "Weapons";
        slots= CreateSlots(Section.transform);



    }


    private void Start()
    {
        CheckSlodItems();
    }
    void CheckSlodItems()
    {
        List<Weapon> weapons = Player.Instance.Weapons;
        for (int j = 0; j < slots.Count; j++)
        {
            if (slots[j].Item is WeaponShopItem)
            {
                WeaponShopItem item = slots[j].Item as WeaponShopItem;
                for (int i = 0; i < weapons.Count; i++)
                {
                    if (item.WeaponID == weapons[i].ID)
                    {
                        soldItems.Add(item.Id);
                    }
                }
            }
        }
    }

    void UpadateSelectedItemPanel()
    {
        ShopItem item = SelectedShopSlot.Item;
        if ( item is WeaponShopItem)
        {
            WeaponShopItem weaponItem = item as WeaponShopItem;
            Weapon weapon = FindWeaponInPlayerInventory(weaponItem.WeaponID);
            itemName.text = weapon.info.name;
            itemIcon.sprite = weapon.Renderer.sprite;
            itemIcon.material = weapon.Renderer.sharedMaterial;

            foreach (GameObject t in stats)
            {
                if (t != itemStatsPanel.gameObject)
                    DestroyImmediate(t.gameObject);
            }
            stats.Clear();
            CreateStatPanel("Level",weapon.info.Level.ToString());
            CreateStatPanel("Fire Rate",weapon.info.FireRate.ToString());
            CreateStatPanel("Aim Speed",weapon.info.Velocidad_Rotacion.ToString());
            CreateStatPanel("Bullet Damage",weapon.info.Damage.ToString());
            transform.Find("ItemInfo_Panel/ButtonsPanel/Upgrade_Button").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("ItemInfo_Panel/ButtonsPanel/Upgrade_Button").gameObject.SetActive(false);
        }

    }
    void CreateStatPanel(string _atribute, string _value)
    {
        GameObject stat =  Instantiate(statSlot, itemStatsPanel);
        stat.transform.Find("ItenmNameText").GetComponent<TextMeshProUGUI>().text = _atribute;
        stat.transform.Find("Stat_value").GetComponent<TextMeshProUGUI>().text = _value;
        stats.Add(stat);
    }

    List<ShopItem> CreateItems()
    {
        List<ShopItem> items = new List<ShopItem>();
        List<Weapon> weapons = Weapon.Types;
        for (int i = 0; i < weapons.Count; i++)
        {
            Weapon w = weapons[i];
            items.Add(new WeaponShopItem(w.name,weaponBasePrice,w.ID));
        }
        return items;
    }
    List<ShopSlot> CreateSlots(Transform parent)
    {
        List<ShopItem> items = CreateItems();
        List<ShopSlot> slots = new List<ShopSlot>(items.Count);
        for (int i = 0; i < items.Count; i++)
        {
            GameObject sItem = Instantiate(itemSlot, parent);
            ShopSlot slot = ShopSlot.CreateComponent(sItem, items[i]);
            slot.Button.onClick.AddListener(() => {ValidateStorage(slot);} );
            slots.Add(slot);
        }
        return slots;

    }
    void Buy(ShopSlot slot)
    {
        ShopItem item = slot.Item;
        if(GameManager.Instance.ChargeGoldPurchase(item.Price))
        {
            //BlockSlot(slot);
            soldItems.Add(item.Id);
            Deliverpurchase(item);
            SelectedShopSlot = slot;
        }
        else
        {
            MenuManager.Instance.DisplayMesagge("parece que no tienes dinero sufuciente, ve y completa una misiones", "Dinero Insificiente");
        }
    }
    int CheckUpgradePrice(Weapon weapon)
    {
        int lvl = weapon.info.Level;
        int upgradePrice = 0;
        upgradePrice = UpgradeBasePrice + lvl * (UpgradeIncrementPrice + 5*lvl);
        return upgradePrice;
    }
    void UpgradeValidation()
    {
        ShopItem item = SelectedShopSlot.Item;
        if (item is WeaponShopItem)
        {
            WeaponShopItem weaponShopItem = item as WeaponShopItem;
            Weapon weapon = FindWeaponInPlayerInventory(weaponShopItem.WeaponID);
            if (weapon!=null)
            {
                if(weapon.info.Level < 10)
                {
                    int Price = CheckUpgradePrice(weapon);
                    ConfimationPopUp confimation = DisplayBuyConfirmation(weapon.info.name+" Upgrade",Price);
                    confimation.OnConfirm += () => { Upgrade(weapon); };
                }
            }
        }
    }
    void Upgrade(Weapon weapon)
    {
        if (GameManager.Instance.ChargeGoldPurchase(CheckUpgradePrice(weapon)))
        {
            GameManager.Instance.PlayerInfo.Lvl++;
            weapon.Upgrade();
            UpadateSelectedItemPanel();
        }
            
    }
    Weapon FindWeaponInPlayerInventory(int _id)
    {
        return Player.Instance.Weapons.Where(x => x.ID == _id).FirstOrDefault();
    }

    void ValidateStorage(ShopSlot slot)
    {
        ShopItem item = slot.Item;
        if (!soldItems.Contains(item.Id))
        {
            ConfimationPopUp confimation = DisplayBuyConfirmation(item);
            confimation.OnConfirm += () => { Buy(slot); };
        }
        else
        {
            SelectedShopSlot = slot;
            if (OnSlotSelected != null)
                OnSlotSelected(slot);
        }
    }
    void Deliverpurchase(ShopItem item)
    {
        if (item is WeaponShopItem)
        {
            WeaponShopItem weaponItem = item as WeaponShopItem;
            Transform gunpivot = Player.Instance.transform.Find("GunPivot");
            Weapon weapon = Instantiate<Weapon>(Weapon.Types[weaponItem.WeaponID],gunpivot.position,Quaternion.identity, gunpivot);
            weapon.GetComponent<HingeJoint2D>().connectedBody= Player.Instance.Rigidbody;
            Player.Instance.Weapons.Add(weapon);
            GameManager.Instance.PlayerInfo.Weapons.Add(weapon.info);
        }
    }
    public ConfimationPopUp DisplayBuyConfirmation(string itemName, int itemPrice)
    {
        string m = "Seguro que desea Comprar " + itemName + " por el precio de " + itemPrice.ToString() + " Monedas";
        return MenuManager.Instance.DisplayConfirmation(m, "Confirmacion de Compra");
    }
    public ConfimationPopUp DisplayBuyConfirmation(ShopItem item)
    {
        string m = "Seguro que desea Comprar " + item.Name + " por el precio de " + item.Price.ToString() + " Monedas";
        return MenuManager.Instance.DisplayConfirmation(m, "Confirmacion de Compra"); 
    }
    void BlockSlot(ShopSlot slot)
    {
        slot.Button.interactable = false;
    }
    void DisplayWeaponStats(Weapon weapon)
    {
        DisplayStat("Level", weapon.info.Level);
        DisplayStat("Level", weapon.info.Level);

        

    }
    void DisplayStat(string name, int value)
    {
        GameObject statGO= Instantiate(statSlot, itemStatsPanel);
        statGO.transform.Find("ItenmNameText").GetComponent<TextMeshProUGUI>().text = name;
        statGO.transform.Find("Stat_value").GetComponent<TextMeshProUGUI>().text = name;
    }
    void EquipSelectedSlot()
    {
        ShopItem item = SelectedShopSlot.Item;
        if (item is WeaponShopItem)
        {
            List<Weapon> weapons = Player.Instance.Weapons;
            WeaponShopItem weaponItem = item as WeaponShopItem;
            Weapon weapon = Weapon.Types[weaponItem.WeaponID];
            Player.Instance.EquipWeapon(weapons.Where(x=>x.info.name == weapon.info.name).First());

        }
    }
}

[System.Serializable]
public abstract class ShopItem
{
    private static int idCount;
    private int id;
    private string name;
    private int price;

    public ShopItem(string name, int price)
    {
        idCount++;
        id = idCount;
        this.name = name;
        this.price = price;
    }

    public int Price
    {
        get
        {
            return price;
        }

        set
        {
            price = value;
        }
    }
    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }

    public int Id
    {
        get
        {
            return id;
        }
    }
}

public class WeaponShopItem : ShopItem
{
    private int weaponID;

    public WeaponShopItem(string name, int price, int weaponID) : base(name, price)
    {

        this.WeaponID = weaponID;
    }
    public int WeaponID
    {
        get
        {
            return weaponID;
        }

        set
        {
            weaponID = value;
        }
    }


}
