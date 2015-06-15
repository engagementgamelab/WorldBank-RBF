using UnityEngine;
using System.Collections;

public class PrioritizationManager : MonoBehaviour {

	public GameObject continueButton;

	void OnEnable () {
		continueButton.SetActive (NotebookManager.Instance.MakingPlan);
	}
}
