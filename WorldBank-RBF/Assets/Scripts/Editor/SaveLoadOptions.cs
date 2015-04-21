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
    string prevCityName;
	GUILayoutOption miniButtonWidth = GUILayout.Width (40f);

	SaveState saveState = SaveState.Unsaved;
	enum SaveState {
		Unsaved,
        Overwrite,
        EmptyName,
		Successful
	}

	LoadState loadState = LoadState.Unloaded;
	enum LoadState {
		Unloaded,
        EmptyName,
        FileDoesNotExist,
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

        if (cityName != prevCityName) {
            saveState = SaveState.Unsaved;
        }

		GUI.color = Color.white;
        EditorGUILayout.BeginHorizontal ();
        cityName = EditorGUILayout.TextField ("City Name: ", cityName);
        if (LayerSettings == null || LayerSettings.Count > 0) {
            if (GUILayout.Button ("Save", EditorStyles.miniButtonLeft, miniButtonWidth)) {
            	loadState = LoadState.Unloaded;
                saveState = Save ();
            }
        }
        if (GUILayout.Button ("Load", EditorStyles.miniButtonRight, miniButtonWidth)) {
        	saveState = SaveState.Unsaved;
            loadState = Load ();
        }
        EditorGUILayout.EndHorizontal ();

        if (saveState == SaveState.EmptyName) {
            EditorGUILayout.HelpBox ("Could not save. Please enter a city name!", MessageType.Error);
        } else if (saveState == SaveState.Overwrite) {
            DrawOverwriteOption ();
        } else if (saveState == SaveState.Successful) {
            EditorGUILayout.HelpBox ("City saved! :)", MessageType.Info);
        } 

        if (loadState == LoadState.EmptyName) {
            EditorGUILayout.HelpBox ("Could not load. Please enter a city name!", MessageType.Error);
        } else if (loadState == LoadState.FileDoesNotExist) {
            EditorGUILayout.HelpBox ("File does not exist :'(", MessageType.Error);
        } else if (loadState == LoadState.Successful) {
            EditorGUILayout.HelpBox ("City loaded! :P", MessageType.Info);
        }
    }

    void DrawOverwriteOption () {
        EditorGUILayout.BeginHorizontal ();
        GUILayout.Label (string.Format ("A file named \"{0}\" already exists. Overwrite?", cityName));
        if (GUILayout.Button ("Yes", EditorStyles.miniButtonLeft, miniButtonWidth)) {
            SaveLayerSettings (true);
        }
        if (GUILayout.Button ("Cancel", EditorStyles.miniButtonRight, miniButtonWidth)) {
            saveState = SaveState.Unsaved;
        }
        EditorGUILayout.EndHorizontal ();
    }

    SaveState Save () {
        if (cityName == "") return SaveState.EmptyName;
        if (SaveLayerSettings ()) {
            return SaveState.Successful;
        } else {
            return SaveState.Overwrite;
        }
    }

    LoadState Load () {
        if (cityName == "") return LoadState.EmptyName;
        if (!File.Exists (CITY_PATH)) {
            return LoadState.FileDoesNotExist;
        }
        PhaseOneCity phaseOne = JsonReader.Deserialize<PhaseOneCity> (ReadJsonData (CITY_PATH));
        parallaxSceneDesignerOptions.Load (phaseOne.GetLayers (), phaseOne.GetLayerCount ());
        return LoadState.Successful;
    }

    bool SaveLayerSettings (bool allowOverwrite=false) {
    	
        if (!allowOverwrite && File.Exists (CITY_PATH)) {
            return false;
        }

    	List<LayerSettingsJson> layers = LayerSettings.ConvertAll (x => x.Json);
		PhaseOneCity phaseOne = new PhaseOneCity ();
		phaseOne.SetCityName (cityName);
		phaseOne.SetLayers (layers);
		phaseOne.SetLayerCount (LayerCount);

        if (!Directory.Exists (PATH)){
            Directory.CreateDirectory (PATH);
        }
        WriteJsonData (CITY_PATH, JsonWriter.Serialize(phaseOne));
        return true;
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