using UnityEngine;
using System.Collections;

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

	bool focused = false;
	bool focusing = false;
	LayerImage npc;
	float zoomBeforeFocus;
	System.Action onFocus;

	public void ToggleFocus (LayerImage npc, System.Action onFocus) {
		if (focusing) return;
		if (focused) {
			FocusOut ();
		} else {
			FocusIn (npc, onFocus);
		}
	}

	void FocusIn (LayerImage npc, System.Action onFocus) {
		this.npc = npc;
		this.onFocus = onFocus;
		focused = true;
		focusing = true;
		zoomBeforeFocus = MainCamera.Instance.Zoom;
		StartCoroutine (CoFocusIn ());

		float duration = 1f;
		Invoke ("FinishFocusIn", duration);
		npc.Expand (duration);
		DirectionalLightController.Instance.FadeOut (duration);
		MainCamera.Instance.MoveToTarget (npc.XPosition, duration);
		MainCamera.Instance.ZoomTo (12, 2.5f);
	}

	void FocusOut () {
		focused = false;

		float duration = 1f;
		Invoke ("FinishFocusOut", duration);	
		npc.Shrink (duration);
		MainCamera.Instance.ZoomTo (zoomBeforeFocus);
		DirectionalLightController.Instance.FadeIn (duration);
	}

	void FinishFocusIn () {
		onFocus ();
		focusing = false;
	}

	void FinishFocusOut () {
		MainCamera.Instance.ZoomVelocity = 3f;
	}

	// TODO: queue actions for focus
	IEnumerator CoFocusIn () {
		
		float duration = 1f;
		float eTime = 0f;
		/*bool npcExpanded = false;
		bool lightFaded = false;
		bool cameraMoved = false;
		bool cameraZoomed = false;

		float npcExpandAt = 0.25f;
		float lightFadeAt = 0f;
		float cameraMoveAt = 0f;
		float cameraZoomAt = 0.25f;*/

		FocusAction[] focusActions = new FocusAction[] {
			new FocusAction (0f, duration * 0.75f),
			new FocusAction (0f, duration * 0.75f)
		};
	
		while (eTime < duration) {
			eTime += Time.deltaTime;
			float p = eTime / duration;
			yield return null;
		}
	}

	class FocusAction {

		public bool Triggered { get; set; }
		public readonly float triggerAt;
		public readonly float duration;

		public FocusAction (float triggerAt, float duration) {
			Triggered = false;
			this.triggerAt = triggerAt;
			this.duration = duration;
		}
	}
}
