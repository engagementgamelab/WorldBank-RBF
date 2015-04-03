using UnityEngine;
using System.Collections;
using System.IO;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {

		// This should live in a static global dictionary somewhere
		string gameData = NetworkManager.Instance.DownloadDataFromURL("http://localhost:3000/api/gameData");

		// create file in Assets/Config/
		File.WriteAllText(Application.dataPath + "/Config/data.json", gameData); 
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
