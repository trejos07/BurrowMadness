using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathLimit : MonoBehaviour
{
    private Vector3 dir;
    private Vector3 pos;
    private Vector3 size;
    private float deathTime=0;

    Coroutine waitForDestroy;

    public static DeathLimit CreateDeathLimit(GameObject parent, Vector3 axis, Vector3 pos, Vector3 size)
    {
        GameObject limit = new GameObject("DeathLimith");
        limit.transform.parent = parent.transform;
        limit.SetActive(false);
        DeathLimit myC = limit.AddComponent<DeathLimit>();

        myC.dir = axis;
        myC.pos = pos;
        myC.size = size;

        limit.SetActive(true);

        return myC;
    }

	private void Awake()
	{
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;

        float dix = pos.x / Mathf.Abs(pos.x);
        Vector2 dirOffset = new Vector2(dir.y, -dir.x);
        Vector3 s = new Vector3(size.x * Mathf.Abs(dir.x), size.y * Mathf.Abs(dir.y), 0);


        transform.position = pos;
        collider.offset = new Vector2(dirOffset.x*size.x/2, dirOffset.y * size.y / 2);
        collider.size = size + s* 20 * 1.75f;
        SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();

    }

    void ReLoadScene()
    {
        AdsManager.DisplayAds(AdsManager.AD_3MIN);
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
        if(collision.transform.tag=="Player")
        {
            waitForDestroy = StartCoroutine(WarningWaiting());
        }
        
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
        if (collision.transform.tag == "Player")
            StopCoroutine(waitForDestroy);
	}

    public IEnumerator WarningWaiting()
    {
        float t = 0;
        while (true)
        {
            t += Time.deltaTime;
            yield return null;

            if(t>7)
            {
                ReLoadScene();
                break;
            }

        }

    }

}
