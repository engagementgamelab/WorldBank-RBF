using UnityEngine;
using System.Collections;

public class PhaseSelectionScreen : MonoBehaviour {

	public MenusManager menus;

	public void OnBack () {
		menus.SetScreen ("title");
	}
}
