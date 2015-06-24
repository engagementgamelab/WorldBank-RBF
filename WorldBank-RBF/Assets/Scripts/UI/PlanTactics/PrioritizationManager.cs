using UnityEngine;
using System.Collections;

public class PrioritizationManager : NotebookCanvas {

	public GameObject continueButton;

	public override void Open () {
		continueButton.SetActive (NotebookManager.Instance.MakingPlan);		
	}
}
