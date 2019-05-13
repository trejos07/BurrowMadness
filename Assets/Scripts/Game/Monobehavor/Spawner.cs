using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] int EnemyID;
    [SerializeField] float spawnRatio;
    [SerializeField] float maxVariationSRatio;
    [SerializeField] float radiusSpawnArea;
    [SerializeField] int maxPrefabsInArea;
    [SerializeField] LayerMask layerPrefab;


    Vector3 worldPos;
    List<Node> nodes;

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
        NodeNavMesh navMesh = FindObjectOfType<NodeNavMesh>();
        Node mNode = navMesh.NodeFromWorldPoint(transform.position);
        nodes = navMesh.GetNodesArraund(mNode, ((int)RadiusSpawnArea/2));
        StartCoroutine(Spawning());
    }
    public int CountPrefabsInArea()
    {
        int n = Physics2D.OverlapCircleAll(transform.position, radiusSpawnArea, layerPrefab).Length;
        return n;
    }
    public void Spawn ()
    {
        Vector3 wPos = nodes[Random.Range(0, nodes.Count)].WorldPos;
        Enemy e =  GameplayManager.Instance.WorldEnemyFactory.GetEnemyAt(wPos, transform.rotation.z);
    }
    public IEnumerator Spawning()
    {
        isSpawning = true;
        while(CountPrefabsInArea()<maxPrefabsInArea)
        {

            float nextSpawnR = Random.Range(spawnRatio - maxVariationSRatio, spawnRatio + maxVariationSRatio);
            yield return new WaitForSeconds(nextSpawnR);
            Spawn();
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
