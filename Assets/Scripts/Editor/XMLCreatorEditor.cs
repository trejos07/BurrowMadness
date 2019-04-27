using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(XMLCreator))]
public class XMLCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        XMLCreator creator = (XMLCreator)target;

        if (GUILayout.Button("Save Info"))
        {
            XMLManager.SaveData(creator.WorldNames, XMLManager.WORLDINFO_FOLDER_NAME + "WorldNames" + ".xml");
            EditorUtility.DisplayDialog("Guadado de Datos", "se guardo correctamente la informacion ", "Ok", "");

        }


    }
}
