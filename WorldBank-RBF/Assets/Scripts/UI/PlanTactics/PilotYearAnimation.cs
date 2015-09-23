using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PilotYearAnimation : MonoBehaviour {

	public Image background;
	public Text text;

	public void Animate (float animationTime) {
		text.text = DataManager.GetUIText ("copy_pilot_year");
		StartCoroutine (CoAnimate (animationTime));
	}

	IEnumerator CoAnimate (float time) {
		
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / time);
			background.fillAmount = progress;
			yield return null;
		}
	}
}
