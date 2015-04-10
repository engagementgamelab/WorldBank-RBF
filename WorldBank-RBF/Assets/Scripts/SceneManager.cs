using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SceneManager : MonoBehaviour {

	public string currentCity = "city";

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

		// Set global game data
		DataManager.SetGameData(gameData);

		// Data tests
		Dictionary<string, IEnumerator> itr = DataManager.GetDataForCity(currentCity);

        foreach(IEnumerator npcEnum in itr.Values) {
        	while(npcEnum.MoveNext()) {

        		Debug.Log(DataManager.GetKVP(npcEnum.Current).Value);
        		Debug.Log(DataManager.GetKVP(npcEnum.Current).Value.GetType());

        	}
        }
        
        // Debug.Log(itr["dialogue"].GetType());

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
