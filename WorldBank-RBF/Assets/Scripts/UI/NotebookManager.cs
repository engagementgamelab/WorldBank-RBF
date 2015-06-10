using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotebookManager : MB {

	public GameObject map;
	public GameObject priorities;
	public GameObject data;
	public GameObject tabGroup;

	Dictionary<string, GameObject> canvases;
	Dictionary<string, GameObject> Canvases {
		get {
			if (canvases == null) {
				canvases = new Dictionary<string, GameObject> ();
				canvases.Add ("map", map);
				canvases.Add ("priorities", priorities);
				canvases.Add ("data", data);
			}
			return canvases;
		}
	}

	bool open = false;
	string activeCanvas = "map";

	void Awake () {
		Close ();
	}

	public void OpenMap () {
		OpenCanvas ("map");
	}

	public void OpenPriorities () {
		OpenCanvas ("priorities");
	}

	public void OpenData () {
		OpenCanvas ("data");
	}

	public void ToggleNotebook () {
		if (open) {
			open = false;
			Close ();
		} else {
			open = true;
			Open ();
		}
	}

	void OpenCanvas (string id) {
		foreach (var canvas in Canvases) {
			canvas.Value.SetActive (canvas.Key == id);
		}
		activeCanvas = id;
	}

	void Open () {
		OpenCanvas (activeCanvas);
		tabGroup.SetActive (true);
	}

	void Close () {
		foreach (var canvas in Canvases) {
			canvas.Value.SetActive (false);
		}
		tabGroup.SetActive (false);
	}
}
