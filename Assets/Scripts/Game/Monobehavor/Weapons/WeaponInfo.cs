using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
public class WeaponInfo
{
    public string name;
    private int level;
    public int bulletID;
    public WeaponColor selectedColor;
    public List<WeaponColor> colors;
    public float baseFireRate;
    public float baseAngularSpeed;
    private float bulletSpeedMultiplier=1;

   
    [XmlIgnore]
    public Bullet BulletType
    {
        get
        {
            return Bullet.Types[bulletID];
        }

        set
        {
            bulletID = value.ID;
        }
    }
    public float FireRate
    {
        get
        {
            return baseFireRate - 0.05f*Level;
        }
    }
    public float Velocidad_Rotacion
    {
        get
        {
            return baseAngularSpeed + 2.5f*Level;
        }
        
    }
    public float BulletDamageMultiplier
    {
        get
        {
            return 1+level/10;
        }
    }
    public float BulletSpeedMultiplier
    {
        get
        {
            return bulletSpeedMultiplier;
        }
    }
    public int Level
    {
        get
        {
            return level;
        }

        set
        {
            level = value;
        }
    }
    public int Damage
    {
        get
        {
            return UnityEngine.Mathf.FloorToInt(BulletType.Damage*BulletDamageMultiplier);
        }
    }
    public List<WeaponColor> Colors
    {
        get
        {
            return colors;
        }

        set
        {
            colors = value;
        }
    }
    

    public WeaponInfo()
    {
    }
}

