using UnityEngine;
using System.Collections;

public class LoadChecker : MonoBehaviour {

	static LoadChecker instance = null;
	static public LoadChecker Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (LoadChecker)) as LoadChecker;
				if (instance == null) {
					GameObject go = new GameObject ("LoadChecker");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<LoadChecker> ();
				}
			}
			return instance;
		}
	}

	bool loaded = false;
	public bool Loaded { 
		get { return loaded; }
		set { loaded = value; }
	}
}