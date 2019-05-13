using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInfo 
{
    private string name;
    private int lvl;
    private int gold;
    private int gems;
    private List<InvSlot> inventorty;
    private List<WeaponInfo> weapons;

    public PlayerInfo()
    {
    }
    public PlayerInfo(string name)
    {
        this.name = name;
        lvl=0;
        gold=0;
        gems=0;
    }
    #region Accesores
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
    public int Lvl
    {
        get
        {
            return lvl;
        }

        set
        {
            lvl = value;
        }
    }
    public int Gold
    {
        get
        {
            return gold;
        }

        set
        {
            gold = value;
        }
    }
    public int Gems
    {
        get
        {
            return gems;
        }

        set
        {
            gems = value;
        }
    }
    public List<InvSlot> Inventorty
    {
        get
        {
            return inventorty;
        }

        set
        {
            inventorty = value;
        }
    }
    public List<WeaponInfo> Weapons
    {
        get
        {
            return weapons;
        }

        set
        {
            weapons = value;
        }
    }
    #endregion

    public void Save()
    {
        XMLManager.SaveData(this, XMLManager.PLAYERINFO_FOLDER_NAME, name+".xml");
    }
}
