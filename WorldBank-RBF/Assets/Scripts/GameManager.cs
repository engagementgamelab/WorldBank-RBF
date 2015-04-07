using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GameManager : MonoBehaviour {

	void Awake () {

		LoadGameConfig();

	}

	// Use this for initialization
	void Start () {

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

		DataManager.SetGameData(gameData);

		Debug.Log( DataManager.GetDataForPhase("phase_one") );

		// create file in Assets/Config/
		#if !UNITY_WEBPLAYER
			File.WriteAllText(Application.dataPath + "/Config/data.json", gameData); 
		#endif
	
	}
	
	private void LoadGameConfig()
	{
		StreamReader reader = new StreamReader(Application.dataPath + "/Config/api.json");
		string strConfigData = reader.ReadToEnd();

		DataManager.SetGameConfig(strConfigData);
	}
}
