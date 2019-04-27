using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MisionManager : MonoBehaviour
{
    public static MisionManager Instance;
    [SerializeField] int baseRedward=100;
    List<List<MisionInfo>> misionsPerWorld;
    List<List<subMisionInfo>> posibleSubMisionsPerWorld = new List<List<subMisionInfo>>();


    public delegate void InfoChangeCallback();
    public event InfoChangeCallback OnInfoChange;

    public List<List<MisionInfo>> MisionsPerWorld
    {
        get
        {
            return misionsPerWorld;
        }
        protected set 
        {
            misionsPerWorld = value;
            OnInfoChange();
        }
       
    }

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }
        misionsPerWorld = XMLManager.LoadData<List<List<MisionInfo>>>(XMLManager.MISIONS_FOLDER_NAME+XMLManager.MISIONS_FILE_NAME);

    }
    private void Start()
    {
        if(misionsPerWorld ==null || misionsPerWorld.Count==0)
        {
            misionsPerWorld = new List<List<MisionInfo>>();
            for (int i = 0; i < GameManager.ins.Worlds.Count; i++)
            {
                posibleSubMisionsPerWorld.Add(createWorldSubmisions(GameManager.ins.Worlds[i]));
                List<MisionInfo> wMisions = createWorldMisions(GameManager.ins.Worlds[i], 5);
                misionsPerWorld.Add(wMisions);
            }
            SaveMisions();
        }
    }
    public List<subMisionInfo>GetSubMisionInfos(WorldInfo info, int nroSubMisions)
    {
        int n = GameManager.ins.Worlds.IndexOf(info);
        List<subMisionInfo> subMisionInfos = new List<subMisionInfo>();

        for (int i = 0; i < nroSubMisions; i++)
        {
            subMisionInfos.Add(posibleSubMisionsPerWorld[n][Random.Range(0, posibleSubMisionsPerWorld[n].Count)]);
        }
        subMisionInfos = subMisionInfos.GroupBy(x => x.ItemName).Select(g => g.OrderByDescending(x => x.Amount).First()).ToList();

        return subMisionInfos;
    }
    public List<subMisionInfo> createWorldSubmisions(WorldInfo info)
    {
        List<subMisionInfo> subMisions = new List<subMisionInfo>();
        foreach (int itemId in info.worldSources)
        {
            Item item = Item.GetItemOfID(itemId);
            for (int i = 1; i <= 6; i++)
            {
                subMisions.Add(new subMisionInfo(item.itemName, i * 5, i * 25,false));
            }
        }
        return subMisions;
    }
    public List<MisionInfo> createWorldMisions(WorldInfo info, int nroMisions)
    {
        List<MisionInfo> misions = new List<MisionInfo>();
        for (int i = 0; i < nroMisions; i++)
        {
            int d = 2 + i;
            misions.Add(new MisionInfo("Recoleccion",info.name, d, GetSubMisionInfos(info,d), baseRedward + 10*i*d));
        }

        return misions;
    }
    public void SaveMisions()
    {
        XMLManager.SaveData(misionsPerWorld,XMLManager.MISIONS_FOLDER_NAME+XMLManager.MISIONS_FILE_NAME+ ".xml");

    }
    public MisionInfo GetMisionFromWorld(WorldInfo world)
    {
        int n = GameManager.ins.Worlds.IndexOf(world);
        List<MisionInfo> misions = misionsPerWorld[n];
        return misions[Random.Range(0, misions.Count)];

    }
    public void ClaimMisionRedward(MisionInfo info)
    {
        int i = GameManager.ins.GetWorldIndex(info.wname);
        if (info.completed && misionsPerWorld[i].Contains(info))
        {
            GameManager.ins.SetGoldRedward(info.reward);
            MisionsPerWorld[i].Remove(info);
            SaveMisions();
        }
            
    }
}
