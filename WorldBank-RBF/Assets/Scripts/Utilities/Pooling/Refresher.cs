using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void OnRefresh ();

[ExecuteInEditMode]
public class Refresher : MonoBehaviour {

	public static OnRefresh OnRefresh { get; set; }
	public bool refresh = false;
	bool prevRefresh = false;

	static Refresher instanceInternal = null;
	static public Refresher Instance {
		get {
			if (instanceInternal == null) {
				instanceInternal = Object.FindObjectOfType (typeof (Refresher)) as Refresher;
				if (instanceInternal == null) {
					GameObject go = new GameObject ("Refresher");
					DontDestroyOnLoad (go);
					instanceInternal = go.AddComponent<Refresher>();
				}
			}
			return instanceInternal;
		}
	}

#if UNITY_EDITOR
	void Update () {
		if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {
			this.enabled = false;
		} else {
			if (refresh != prevRefresh) {
				if (OnRefresh != null) 
					OnRefresh ();
				refresh = prevRefresh;
			}
		}
	}
#endif

	public void Refresh () {
		refresh = !refresh;
	}
}
