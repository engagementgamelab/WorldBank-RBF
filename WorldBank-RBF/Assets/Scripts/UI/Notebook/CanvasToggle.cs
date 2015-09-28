using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

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

	public void Open () {
		if (activeCanvas != thisCanvas) {
			OnClick ();
		}
	}

	public void Close () {
		SetCanvasActive (false);
	}

	public void OnClick () {
		if (CurrentCityIndicator.Moving) return;
		if (activeCanvas == thisCanvas && !NotebookManagerPhaseOne.Instance.CanCloseNotebook) 
			return;
		foreach (CanvasToggle toggle in otherToggles) {
			toggle.SetCanvasActive (false);
		}
		bool open = !thisCanvas.gameObject.activeSelf;
		PlaySfx (open);
		SetCanvasActive (open);
		
		List<TacticItem> tactics = PlayerData.TacticGroup.Tactics;
		bool inCapitol = ( PlayerData.CityGroup.CurrentCity == "capitol" );
		bool hasTactics = ( tactics.Where(i => i.Unlocked).Count() > 0 );
		
		// Tactics/plan screen tutorial (player is in capitol and has tactics) 
		if(thisCanvas.GetType() == typeof(PrioritizationManager) && inCapitol && hasTactics)
			DialogManager.instance.CreateTutorialScreen(open ? "phase_1_tactics" : "phase_1_continue_talking");
		
		// Map
		else if(thisCanvas.GetType() == typeof(MapManager2)) {

			if(inCapitol && PlayerData.InteractionGroup.Count == 0)
				DialogManager.instance.CreateTutorialScreen("phase_1_map");
			else if(PlayerData.InteractionGroup.Count > 0)
				DialogManager.instance.CreateTutorialScreen("phase_1_conversation_points");

		}

	}

	void SetCanvasActive (bool active) {
		thisCanvas.gameObject.SetActive (active);
		NotebookManagerPhaseOne.Instance.IsOpen = active;
		activeCanvas = active ? thisCanvas : null;

		if (!NotebookManagerPhaseOne.Instance.CanCloseNotebook && active) {
			Image.sprite = defaultImage;
			Button.interactable = false;
		} else {
			Image.sprite = active ? returnImage : defaultImage;
			Button.interactable = true;
		}
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
