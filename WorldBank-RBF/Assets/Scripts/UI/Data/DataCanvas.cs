using UnityEngine;
using System.Collections;

public class DataCanvas : NotebookCanvas {

	public GameObject feedbackPanel;

	/*void OnEnable () {
		feedbackPanel.SetActive (NotebookManager.Instance.MakingPlan);
	}*/

	public override void Open () {
		feedbackPanel.SetActive (NotebookManager.Instance.MakingPlan);
	}
}
