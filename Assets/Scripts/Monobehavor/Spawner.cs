using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject prefabToSpawn;
    [SerializeField] float spawnRatio;
    [SerializeField] float maxVariationSRatio;
    [SerializeField] float radiusSpawnArea;
    [SerializeField] int maxPrefabsInArea;
    [SerializeField] LayerMask layerPrefab;

    Vector3 worldPos;

    bool isSpawning=false;

    public Vector3 WorldPos
    {
        get
        {
            return worldPos;
        }

        set
        {
            worldPos = value;
        }
    }

    public float RadiusSpawnArea
    {
        get
        {
            return radiusSpawnArea;
        }

        set
        {
            radiusSpawnArea = value;
        }
    }

    public static Spawner CreateComponent(GameObject where)
    {
        Spawner myC = where.AddComponent<Spawner>();
        return myC;
    }

    private void Start()
    {
        StartCoroutine(Spawning());
    }

    public int CountPrefabsInArea()
    {
        return Physics2D.OverlapCircleAll(transform.position, radiusSpawnArea, layerPrefab).Length;
    }

    public IEnumerator Spawning()
    {
        isSpawning = true;
        while(CountPrefabsInArea()<maxPrefabsInArea)
        {
            float nextSpawnR = Random.Range(spawnRatio - maxVariationSRatio, spawnRatio + maxVariationSRatio);
            yield return new WaitForSeconds(nextSpawnR);
            Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        }
        isSpawning = false;
        StartCoroutine(CheckArea());

    }

    public IEnumerator CheckArea()
    {
        while (!isSpawning)
        {
            if (CountPrefabsInArea() < maxPrefabsInArea)
                StartCoroutine(Spawning());

            yield return new WaitForSeconds(spawnRatio);

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radiusSpawnArea);
    }

}
