using UnityEngine;
using System.Collections;
using System.IO;
using SimpleJSON;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {

		// This should live in a static global dictionary somewhere
		string gameData = NetworkManager.Instance.DownloadDataFromURL("http://localhost:3000/api/gameData");
		var node = JSON.Parse(gameData)["content"][0]["phase_one"];

		Debug.Log(node[0]["city"]);

		// create file in Assets/Config/
		#if !UNITY_WEBPLAYER
			File.WriteAllText(Application.dataPath + "/Config/data.json", gameData); 
		#endif
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
