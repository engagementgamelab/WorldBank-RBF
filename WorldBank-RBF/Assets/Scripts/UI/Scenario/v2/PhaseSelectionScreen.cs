using UnityEngine;
using System.Collections;

public class PhaseSelectionScreen : MonoBehaviour {

	public MenusManager menus;

	public void OnPhaseOne () {
		menus.SetScreen ("loading");
	}

	public void OnPhaseTwo () {
		menus.SetScreen ("plan");
	}

	public void OnBack () {
		menus.SetScreen ("title");
	}
}
