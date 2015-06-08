using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapManager2 : MB {

	Canvas canvas = null;
	Canvas Canvas {
		get {
			if (canvas == null) {
				canvas = GetComponent<Canvas> ();
			}
			return canvas;
		}
	}

	void Open () {
		Canvas.enabled = true;
	}

	void Close () {
		Canvas.enabled = false;
	}

	/*void OnGUI () {
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Open")) {
			Open ();
		}
		if (GUILayout.Button ("Close")) {
			Close ();
		}
		GUILayout.EndHorizontal ();
	}*/
}
