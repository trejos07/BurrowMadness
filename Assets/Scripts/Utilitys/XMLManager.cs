using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System;

public class XMLManager : MonoBehaviour
{
    public static XMLManager ins;

    public subMisionDataBase subMisionDB;

    private void Awake()
    {
        ins = this;
    }

    public void SaveSubMisions ()
    {
        
        XmlSerializer serializer = new XmlSerializer(typeof(subMisionDataBase));
        FileStream stream = new FileStream(Application.dataPath + "/Resources/XML/SubMisionsData.xml", FileMode.Create);
        serializer.Serialize(stream, subMisionDB);
        stream.Close();
    }

    public subMisionDataBase LoadSubmisions()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(subMisionDataBase));
        FileStream stream = new FileStream(Application.dataPath + "/Resources/XML/SubMisionsData.xml", FileMode.Open);
        subMisionDB = serializer.Deserialize(stream) as subMisionDataBase;
        stream.Close();
        return subMisionDB;
    }

}


