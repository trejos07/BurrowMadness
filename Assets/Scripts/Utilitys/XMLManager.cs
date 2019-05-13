using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System;
using UnityEngine.Networking;
using System.Text;

[ExecuteAlways]
public class XMLManager : MonoBehaviour
{

    public const string MISIONS_FILE_NAME = "MisionsData";

    public const string CHUNKINFO_FOLDER_NAME = "XML/ChunkInfo/";
    public const string WORLDINFO_FOLDER_NAME = "XML/WorldInfo/";
    public const string PLAYERINFO_FOLDER_NAME = "XML/PlayerInfo/";
    public const string MISIONS_FOLDER_NAME = "XML/Misions/";

    public static string baseFilesPath;
    public static string saveFilesPath;

    private void Awake()
    {
#if UNITY_EDITOR
        
        baseFilesPath = Application.dataPath + "/StreamingAssets/";
        saveFilesPath = baseFilesPath;

#elif UNITY_IOS
        baseFilesPath = Application.streamingAssetsPath + "/Raw";
        
#elif UNITY_ANDROID
        baseFilesPath = Application.streamingAssetsPath+"/";
        saveFilesPath = Application.persistentDataPath+"/";
#endif
    }
    public static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();

        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
    public static string ReadFile(string filePath)
    {
        string text = "";
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            UnityWebRequest www = UnityWebRequest.Get(filePath);
            www.SendWebRequest();
            while (!www.isDone) { Debug.Log("Waiting for Request"); }
            text = www.downloadHandler.text;
        }
        else
        {
            Debug.Log("comprobando  si el path Existe" + filePath);

            if (File.Exists(filePath))
                text = File.ReadAllText(filePath);
            else
            {
                Debug.Log(" el path NO Existe pero el directorio existe ?"+ Directory.Exists(filePath));
            }
        }
        return text;
    }


    public static T LoadData<T>(string path)
        where T : class
    {
        T data = null;
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        Stream stream = null;
        string text = "";
        string filePath = saveFilesPath + path+".xml";

        if(File.Exists(filePath))
            text= ReadFile(filePath);
        else
        {
            filePath = baseFilesPath+path+".xml";
            text = ReadFile(filePath);
        }

        if (text != null&&text!="")
        {
            stream = GenerateStreamFromString(text);
            if (stream != null)
            {
                data = serializer.Deserialize(stream) as T;
                stream.Close();
            }
        }
        else
            Debug.Log("no se encontro ningun archivo en la ruta "+ filePath );
        
        return data;
    }
    public static List<T> LoadFolderData<T>(string _path)
        where T : class
    {
        List<T> datas = new List<T>();

        string folderPath = Path.Combine(saveFilesPath, _path);

        if(!Directory.Exists(folderPath))
        {
            Debug.Log(String.Format("el Folder {0} NO existe!", folderPath));
            folderPath = Path.Combine(baseFilesPath, _path);
        }
        if (Directory.Exists(folderPath))
        {
            string[] files = Directory.GetFiles(folderPath);
            foreach (string path in files)
            {
                if (path.Contains(".meta")) continue;
                string fileName = Path.GetFileNameWithoutExtension(path);
                T data = LoadData<T>(Path.Combine(_path, fileName));
                if (data != null)
                    datas.Add(data);
            }
        }
        else Debug.Log(String.Format("el Folder {0} NO existe!", folderPath));

        return datas;
    }
    public static List<T> LoadFromResourcesData<T>(string _path)
        where T : class
    {
        List<T> datas = new List<T>();
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        TextAsset[] texts = Resources.LoadAll<TextAsset>(_path);

        foreach (TextAsset text in texts)
        {
            Stream stream = GenerateStreamFromString(text.text);
            if (stream!=null)
            {
                datas.Add(serializer.Deserialize(stream) as T);
                stream.Close();
            }
        }

        

        return datas;
    }
    public static void SaveData<T>(T data, string folderName, string fileName )
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        string path = Path.Combine(saveFilesPath, folderName);
        Directory.CreateDirectory(path);
        var fname = Path.Combine(path, fileName);
        var appendMode = false;
        //Encoding utf8WithoutBom = new UTF8Encoding(true);
        var encoding = new UTF8Encoding(false);

        using (StreamWriter sw = new StreamWriter(fname, appendMode, encoding))
        {
            serializer.Serialize(sw, data);
        }
    }

}


