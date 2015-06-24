using UnityEngine;
using System.Collections;

public class DataCanvas : NotebookCanvas {

	public GameObject feedbackPanel;

	public override void Open () {
		feedbackPanel.SetActive (NotebookManager.Instance.MakingPlan);
	}
}
