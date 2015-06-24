using UnityEngine;
using System.Collections;

public class PrioritizationManager : NotebookCanvas {

	public GameObject continueButton;

	/*void OnEnable () {
		continueButton.SetActive (NotebookManager.Instance.MakingPlan);
	}*/

	public override void Open () {
		continueButton.SetActive (NotebookManager.Instance.MakingPlan);		
	}
}
