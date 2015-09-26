using UnityEngine;
using System.Collections;

public class CreditsScreen : MonoBehaviour {

	public MenusManager menus;

	public void OnBack () {
		menus.SetScreen ("title");
	}
}
