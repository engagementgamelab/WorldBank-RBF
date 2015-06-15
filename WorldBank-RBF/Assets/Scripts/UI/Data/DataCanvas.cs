using UnityEngine;
using System.Collections;

public class DataCanvas : MonoBehaviour {

	public GameObject feedbackPanel;

	void OnEnable () {
		feedbackPanel.SetActive (NotebookManager.Instance.MakingPlan);
	}
}
