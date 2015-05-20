using UnityEngine;
using System.Collections;

public enum FocusLevel {
	Null,
	Default = 0,
	Preview = 25,
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

	FocusLevel focusLevel = FocusLevel.Default;
	public FocusLevel FocusLevel {
		get { return focusLevel; }
	}
	
	bool focused = false;
	bool focusing = false;
	
	// TODO: Make this work with ParallaxNpc instead

	/*LayerImage npc;
	float zoomBeforeFocus;
	float focusPercentage = 0f;

	public void SetFocus (LayerImage npc, FocusLevel level=FocusLevel.Null, bool openNext=true) {

		if (!InitFocusIn (npc)) return;
		if (level == FocusLevel.Null) {
			Debug.Log ("level: " + level + "; focusLevel: " + focusLevel);
			if (focusLevel == FocusLevel.Default) {
				zoomBeforeFocus = MainCamera.Instance.Zoom;
			} else if (focusLevel == FocusLevel.Preview)  {
				focusLevel = FocusLevel.Dialog;
			} else if (focusLevel == FocusLevel.Dialog) {
				focusLevel = FocusLevel.Default;
			}
		} else {
			focusLevel = level;
		}
		// StartCoroutine (CoFocusIn ((float)focusLevel / 100f));
		if(!openNext)
			StartCoroutine (CoFocusOut ());
		else if (focusLevel == FocusLevel.Default || focusLevel == FocusLevel.Preview) {
			StartCoroutine (CoFocusOut ());
		} else {
			StartCoroutine (CoFocusIn ((float)focusLevel / 100f));
		}
	}

	bool InitFocusIn (LayerImage npc) {
		if (focusing) return false;
		this.npc = npc;
		focusing = true;
		
		return true;
	}

	public void FocusOut () {
		if (InitFocusOut ()) {
			StartCoroutine (CoFocusOut ());
		}
	}

	bool InitFocusOut () {
		if (focusing) return false;
		focusing = true;
		return true;
	}

	void FinishFocusIn (float percentage, bool openNext=true) {
		if (Mathf.Approximately (percentage, 1f) && npc.Behavior != null && openNext) {
			npc.Behavior.OpenDialog ();
		}
		focusing = false;
	}

	void FinishFocusOut () {
		MainCamera.Instance.ZoomVelocity = 3f;
		MainCamera.Instance.Positioner.DragEnabled = true;
		MainCamera.Instance.LineOfSight.ZoomEnabled = true;
		focusing = false;
	}

	IEnumerator CoFocusIn (float percentage, bool openNext=true) {
		
		float duration = 1.25f;
		float eTime = 0f;

		float center = npc.Position.x - (npc.XOffset * npc.Transform.lossyScale.x);
		float middle = npc.Position.y - (npc.ColliderCenterY * npc.Transform.lossyScale.x);
		float offset = (npc.ColliderWidth + NPCDialogBox.width) / 2f;
		float xPosition = npc.FacingLeft ? center - offset : center + offset;
		Vector3 highlightPosition = new Vector3 (center, middle, npc.Position.z);

		MainCamera.Instance.Positioner.DragEnabled = false;
		MainCamera.Instance.LineOfSight.ZoomEnabled = false;

		FocusAction[] focusActions = new FocusAction[] {
			new FocusAction (1f, 0.1f, 0.9f,  (float t) => npc.ScaleToPercentage (percentage, t)),
			new FocusAction (1f, 0f,   0.75f, (float t) => MainCamera.Instance.MoveToTarget (xPosition, t)),
			new FocusAction (1f, 0.1f, 0.9f,  (float t) => MainCamera.Instance.ZoomToPercentage (percentage, 2.5f)),
			new FocusAction (1f, 0f,   0.75f, (float t) => DirectionalLightController.Instance.FadeToPercentage (percentage, t)),
			new FocusAction (1f, 0.5f, 1f,    (float t) => NPCHighlight.Instance.Activate (highlightPosition, percentage, t))
		};
	
		while (eTime < duration) {
			eTime += Time.deltaTime;
			float p = eTime / duration;
			for (int i = 0; i < focusActions.Length; i ++) {
				focusActions[i].SetProgress (p);
			}
			yield return null;
		}

		FinishFocusIn (percentage, openNext);
	}

	IEnumerator CoFocusOut () {
		
		float duration = 1.25f;
		float eTime = 0f;

		FocusAction[] focusActions = new FocusAction[] {
			new FocusAction (1f, 0.1f, 0.9f,  (float t) => npc.ScaleToPercentage (0f, t)),
			new FocusAction (1f, 0.1f, 0.9f,  (float t) => MainCamera.Instance.ZoomTo (zoomBeforeFocus, 2.5f)),
			new FocusAction (1f, 0f,   0.75f, (float t) => DirectionalLightController.Instance.FadeToPercentage (0f, t)),
			new FocusAction (1f, 0f,   0.5f,  (float t) => NPCHighlight.Instance.Deactivate (t))
		};

		while (eTime < duration) {
			eTime += Time.deltaTime;
			float p = eTime / duration;
			for (int i = 0; i < focusActions.Length; i ++) {
				focusActions[i].SetProgress (p);
			}
			yield return null;
		}

		MainCamera.Instance.Positioner.DragEnabled = true;
		MainCamera.Instance.LineOfSight.ZoomEnabled = true;

		FinishFocusOut ();
	}*/

	class FocusAction {

		public bool triggered = false;
		public readonly float totalDuration;
		public readonly float triggerAt;
		public readonly float duration; // percentage of total duration
		public readonly System.Action<float> action;

		public FocusAction (float totalDuration, float triggerAt, float duration, System.Action<float> action) {
			triggered = false;
			this.totalDuration = totalDuration;
			this.triggerAt = triggerAt;
			this.duration = duration;
			this.action = action;
		}

		public void SetProgress (float progress) {
			if (progress >= triggerAt && !triggered) {
				action (totalDuration * duration);
				triggered = true;
			}
		}
	}
}
