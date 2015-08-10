using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CanvasToggle : MonoBehaviour {

	Image image = null;
	Image Image {
		get {
			if (image == null) {
				image = GetComponent<Image> ();
			}
			return image;
		}
	}

	Button button = null;
	Button Button {
		get {
			if (button == null) {
				button = GetComponent<Button> ();
			}
			return button;
		}
	}

	public NotebookCanvas thisCanvas;
	public List<CanvasToggle> otherToggles;
	public bool openAtStart = false;

	static NotebookCanvas activeCanvas = null;

	void Start () {
		NPCFocusBehavior.Instance.onSetFocus += OnSetFocus;
		if (openAtStart) {
			SetCanvasActive (true);
		}
	}

	public void Close () {
		SetCanvasActive (false);
	}

	public void OnClick () {
		if (activeCanvas == thisCanvas && !NotebookManagerPhaseOne.Instance.CanCloseNotebook) return;
		foreach (CanvasToggle toggle in otherToggles) {
			toggle.SetCanvasActive (false);
		}
		SetCanvasActive (!thisCanvas.gameObject.activeSelf);
	}

	public void SetCanvasActive (bool active) {
		thisCanvas.gameObject.SetActive (active);
		NotebookManagerPhaseOne.Instance.IsOpen = active;
		activeCanvas = active ? thisCanvas : null;
	}

	void SetButtonActive (bool active) {
		Image.enabled = active;
		Button.enabled = active;
	}

	void OnSetFocus (FocusLevel focusLevel) {
		SetButtonActive (focusLevel == FocusLevel.Default);
	}
}
