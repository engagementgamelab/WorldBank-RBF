using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlanSelectionScreen : MonoBehaviour {

	public Text instructions;
	public PlanContainer plan;
	public IndicatorsSummary yourPlan;
	public IndicatorsSummary thisPlan;
	public PreviewPosition previewPosition;

	List<Models.Plan> plans;

	int PlanCount {
		get { return 5; } //plans.Count-1; }
	}
	
	int planIndex;
	int PlanIndex {
		get { return planIndex; }
		set {
			planIndex = value;
			previewPosition.Position = value;
		}
	}

	public void Init (List<Models.Plan> plans) {
		this.plans = plans;
	}

	public void NextPlan () {
		if (PlanIndex < PlanCount)
			PlanIndex ++;
	}

	public void PreviousPlan () {
		if (PlanIndex > 0)
			PlanIndex --;
	}

	public void Continue () {
		Debug.Log ("continue");
	}

	public void GoBack () {
		Debug.Log ("go back");
	}
}
