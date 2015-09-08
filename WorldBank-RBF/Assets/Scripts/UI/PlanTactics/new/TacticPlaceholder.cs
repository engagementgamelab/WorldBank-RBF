using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TacticPlaceholder : MB {

	LayoutElement layout = null;
	LayoutElement Layout {
		get {
			if (layout == null) {
				layout = GetComponent<LayoutElement> ();
			}
			return layout;
		}
	}

	const float maxHeight = 180f;

	void OnEnable () {
		SetHeight (maxHeight);
	}

	public void ShrinkAndDestroy () {
		StartCoroutine (CoShrinkAndDestroy ());
	}

	IEnumerator CoShrinkAndDestroy () {
		
		float time = 0.2f;
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			SetHeight (Mathf.Lerp (maxHeight, 0f, progress));
			yield return null;
		}

		ObjectPool.Destroy<TacticPlaceholder> (Transform);
	}

	void SetHeight (float h) {
		Layout.minHeight = h;
		Layout.preferredHeight = h;
	}
}
