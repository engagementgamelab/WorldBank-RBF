using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CanvasToggle : MonoBehaviour {

	Image image = null;
	Image Image {
		get {
			if (image == null) {
				image = GetComponent<Image> ();
				defaultImage = image.sprite;
			}
			return image;
		}
	}

	Sprite defaultImage;

	Button button = null;
	Button Button {
		get {
			if (button == null) {
				button = GetComponent<Button> ();
			}
			return button;
		}
	}

	public Sprite returnImage;
	public NotebookCanvas thisCanvas;
	public List<CanvasToggle> otherToggles;
	public bool openAtStart = false;
	public string sfxGroup;
	public string openSfx;
	public string closeSfx;

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
		if (activeCanvas == thisCanvas && !NotebookManagerPhaseOne.Instance.CanCloseNotebook) 
			return;
		foreach (CanvasToggle toggle in otherToggles) {
			toggle.SetCanvasActive (false);
		}
		bool open = !thisCanvas.gameObject.activeSelf;
		PlaySfx (open);
		SetCanvasActive (open);
	}

	public void SetCanvasActive (bool active) {
		thisCanvas.gameObject.SetActive (active);
		NotebookManagerPhaseOne.Instance.IsOpen = active;
		activeCanvas = active ? thisCanvas : null;
		Image.sprite = active ? returnImage : defaultImage;
	}

	void SetButtonActive (bool active) {
		Image.enabled = active;
		Button.enabled = active;
	}

	void OnSetFocus (FocusLevel focusLevel) {
		SetButtonActive (focusLevel == FocusLevel.Default);
	}

	void PlaySfx (bool open) {
		AudioManager.Sfx.Play (open ? openSfx : closeSfx, sfxGroup);
	}
}
