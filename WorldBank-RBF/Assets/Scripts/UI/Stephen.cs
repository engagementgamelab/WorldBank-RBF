using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Stephen : MonoBehaviour {

	public GameObject steveContainer;
	public InputField email;

	void OnEnable () {
		steveContainer.SetActive (false);
	}

	public void OnUpdateEmail () {
		steveContainer.SetActive (
			email.text == "@stove" ||
			email.text == "swalter4669@gmail.com" ||
			email.text == "steve@elab.emerson.edu" ||
			email.text == "steve@engagementgamelab.org");
	}
}
