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
using Parse;

public class SceneManager : MonoBehaviour {

	static SceneManager instance = null;
	static public SceneManager Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (SceneManager)) as SceneManager;
				if (instance == null) {
					GameObject go = new GameObject ("SceneManager");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<SceneManager>();
				}
			}
			return instance;
		}
	}

	public string sceneName;
	public bool phaseOne;

	public delegate void AuthCallbackDelegate();

	private PlayerLoginRegisterUI loginUI;

	void Awake () {

		// We need our game config data before calling any remote endpoints
		LoadGameConfig();
	
		// Set global game data if needed
		SetGameData();

		DataManager.SceneContext = sceneName;

		// Authenticate player -- user/pass is hard-coded for now
		if(!PlayerManager.Instance.Authenticated)
		{

			#if UNITY_EDITOR
				PlayerManager.Instance.Authenticate("tester@elab.emerson.edu", "password");
			#else
				loginUI = ObjectPool.Instantiate<PlayerLoginRegisterUI>();
				loginUI.Callback = UserAuthenticated;
			#endif
			
		}

	}
	
	public void UserAuthenticated(bool success) {

		if(!success)
			return;

		if(phaseOne)
			NotebookManager.Instance.OpenMap();

		Debug.Log("Player auth successful? " + success);

	}

	/// <summary>
	/// Obtains game config data and passes it to global data manager
	/// </summary>
	private void LoadGameConfig()
	{
		// Open stream to API JSON config file
		TextAsset apiJson = (TextAsset)Resources.Load("api", typeof(TextAsset));
		StringReader strConfigData = new StringReader(apiJson.text);

		// Set in data manager class
		DataManager.SetGameConfig(strConfigData.ReadToEnd());

		strConfigData.Close();
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
		catch(System.Exception e) {

			// If in editor, always throw so we catch issues
			#if UNITY_EDITOR
				throw new System.Exception("Unable to obtain game data due to error '" + e + "'");
			#endif
 
	        TextAsset dataJson = (TextAsset)Resources.Load("data", typeof(TextAsset));
			StringReader strData = new StringReader(dataJson.text);
	        
			gameData = strData.ReadToEnd();

			strData.Close();
		
		}

		// Set global game data
		if(gameData != null && gameData.Length > 0)	
			DataManager.SetGameData(gameData);

	}
}
