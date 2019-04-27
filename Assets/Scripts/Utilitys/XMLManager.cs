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

    public static string BaseFilesPath;

    private void Awake()
    {
        if (Application.isEditor)
            BaseFilesPath =  Application.dataPath+"/Resources/";
        else
            BaseFilesPath = Path.Combine(Application.persistentDataPath , "/Resources/");
        
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

    public static T LoadData<T>(string Path)
        where T : class
    {
        T data = null;
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        Stream stream = null;
        TextAsset text = Resources.Load<TextAsset>(Path);

        if (text != null)
        {
            stream = GenerateStreamFromString(text.text);
            if (stream != null)
            {
                data = serializer.Deserialize(stream) as T;
                stream.Close();
            }
        }
            
        else
            Debug.Log("no se encontro ningun archivo en la ruta especificada");
        
        return data;
    }

    public static List<T> LoadFolderData<T>(string Path)
        where T : class
    {
        List<T> data = new List<T>();
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        TextAsset[] text = Resources.LoadAll<TextAsset>(Path);
        
        foreach (TextAsset t in text)
        {
            Stream stream = null;
            stream = GenerateStreamFromString(t.text);
            if (stream != null)
                data.Add(serializer.Deserialize(stream) as T);
            stream.Close();
        }
        
        return data;
    }

    //public static T LoadData<T>(string directory, string fileName)
    //   where T : class
    //{
    //    T data = null;
    //    XmlSerializer serializer = new XmlSerializer(typeof(T));
    //    Stream stream = null;
    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        string oriPath = Path.Combine(Application.streamingAssetsPath, fileName);

    //        UnityWebRequest reader = UnityWebRequest.Get(oriPath);
    //        reader.SendWebRequest();
    //        while (!reader.isDone) { }
    //        if (string.IsNullOrEmpty(reader.error))
    //        {
    //            string fileContents = reader.downloadHandler.text;
    //            stream = GenerateStreamFromString(fileContents);
    //        }

    //    }
    //    else
    //    {
    //        stream = new FileStream(BaseFilePath + SUBMISIONS_FILE_NAME, FileMode.Open);
    //    }

    //    if (stream != null)
    //        data = serializer.Deserialize(stream) as T;

    //    stream.Close();
    //    return data;
    //}

    public static void SaveData<T>(T data, string fileName )
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));

        var fname = Path.Combine(BaseFilesPath,fileName);
        var appendMode = false;
        var encoding = Encoding.GetEncoding("UTF-8");

        using (StreamWriter sw = new StreamWriter(fname, appendMode, encoding))
        {
            serializer.Serialize(sw, data);
        }

        //XmlSerializer serializer = new XmlSerializer(typeof(T));
        //FileStream stream = new FileStream(, FileMode.Create);
        //serializer.Serialize(stream, data);
        //stream.Close();

    }

}


