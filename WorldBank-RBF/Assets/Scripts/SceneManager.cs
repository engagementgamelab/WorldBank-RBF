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

		NetworkManager.Instance.Authenticate(ClientAuthenticated);
	
		// Set global game data if needed
		// TODO: This needs to not be in awake method
		SetGameData();

		DataManager.SceneContext = sceneName;

      
	}

	/// <summary>
	/// Client was authenticated to API; we can now get game data and ask player to log in
	/// </summary>
    /// <param name="response">Dictionary containing "authed" key telling us if API auth </param>
    public void ClientAuthenticated(Dictionary<string, object> response) {

		Debug.Log("Client API auth successful? " + response["authed"]);

		if(!System.Convert.ToBoolean(response["authed"]))
			return;
	
		// Set global game data if needed
		// SetGameData();

		NetworkManager.Instance.Cookie = response["session_cookie"].ToString();

		// Authenticate player -- user/pass is hard-coded if in editor
		if(!PlayerManager.Instance.Authenticated)
		{

			#if UNITY_EDITOR
				PlayerManager.Instance.Authenticate("tester@elab.emerson.edu", "password");
			#else
				loginUI = ObjectPool.Instantiate<PlayerLoginRegisterUI>();
				loginUI.Callback = UserAuthenticateResponse;
			#endif
			
		}

    }
	
	/// <summary>
	/// User attempted authentication; return/show error if failed
	/// </summary>
    /// <param name="success">Was authentication successful?.</param>
	public void UserAuthenticateResponse(bool success) {

		if(!success)
			return;

		// Open map; this may be something different later
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
