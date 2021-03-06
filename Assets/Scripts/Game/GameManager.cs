﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    Player playerIns;

    [SerializeField] Camera guiCamera;

    private static GameManager instance;
    public GameSettings settings;
    private PlayerInfo playerInfo;
    private WorldInfo selectedWorld;
    private MisionInfo activeMision;
    private List<WorldInfo> worlds;
    WorldInfo tutuorial;
    private bool onGamplay;


    #region Accesores
    public static GameManager Instance
    {
        get
        {
            return instance;
        }

        set
        {
            instance = value;
        }
    }
    public WorldInfo SelectedWorld
    {
        get
        {
            return selectedWorld;
        }

        set
        {
            selectedWorld = value;
            if (OnSelecWorld != null && value != null)
                OnSelecWorld(value);
        }
    }
    public PlayerInfo PlayerInfo
    {
        get
        {
            return playerInfo;
        }

        set
        {
            playerInfo = value;
        }
    }
    public List<WorldInfo> Worlds
    {
        get
        {
            return worlds;
        }
    }
    public GameSettings Settings
    {
        get
        {
            return settings;
        }
    }
    public MisionInfo ActiveMision
    {
        get
        {
            return activeMision;
        }

        set
        {
            activeMision = value;
        }
    }
    public bool OnGamplay
    {
        get
        {
            return onGamplay;
        }

        set
        {
            onGamplay = value;
        }
    }
    #endregion

    public delegate void Action(); 
    public event Action OnLevelLoaded;
    public delegate void WorldAction(WorldInfo world);
    public event WorldAction OnSelecWorld;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);

        SceneManager.sceneLoaded += OnSceneLoaded;
        Item.ItemsList = new List<Item>(Resources.LoadAll<Item>("Items"));
        TerrainTile.tileList = new List<TerrainTile>(Resources.LoadAll<TerrainTile>("Tiles"));
        

       

    }
    private void Start()
    {
        worlds = new List<WorldInfo>();

        if (settings.tutorialOn)
        {
            tutuorial = XMLManager.LoadData<WorldInfo>(XMLManager.WORLDINFO_FOLDER_NAME + "Tutorial/Tutorial");
            worlds.Add(tutuorial);
            SelectedWorld = tutuorial;
            //TutorialManager.Instance.MainTutorial.OnTutorialEnd += EndTutorial;
        }
        else
            worlds = GetWorlds();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {

        }
        else if (scene.buildIndex == 1)
        {
            OnGamplay = true;
            if (OnLevelLoaded != null)
            {
                OnLevelLoaded();
            }
        }

    }
    
    void OnStartGamplay()
    {
        
    }
    void EndTutorial()
    {
        worlds.Remove(tutuorial);
        worlds = GetWorlds();
    }
    List<WorldInfo> GetWorlds()
    {
        if (worlds.Count == 0)
        {
            List<WorldInfo> LoadedWorlds = XMLManager.LoadFolderData<WorldInfo>(XMLManager.WORLDINFO_FOLDER_NAME+ "GeneratedWorlds");
            int remanigForCreate = settings.numOfWorlds - LoadedWorlds.Count;

            if (remanigForCreate >0)
            {
                for (int i = 0; i < remanigForCreate; i++)
                {
                    WorldInfo info = GenerateWorldInfo(settings.dificulty);
                    info.Save();
                    LoadedWorlds.Add(info);

                }
            }
                worlds = LoadedWorlds;
            SelectedWorld = worlds[0];
        }
        
        return worlds;
    }
    public WorldInfo GetWorldInfo(string name)
    {
        return worlds.Find(x => x.name == name);
    }
    public int GetWorldIndex(string name)
    {
        return GetWorldIndex(GetWorldInfo(name));
    }
    public int GetWorldIndex(WorldInfo info)
    {
        return worlds.IndexOf(info);
    }
    WorldInfo GenerateWorldInfo(WorldDificulty dificulty)
    {
        Debug.Log("generando un mundo...");
        List<string> names = XMLManager.LoadData<List<string>>(XMLManager.WORLDINFO_FOLDER_NAME+"WorldNames");

        string name = names[UnityEngine.Random.Range(0, names.Count)];
        int d = (int)dificulty;
        int nDoungeons=0;
        for (int i = 0; i < d*2; i++)
        {
            nDoungeons += Random.Range(0, 100)<=75? 1:0;
        }
        
        int maxSpawners = Random.Range(d,d+2)+2;
        int nResources = Random.Range(d, d + 2) + 1;
        float gravity = Random.Range(-15f, -5f);

        Vector2Int worldSizeInChunks = new Vector2Int(Random.Range(d, d + 3)+1, Random.Range(d, d + 3)+1);
        ChunkInfo[][] chunks = null;
        Color color = Color.HSVToRGB(Random.Range(0f,1f), Random.Range(0.6f,1f), Random.Range(0.5f, 0.75f));
        List<Item> worldSources = new List<Item>();

        while(nResources>0)
        {
            Item posibleItem = Item.ItemsList[Random.Range(0, Item.ItemsList.Count)];
            if (!worldSources.Contains(posibleItem))
            {
                worldSources.Add(posibleItem);
                nResources--;
            }
        }

        return new WorldInfo(name, maxSpawners,nDoungeons, worldSizeInChunks, Item.IdsFormItems(worldSources.ToArray()), chunks, color,gravity);

    }
    public void LoaadLevel(WorldInfo info)
    {
        SelectedWorld = info; 
        SceneManager.LoadScene(1);
        MenuManager.Instance.TurnOffAllMenus();
        guiCamera.gameObject.SetActive(false);
    }
    public void LoadLobby()
    {
        GameplayManager.Instance.SaveSesion();
        SelectedWorld.Save();
        SelectedWorld = null;
        SceneManager.LoadScene(0);
        guiCamera.gameObject.SetActive(false);
        guiCamera.gameObject.SetActive(true);
        MenuManager.Instance.OpenMenu("Main_Menu");
        MenuManager.Instance.OpenMenu("Lobby_Menu");
    }
    public void CheckPlayerInfo()
    {
        List<PlayerInfo> infos =  XMLManager.LoadFolderData<PlayerInfo>(XMLManager.PLAYERINFO_FOLDER_NAME);
        if (infos.Count > 0)
        {
            PlayerInfo = infos[0];
            MenuManager.Instance.OpenMenu("Lobby_Menu");
            MenuManager.Instance.SetPlayerInfo();
        }
        else
            MenuManager.Instance.CreateRegisterMenu();
    }
    public void CreatePlayerInfo(string name)
    {
        PlayerInfo = new PlayerInfo(name);
        PlayerInfo.Save();
        MenuManager.Instance.OpenMenu("Lobby_Menu");
        MenuManager.Instance.SetPlayerInfo();
    }
    public void SetGoldRedward(int redward)
    {
        playerInfo.Gold += redward;
        PlayerInfo.Save();
    }
    public void SetGemsRedward(int redward)
    {
        playerInfo.Gems += redward;
    }
    public bool ChargeGoldPurchase(int amout)
    {
        if (playerInfo.Gold - amout >= 0)
        {
            playerInfo.Gold -= amout;
            PlayerInfo.Save();
            return true;
        }
        return false;
        
    }


}

public enum WorldDificulty { easy = 1, medium, hard }
