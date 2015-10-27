using UnityEngine;
using System.Collections;

public delegate void OnSetFocus (FocusLevel focusLevel);

public enum FocusLevel {
	Null,
	Default = 0,
	Preview = 50,
	Dialog = 100
}

public class NPCFocusBehavior : MonoBehaviour {

	static NPCFocusBehavior instance = null;
	static public NPCFocusBehavior Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (NPCFocusBehavior)) as NPCFocusBehavior;
				if (instance == null) {
					GameObject go = new GameObject ("NPCFocusBehavior");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<NPCFocusBehavior>();
				}
			}
			return instance;
		}
	}

	float focus = 0f;
	float targetFocus = 0f;
	const float speed = 1.25f;
	ParallaxNpc npc = null;

	FocusLevel focusLevel = FocusLevel.Default;
	public FocusLevel FocusLevel {
		get { return focusLevel; }
		set { 
			PlayVoice ();
			focusLevel = value;
			if (onSetFocus != null) onSetFocus (value);
			targetFocus = (float)focusLevel / 100f;
			StartCoroutine (CoFocus ());
		}
	}

	public bool Unfocused {
		get { return FocusLevel == FocusLevel.Default && !focusing; }
	}

	MainCamera mainCamera = null;
	MainCamera MainCamera {
		get {
			if (mainCamera == null) {
				mainCamera = MainCamera.Instance;
			}
			return mainCamera;
		}
	}

	CameraPositioner cameraPositioner = null;
	CameraPositioner CameraPositioner {
		get { 
			if (cameraPositioner == null) {
				cameraPositioner = MainCamera.Positioner; 
			}
			return cameraPositioner;
		}
	}

	public OnSetFocus onSetFocus;

	float startCamPosition = 0f;
	float endCamPosition = 0f;
	const float npcDialogSeparation = 0.45f; // % of screen
	bool focusing = false;

	public void DefaultFocus (int choicesCount=0) {
		
		Focus ();
		FocusLevel = FocusLevel.Default;

	}

	public void PreviewFocus (ParallaxNpc npc) {
		if (NotebookManagerPhaseOne.Instance.IsOpen 
			|| focusing 
			|| FocusLevel != FocusLevel.Default) return;
		if (!CameraPositioner.Drag.Dragging) {
			this.npc = npc;
			Focus ();
			FocusLevel = FocusLevel.Preview;
		}
	}

	public void OnCloseDialog () {
		if (FocusLevel != FocusLevel.Default) {
			Focus ();
			FocusLevel = FocusLevel.Default;
		}
	}

	public void DialogFocus () {
		if (npc == null)
			throw new System.Exception ("An NPC has not been selected");
		Focus ();
		FocusLevel = FocusLevel.Dialog;
	}

	void PlayVoice () {
		string quality;
		if (FocusLevel == FocusLevel.Default) {
			quality = "greeting";
		} else if (FocusLevel == FocusLevel.Preview) {
			quality = "response";
		} else {
			quality = "farewell";
		}
		AudioManager.Sfx.Play (npc.voice + quality, "NPCs");
	}

	void Focus () {
		ParallaxLayerLightController.Instance.AssignLayers (npc.gameObject);
		FocusCamera ();
	}

	void FocusCamera () {
		
		CameraPositioner.Drag.Enabled = (FocusLevel == FocusLevel.Dialog);
		startCamPosition = CameraPositioner.Position.x;
		
		if (npc == null) {
			endCamPosition = startCamPosition;
			return;
		}

		float npcPosition = npc.GetPositionAtScale (1f);
		float viewportOffset = 0.5f + npcDialogSeparation * 0.5f;
		float offset = ScreenPositionHandler.ViewportToWorldRelative (
			new Vector3 (viewportOffset, 0, npc.Position.z)).x;

		endCamPosition = (npc.FacingLeft)
			? npcPosition - offset
			: npcPosition + offset;
	}

	void OnEndFocus () {
		if (FocusLevel == FocusLevel.Default) {
			ParallaxLayerLightController.Instance.AssignLayers ();
			npc = null;
			CameraPositioner.Drag.Enabled = true;
		} else if (FocusLevel == FocusLevel.Preview) {
			DialogManager.instance.OpenNpcDescription (NpcManager.GetNpc (npc.symbol), npc.FacingLeft);
		} else if (FocusLevel == FocusLevel.Dialog) {
			DialogManager.instance.OpenNpcDialog (NpcManager.GetNpc (npc.symbol), npc.voice, npc.FacingLeft);
		}
	}

	IEnumerator CoFocus () {

		if (focusing) yield break;
		focusing = true;

		float startFocus = focus;
		float endFocus = targetFocus;
		float time = Mathf.Abs (startFocus-endFocus) * speed;
		float eTime = 0f;

		while (eTime < time) {
			eTime += Time.deltaTime;
			focus = Mathf.Lerp (startFocus, endFocus, Mathf.SmoothStep (0f, 1f, eTime / time));
			ParallaxLayerLightController.Instance.Lighting1Intensity = Mathf.Abs (focus-1f);
			npc.Scale = focus;
			MainCamera.Zoom = focus * Mathf.Pow (1.25f, 3f);

			// TODO: Clean up	
			float progress = focus;
			if (targetFocus >= 0.5f) {
				progress = focus * 2f;
			}
			if (targetFocus >= 1f) {
				progress = 1f;
			}
			if (targetFocus > 0f) {
				CameraPositioner.Transform.SetLocalPositionX (
					Mathf.Lerp (startCamPosition, endCamPosition, progress));
			}

			yield return null;
		}

		focusing = false;
		OnEndFocus ();
	}
}

