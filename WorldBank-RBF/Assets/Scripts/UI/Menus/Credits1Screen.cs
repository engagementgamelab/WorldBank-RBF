using UnityEngine;
using System.Collections;

public class Credits1Screen : MonoBehaviour {

	public MenusManager menus;

	public void OnBack () {
		menus.SetScreen ("title");
	}

	public void OnNext () {
		menus.SetScreen ("credits2");
	}
}
