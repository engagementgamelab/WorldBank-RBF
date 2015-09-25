using UnityEngine;
using System.Collections;

public class PhaseSelectionScreen : MonoBehaviour {

	public MenusManager menus;

	public void OnPhaseOne () {
		AudioManager.Music.FadeOut ("title_theme", 0.5f, () => {
				MenusManager.gotoSceneOnLoad = "PhaseOne";
				AudioManager.Music.Stop ("title_theme");
				StartCoroutine (CoGotoLoad ());
			}
		);
	}

	public void OnPhaseTwo () {
		AudioManager.Music.FadeOut ("title_theme", 0.5f, () => {
				MenusManager.gotoSceneOnLoad = "PhaseTwo";
				AudioManager.Music.Stop ("title_theme");
				StartCoroutine (CoGotoLoad ());
			}
		);
	}

	public void OnBack () {
		menus.SetScreen ("title");
	}

	IEnumerator CoGotoLoad () {
		yield return new WaitForFixedUpdate ();
		ObjectPool.Clear ();
		menus.SetScreen ("loading");
	}
}
