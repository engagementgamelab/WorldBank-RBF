using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanvasToggle : MonoBehaviour {

	public NotebookCanvas thisCanvas;
	public List<NotebookCanvas> otherCanvases;
	public bool openAtStart = false;

	/*bool CanCloseNotebook {
		get {
			string currentCity = PlayerData.CityGroup.CurrentCity;
			return (
				(currentCity == DataManager.SceneContext
				&& !NotebookManager.Instance.MakingPlan)
			);
		}
	}*/

	void Start () {
		if (openAtStart) {
			SetCanvasActive (true);
		}
	}

	public void Close () {
		SetCanvasActive (false);
	}

	public void OnClick () {
		// TODO: handle this better
		if (!NPCFocusBehavior.Instance.Unfocused) return;// || !CanCloseNotebook) return;
		foreach (NotebookCanvas canvas in otherCanvases) {
			canvas.gameObject.SetActive (false);
		}
		SetCanvasActive (!thisCanvas.gameObject.activeSelf);
	}

	void SetCanvasActive (bool active) {
		thisCanvas.gameObject.SetActive (active);
		NotebookManagerPhaseOne.Instance.IsOpen = active;	
	}
}
