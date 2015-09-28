using UnityEngine;
using System.Collections;

public class PhaseSelectionScreen : MonoBehaviour {

	public MenusManager menus;

	public void OnPhaseOne () {
		AudioManager.Music.FadeOut ("title_theme", 0.5f, () => {
				MenusManager.gotoSceneOnLoad = "PhaseOne";
				AudioManager.StopAll ();
				StartCoroutine (CoGotoLoad ("loading"));
			}
		);
	}

	public void OnPhaseTwo () {
		AudioManager.StopAll ();
		StartCoroutine (CoGotoLoad ("plan"));
	}

	public void OnBack () {
		menus.SetScreen ("title");
	}

	IEnumerator CoGotoLoad (string strScreen) {
		yield return new WaitForFixedUpdate ();
		ObjectPool.Clear ();
		menus.SetScreen (strScreen);
	}
}
