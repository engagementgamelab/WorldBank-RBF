using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PhaseSelectionScreen : MonoBehaviour {

	public MenusManager menus;
	public Button backButton;

	void OnEnable() {

		backButton.gameObject.SetActive(!NetworkManager.Instance.Offline);

	}

	public void OnPhaseOne () {
		AudioManager.Music.FadeOut ("title_theme", 0.5f, () => {
				MenusManager.gotoSceneOnLoad = "PhaseOne";
				AudioManager.StopAll ();
				StartCoroutine (CoGotoLoad ("loading"));
			}
		);
	}

	public void OnPhaseTwo () {
		menus.SetScreen ("plan");
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
