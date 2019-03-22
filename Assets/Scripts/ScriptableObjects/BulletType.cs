using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class BulletType : ScriptableObject
{
    public static List<BulletType> types = new List<BulletType>();

    public string name;
    public Sprite bSprite;
    public Color color;
    public ParticleSystem bparticle;
    public bool isAnimated;
    public int damage;
    public float Velocidad_Bala;

	private void OnEnable()
	{
        if (types != null && !types.Contains(this))
            types.Add(this);


	}


}
