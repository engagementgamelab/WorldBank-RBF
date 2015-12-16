using UnityEngine;
using System.Collections;

public class Credits2Screen : MonoBehaviour {

	public MenusManager menus;

	public void OnBack () {
		menus.SetScreen ("credits1");
	}

	public void OnNext () {
		menus.SetScreen ("title");
	}
}
