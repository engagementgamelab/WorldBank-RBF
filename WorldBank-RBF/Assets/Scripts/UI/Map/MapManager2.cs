using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapManager2 : NotebookCanvas {

	Canvas canvas = null;
	Canvas Canvas {
		get {
			if (canvas == null) {
				canvas = GetComponent<Canvas> ();
			}
			return canvas;
		}
	}

	public override void Open () {
		Canvas.enabled = true;
	}

	public override void Close () {
		Canvas.enabled = false;
	}
}
