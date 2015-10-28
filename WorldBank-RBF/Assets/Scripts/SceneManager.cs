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
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneManager : MonoBehaviour {

	public string sceneName;
	public bool inMenus;

	[HideInInspector]
	public int environmentIndex = 0;
	[HideInInspector]
	public string environment;

	[HideInInspector]
	public bool tutorialEnabled;

	public InfoBox infoBox;

	public delegate void AuthCallbackDelegate();

	private PlayerLoginRegisterUI loginUI;

	void Awake () { 

		NetworkManager.Instance.onServerDown += OnServerDown;

		// We need our game config data before calling any remote endpoints
		LoadGameConfig();

		// Authenticate to API
		// if(inMenus) {
			NetworkManager.Instance.Authenticate(ClientAuthenticated);
			
			// Set global game data if needed
			SetGameData();
		// }
	}

	void Start() {

		Application.targetFrameRate = 60;
		
	}

  #if UNITY_EDITOR
	void OnGUI() {
		GUIStyle style = new GUIStyle();
		
		style.fontSize = 13;
		style.fontStyle = FontStyle.BoldAndItalic;

	    GUI.contentColor = Color.white;

        GUI.Label(new Rect(4, 4, 100, 20), "ENVIRONMENT: " + environment, style);
   }
  #endif

	/// <summary>
	/// Client was authenticated to API; we can now get game data and ask player to log in
	/// </summary>
    /// <param name="response">Dictionary containing "authed" key telling us if API auth </param>
    public void ClientAuthenticated(Dictionary<string, object> response) {

		Debug.Log("Client API auth successful? " + response["authed"]);

		if(!System.Convert.ToBoolean(response["authed"]))
			return;

		NetworkManager.Instance.Cookie = response["session_cookie"].ToString();

		// Authenticate player -- user/pass is hard-coded if in editor
		if(!PlayerManager.Instance.Authenticated)
		{

			#if UNITY_EDITOR
				if (EditorApplication.currentScene != "Assets/Scenes/Menus.unity"
					&& EditorApplication.currentScene != "Assets/Scenes/ScreenSizeSetup.unity") {
					PlayerManager.Instance.Authenticate("tester@elab.emerson.edu", "password");
				}
			#endif
			
		}

		DataManager.SceneContext = sceneName;

    }
	
	/// <summary>
	/// User attempted authentication; return/show error if failed
	/// </summary>
    /// <param name="success">Was authentication successful?.</param>
	public void UserAuthenticateResponse(bool success) {

		if(!success)
			return;

		Debug.Log("Player auth successful? " + success);

	}

	void OnServerDown() {

		infoBox.Open("Sorry!", 
								 "The game's server is currently unreachable. Your internet connection may be having some issues, or the server is offline for regular maintenance.\n\nPlease close the application and try again in a few minutes. Apologies for the inconvenience!");

	}

	/// <summary>
	/// Obtains game config data and passes it to global data manager
	/// </summary>
	void LoadGameConfig()
	{
		// Open stream to API JSON config file
		TextAsset apiJson = (TextAsset)Resources.Load("api", typeof(TextAsset));
		StringReader strConfigData = new StringReader(apiJson.text);

		// Set in data manager class with chosen environment config
		DataManager.SetGameConfig(strConfigData.ReadToEnd(), environment);


	  #if UNITY_EDITOR
			DataManager.tutorialEnabled = tutorialEnabled;
		#else
			DataManager.tutorialEnabled = true;
		#endif

		strConfigData.Close();
	}

	/// <summary>
	/// Obtains and sets global game data
	/// </summary>
	void SetGameData() {

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
			#else
 
		    TextAsset dataJson = (TextAsset)Resources.Load("data", typeof(TextAsset));
				StringReader strData = new StringReader(dataJson.text);
		        
				gameData = strData.ReadToEnd();

				strData.Close();
				
			#endif
		
		}

		// Set global game data
		if(gameData != null && gameData.Length > 0)	
			DataManager.SetGameData(gameData);

	}
	
}
