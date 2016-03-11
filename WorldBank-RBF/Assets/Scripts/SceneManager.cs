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
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneManager : MonoBehaviour {

	public string sceneName;
	public bool inMenus;

	public MenusManager menus;

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

		if(infoBox != null)
			infoBox.onButtonClicked += SkipLogin;

		// We need our game config data before calling any remote endpoints
		LoadGameConfig();
			
		// Set global game data local fallback in case of no connection
		SetGameData(true);
	}

	void Start() {

		#if !UNITY_WEBGL
			Application.targetFrameRate = 60;
		#endif

		// Set loading indicator styles
		#if UNITY_IPHONE
        Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.WhiteLarge);
    #elif UNITY_ANDROID
        Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Large);
    #endif

		// Authenticate to API
    if(!NetworkManager.Instance.Authenticated) {
    	Debug.Log("Authenticate to API");
			NetworkManager.Instance.Authenticate(ClientAuthenticated);
    }
	
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
		// If failed (bad auth or local)
		if(response.ContainsKey("local")) {
		
			Debug.Log("Unable to contact API endpoint.");
			return;
		
		}
		else if(!System.Convert.ToBoolean(response["authed"])) {

			Debug.Log(">>> API authentication failed!");
			return;	
		
		}

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

		// Set as authenticated
		NetworkManager.Instance.Authenticated = true;

		DataManager.SceneContext = sceneName;
			
		// Set global game data if needed
		SetGameData();

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

		Debug.Log("Show on server down");

		infoBox.buttonText.text = "Ok";
		infoBox.Open(DataManager.GetUIText("copy_connection_lost_header"), DataManager.GetUIText("copy_connection_lost_body"));

	}

	void SkipLogin() {

		Events.instance.Raise(new PlayerLoginEvent(true));
		// menus.SetScreen ("phase");

	}

	/// <summary>
	/// Obtains game config data and passes it to global data manager
	/// </summary>
	void LoadGameConfig()
	{

		Debug.Log("Loading game config");

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
	void SetGameData(bool fallback=false) {

		string gameData = null;

		if(fallback)
			gameData = GameDataLoadFallback();
		
		else
		{
			// This should live in a static global dictionary somewhere
			// Try to get data from API remote
			try {

	      #if UNITY_WEBGL

		      // Get gamedata for webgl
		      NetworkManager.Instance.GetURL("/gameData", GameDataResponse);

	      #else

					gameData = NetworkManager.Instance.DownloadDataFromURL("/gameData");

	      #endif

			}
			// Fallback: load game data from local config
			catch(System.Exception e) {
						
				gameData = GameDataLoadFallback();

			}
		}

		GameDataResponse(gameData);

	}

	string GameDataLoadFallback() {

		string localData;

	  TextAsset dataJson = (TextAsset)Resources.Load("data", typeof(TextAsset));
		StringReader strData = new StringReader(dataJson.text);
	      
		localData = strData.ReadToEnd();

		strData.Close();

		return localData;

	}

	void GameDataResponse(string data) {

		// Set global game data
		if(data != null && data.Length > 0)	
			DataManager.SetGameData(data);

	}
	
	// experimental: re-routes all logging
  void HandleLog(string logString, string stackTrace, LogType type) {
      Debug.Log(logString);
      Debug.Log(stackTrace);
  }
}
