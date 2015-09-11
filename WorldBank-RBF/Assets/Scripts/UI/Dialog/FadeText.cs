using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeText : MB {

	Text text = null;
	public Text Text {
		get {
			if (text == null) {
				text = GetComponent<Text> ();
			}
			return text;
		}
	}

	float alpha = -1;
	public float Alpha {
		set {
			InitAlpha ();
			alpha = value;
			color.a = value;
			Text.color = color;
		}
	}

	Color color;

	void InitAlpha () {
		if (Mathf.Approximately (alpha, -1))
			color = Text.color;
	}

	public void FadeIn (float time) {
		StartCoroutine (CoFade (0f, 1f, time));
	}

	public void FadeOut (float time) {
		StartCoroutine (CoFade (1f, 0f, time));
	}

	IEnumerator CoFade (float from, float to, float time) {
		
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (from, to, eTime / time);
			Alpha = progress;			
			yield return null;
		}
	}
}
