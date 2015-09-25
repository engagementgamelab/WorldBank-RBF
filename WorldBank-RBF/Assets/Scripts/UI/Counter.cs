using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Counter : MB {

	public Text text;
	public Text animText;

	string newText = "";
	int prevCount = 0;
	int count = 0;
	float endY = 50f;

	protected virtual float Offset {
		get { return -50f; }
	}

	RectTransform textTransform = null;
	RectTransform TextTransform {
		get {
			if (textTransform == null) {
				textTransform = text.GetComponent<RectTransform> ();
			}
			return textTransform;
		}
	}

	RectTransform animTransform = null;
	RectTransform AnimTransform {
		get {
			if (animTransform == null) {
				animTransform = animText.GetComponent<RectTransform> ();
			}
			return animTransform;
		}
	}

	protected int Count { 
		set { 
			newText = value.ToString ();
			animText.text = "-";
			if (value < count) {
				StartCoroutine (CoAnimate ());
			} else {
				text.text = newText;
			}
			count = value;
			prevCount = count;
		}
	}

	void Start () {
		animText.color = new Color (1f, 1f, 1f, 0f);
		SetUpdateCallback ();
	}

	IEnumerator CoAnimate () {
		yield return StartCoroutine (CoScale (Vector3.one, Vector3.zero, 0.2f));
		StartCoroutine (CoRise (Offset, endY+Offset, 0.5f)); // why??? - jay, 9/24/15
		StartCoroutine (CoFade (1f, 0f, 0.5f));
		text.text = newText;
		yield return StartCoroutine (CoScale (Vector3.zero, Vector3.one, 0.2f));
	}

	IEnumerator CoScale (Vector3 from, Vector3 to, float time) {
		
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			TextTransform.localScale = Vector3.Lerp (from, to, progress);
			yield return null;
		}

		TextTransform.localScale = to;
	}

	IEnumerator CoRise (float from, float to, float time) {
		
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			AnimTransform.SetLocalPositionY (Mathf.Lerp (from, to, progress));
			yield return null;
		}
	}

	IEnumerator CoFade (float from, float to, float time) {

		float eTime = 0f;

		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			animText.color = new Color (1f, 1f, 1f, Mathf.Lerp (from, to, progress));
			yield return null;
		}
	}

	protected virtual void SetUpdateCallback () {}
	protected virtual void OnUpdate () {}
}
