using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotebookManagerPhaseOne : MonoBehaviour {

	static NotebookManagerPhaseOne instance = null;
	static public NotebookManagerPhaseOne Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (NotebookManagerPhaseOne)) as NotebookManagerPhaseOne;
			}
			return instance;
		}
	}

	public bool IsOpen { get; set; }

	public List<CanvasToggle> toggles;

	public void CloseCanvases () {
		foreach (CanvasToggle toggle in toggles) {
			toggle.Close ();
		}
	}
}
