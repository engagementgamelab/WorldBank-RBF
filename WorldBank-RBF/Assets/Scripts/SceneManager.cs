/* 
World Bank RBF
Created by Engagement Lab, 2015
==============
 SceneManager.cs
 Unity scene management. Mostly handles data to/from static DataManager, but applying it only to this scene. Should likely be inside of any scene.

 Created by Johnny Richardson on 4/13/15.
==============
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SceneManager : MonoBehaviour {

	public string cityName;

	void Awake () {

		// We need our game config data before calling any remote endpoints
		LoadGameConfig();
	
		// Set global game data if needed
		SetGameData();

		DataManager.SetSceneContext(cityName);

	}

	void Start () {

	}

	/// <summary>
	/// Obtains game config data and passes it to global data manager
	/// </summary>
	private void LoadGameConfig()
	{
		// Open stream to API JSON config file
		StreamReader reader = new StreamReader(Application.dataPath + "/Config/api.json");
		string strConfigData = reader.ReadToEnd();

		// Set in data manager class
		DataManager.SetGameConfig(strConfigData);
	}

	/// <summary>
	/// Obtains and sets global game data
	/// </summary>
	private void SetGameData() {

		string gameData = null;

		// This should live in a static global dictionary somewhere
		// Try to get data from API remote
		try {

			gameData = NetworkManager.Instance.DownloadDataFromURL("/gameData");

		}
		// Fallback: load game data from local config
		catch {
 
	        StreamReader reader = new StreamReader(Application.dataPath + "/Config/data.json");
	        
			gameData = reader.ReadToEnd();
		
		}

		// Set global game data
		DataManager.SetGameData(gameData);

	}
}
