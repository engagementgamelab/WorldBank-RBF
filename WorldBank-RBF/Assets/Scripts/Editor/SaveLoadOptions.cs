using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JsonFx.Json;

[Serializable]
public class SaveLoadOptions : ScriptableObject {

	ParallaxSceneDesignerOptions parallaxSceneDesignerOptions;
	string cityName;
	GUILayoutOption miniButtonWidth = GUILayout.Width (40f);

	public List<LayerSettings> LayerSettings { get; set; }
	public int LayerCount { get; set; } 

	string PATH { get { return Application.dataPath + "/Config/PhaseOne/Cities/"; } }

	public void Init (ParallaxSceneDesignerOptions parallaxSceneDesignerOptions) {
		this.parallaxSceneDesignerOptions = parallaxSceneDesignerOptions;
	}

	public void OnEnable () {
        hideFlags = HideFlags.HideAndDontSave;
    }

	public void OnGUI () {

		GUI.color = Color.white;
        EditorGUILayout.BeginHorizontal ();

        cityName = EditorGUILayout.TextField ("City Name: ", cityName);
        if (LayerSettings == null || LayerSettings.Count > 0) {
            if (GUILayout.Button ("Save", EditorStyles.miniButtonLeft, miniButtonWidth)) {
                string savePath = EditorUtility.SaveFilePanel ("Save city", PATH, cityName + ".json", "json");
                Save (savePath);
            }
        }
        if (GUILayout.Button ("Load", EditorStyles.miniButtonRight, miniButtonWidth)) {
            string loadPath = EditorUtility.OpenFilePanel ("Load a city", PATH, "json");
            Load (loadPath);
        }
        EditorGUILayout.EndHorizontal ();
    }

    void Save (string savePath) {
        SaveLayerSettings (savePath);
    }

    void Load (string loadPath) {
        if (loadPath == "") return;
        PhaseOneCity phaseOne = JsonReader.Deserialize<PhaseOneCity> (ReadJsonData (loadPath));
        parallaxSceneDesignerOptions.Load (phaseOne.GetLayers (), phaseOne.GetLayerCount ());
        cityName = phaseOne.GetCityName ();
    }

    void SaveLayerSettings (string savePath, bool allowOverwrite=false) {
    	List<LayerSettingsJson> layers = LayerSettings.ConvertAll (x => x.Json);
		PhaseOneCity phaseOne = new PhaseOneCity ();
		phaseOne.SetCityName (cityName);
		phaseOne.SetLayers (layers);
		phaseOne.SetLayerCount (LayerCount);
        WriteJsonData (savePath, JsonWriter.Serialize(phaseOne));
    }

    // TODO: Move this to DataManager
    string ReadJsonData (string path) {
    	StreamReader streamReader = new StreamReader (path);
    	string data = streamReader.ReadToEnd ();
    	streamReader.Close ();
    	return data;
    }

    // TODO: Move this to DataManager
    void WriteJsonData (string path, string data) {
		var streamWriter = new StreamWriter(path);
        streamWriter.Write(data);
        streamWriter.Close();
    }
}