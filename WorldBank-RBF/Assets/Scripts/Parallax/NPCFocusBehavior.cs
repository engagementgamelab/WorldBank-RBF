using UnityEngine;
using System.Collections;

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
	float speed = 2.5f;
	ParallaxNpc npc;

	FocusLevel focusLevel = FocusLevel.Default;
	public FocusLevel FocusLevel {
		get { return focusLevel; }
		set { 
			focusLevel = value; 
			targetFocus = (float)focusLevel / 100f;
			StartCoroutine (CoFocus ());
		}
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

	float startCamPosition = 0f;
	float endCamPosition = 0f;
	float npcDialogSeparation = 0.45f; // % of screen
	bool focusing = false;

	public void OnClickNpc (ParallaxNpc npc) {
		this.npc = npc;
		ParallaxLayerLightController.Instance.AssignLayers (npc.gameObject);
		FocusCamera ();		
		IterateFocusLevel ();
	}

	void FocusCamera () {
		
		CameraPositioner.DragEnabled = false;
		startCamPosition = CameraPositioner.Position.x;
		
		if (npc == null) {
			endCamPosition = startCamPosition;
			return;
		}

		float npcPosition = npc.GetPositionAtScale (1f);
		float viewportOffset = 0.5f + npcDialogSeparation * 0.5f;
		float offset = CameraPositioner.Position.x - ScreenPositionHandler.ViewportToWorld (
			new Vector3 (viewportOffset, 0, npc.Position.z)).x;
		endCamPosition = (npc.FacingLeft)
			? npcPosition + offset
			: npcPosition - offset;
	}

	void IterateFocusLevel () {
		if (FocusLevel == FocusLevel.Default) {
			FocusLevel = FocusLevel.Preview;
		} else if (FocusLevel == FocusLevel.Preview) {
			FocusLevel = FocusLevel.Dialog;
		} else if (FocusLevel == FocusLevel.Dialog) {
			FocusLevel = FocusLevel.Default;
		}
	}

	void OnEndFocus () {
		if (FocusLevel == FocusLevel.Default) {
			CameraPositioner.DragEnabled = true;
			ParallaxLayerLightController.Instance.AssignLayers ();
		}
	}

	IEnumerator CoFocus () {

		if (focusing) yield break;
		focusing = true;

		while (!Mathf.Approximately (focus, targetFocus)) {
			focus = (Mathf.Abs (focus - targetFocus) < 0.005f)
				? targetFocus
				: Mathf.Lerp (focus, targetFocus, speed * Time.deltaTime);
			ParallaxLayerLightController.Instance.Lighting1Intensity = Mathf.Abs (focus-1);
			npc.Scale = focus;
			MainCamera.Zoom = focus * Mathf.Pow (1.25f, 3f);

			// TODO: make this more generic
			float progress = focus;
			if (targetFocus >= 0.5f) {
				progress = focus * 2f;
			}
			if (targetFocus >= 1f) {
				progress = 1f;
			}
			if (targetFocus <= 0f) {
				progress = focus;
			}

			CameraPositioner.Transform.SetLocalPositionX (
				Mathf.Lerp (startCamPosition, endCamPosition, progress));
			yield return null;
		}

		focusing = false;
		OnEndFocus ();
	}
}
