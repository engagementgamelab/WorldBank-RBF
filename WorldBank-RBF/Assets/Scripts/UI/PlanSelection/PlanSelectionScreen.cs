using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;

public class PlanSelectionScreen : MonoBehaviour {

	public MenusManager menus;

	public Text header;
	public PlanContainer plan;
	public IndicatorsSummary yourPlan;
	public IndicatorsSummary thisPlan;
	public PreviewPosition previewPosition;

	public GameObject leftArrow;
	public GameObject rightArrow;

	public Text[] tacticsLabels;
	public Text[] indicatorsLabels;

	Models.PlanRecord[] plans;

	int PlanCount {
		get { return 4; } //plans.Count-1; }
	}
	
	int planIndex;
	int PlanIndex {
		get { return planIndex; }
		set {
			planIndex = value;
			previewPosition.Position = value;
		}
	}

	public void Init () {
		// this.plans = plans;

        // Insert user ID
		Dictionary<string, object> userField = new Dictionary<string, object> {{ "user_id", PlayerManager.Instance.ID }};

	    // Get plans
        NetworkManager.Instance.PostURL("/plan/all/", userField, PlansRetrieved);


	}

	public void NextPlan () {
		if (PlanIndex < PlanCount)
			PlanIndex ++;

		rightArrow.SetActive(!(PlanIndex == PlanCount-1));
		leftArrow.SetActive(!(PlanIndex == 0));

		ShowPlan(PlanIndex);
	}

	public void PreviousPlan () {
		if (PlanIndex > 0)
			PlanIndex --;

		rightArrow.SetActive(!(PlanIndex == PlanCount-1));
		leftArrow.SetActive(!(PlanIndex == 0));

		ShowPlan(PlanIndex);
	}

	public void Continue () {
		Application.LoadLevel("PhaseTwo");
	}

	public void GoBack () {
		menus.SetScreen ("title");
	}

	void ShowPlan(int planIndex) {

    	int tactInt = 0;

    	foreach(Text label in tacticsLabels) {

    		label.text = PlayerData.TacticGroup.GetName(plans[planIndex].tactics[tactInt]);
    		tactInt++;

    	}

    	indicatorsLabels[0].text = "Facility Births: " + plans[planIndex].default_affects[0]+"%";
    	indicatorsLabels[1].text = "Vaccinations: " + plans[planIndex].default_affects[1]+"%";
    	indicatorsLabels[2].text = "Quality of Care: " + plans[planIndex].default_affects[2]+"%";

    	header.text = plans[planIndex].name;
    	DataManager.currentPlanId = plans[planIndex]._id;

	}

    /// <summary>
    /// Callback that handles all display for plans after they are retrieved.
    /// </summary>
    /// <param name="response">Textual response from /plan/all/ endpoint.</param>
    void PlansRetrieved(Dictionary<string, object> response) {

    	Debug.Log(response);

    	 plans = JsonReader.Deserialize<Models.PlanRecord[]>(response["plans"].ToString());

    	 ShowPlan(0);

    }
}
