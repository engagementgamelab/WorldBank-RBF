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

	SaveState saveState = SaveState.Unsaved;
	enum SaveState {
		Unsaved,
		Unsuccessful,
		Successful
	}

	LoadState loadState = LoadState.Unloaded;
	enum LoadState {
		Unloaded,
		Unsuccessful,
		Successful
	}

	public List<LayerSettings> LayerSettings { get; set; }
	public int LayerCount { get; set; }

	string PATH { get { return Application.dataPath + "/Config/PhaseOne/Cities/"; } }
	string CITY_PATH { get { return PATH + cityName + ".json"; } }

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
        if (GUILayout.Button ("Save", EditorStyles.miniButtonLeft, miniButtonWidth)) {
        	loadState = LoadState.Unloaded;
            bool saveSuccessful = Save ();
            if (saveSuccessful) {
            	saveState = SaveState.Successful;
            } else {
            	saveState = SaveState.Unsuccessful;
            }
        }
        if (GUILayout.Button ("Load", EditorStyles.miniButtonRight, miniButtonWidth)) {
        	saveState = SaveState.Unsaved;
        	bool loadSuccesful = Load ();
        	if (loadSuccesful) {
        		loadState = LoadState.Successful;
        	} else {
        		loadState = LoadState.Unsuccessful;
        	}
        }
        EditorGUILayout.EndHorizontal ();

        if (saveState == SaveState.Unsuccessful) {
            EditorGUILayout.HelpBox ("Could not save. Please enter a city name!", MessageType.Error);
        } else if (saveState == SaveState.Successful) {
        	EditorGUILayout.HelpBox ("City saved! :)", MessageType.Info);
        }

        if (loadState == LoadState.Unsuccessful) {
        	EditorGUILayout.HelpBox ("Could not load. Please enter a city name!", MessageType.Error);
        } else if (loadState == LoadState.Successful) {
        	EditorGUILayout.HelpBox ("City loaded! :P", MessageType.Info);
        }
    }

    bool Save () {
    	if (cityName == "") return false;
        SaveLayerSettings ();
        return true;
    }

    bool Load () {
    	if (cityName == "") return false;
    	PhaseOneCity phaseOne = JsonReader.Deserialize<PhaseOneCity> (ReadJsonData (CITY_PATH));
    	parallaxSceneDesignerOptions.Load (phaseOne.GetLayers (), phaseOne.GetLayerCount ());
    	return true;
    }

    void SaveLayerSettings () {
    	
    	List<LayerSettingsJson> layers = LayerSettings.ConvertAll (x => x.Json);
		PhaseOneCity phaseOne = new PhaseOneCity ();
		phaseOne.SetCityName (cityName);
		phaseOne.SetLayers (layers);
		phaseOne.SetLayerCount (LayerCount);

        if (!Directory.Exists (PATH)){
            Directory.CreateDirectory (PATH);
        }
        WriteJsonData (CITY_PATH, JsonWriter.Serialize(phaseOne));
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