using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PrioritizationChart : MonoBehaviour {

	public TacticsColumn tacticsColumn;
	public PrioritiesColumn prioritiesColumn;
	bool open = false;
	public Button continuePlanButton;
	int tacticsAssigned = 0;

	void Awake () {
		// Listen for TacticSlotEvent
		Events.instance.AddListener<TacticSlotEvent>(OnTacticEvent);

		// TODO: FOR TESTING BUILD ONLY; since capital city is not ready, add these to inventory by default
		PlayerData.UnlockImplementation ("unlockable_vouchers_for_services");
		PlayerData.UnlockImplementation ("unlockable_information_campaign_to_explain_changes_to_system");
		PlayerData.UnlockImplementation ("unlockable_incentivise_providers_to_deliver_services");
	}

	void OnEnable () {
		Open ();
	}

	void OnDisable () {
		Close ();
	}

	public void Open () {
		if (open) return;
		tacticsColumn.gameObject.SetActive (true);
		prioritiesColumn.gameObject.SetActive (true);
		// prioritiesColumn.SetPriorities (
		// 	tacticsColumn.Init (PlayerData.PlanTacticGroup, PlayerData.TacticPriorityGroup));
		open = true;
	}

	public void Close () {
		if (!open) return;
		PlayerData.SetTactics (tacticsColumn.GetTactics ());
		PlayerData.SetPriorities (prioritiesColumn.GetPriorities ());
		tacticsColumn.ResetTactics ();
		tacticsColumn.gameObject.SetActive (false);
		prioritiesColumn.gameObject.SetActive (false);
		open = false;
	}

    /// <summary>
    // Callback for TacticSlotEvent, filtering for type of event
    /// </summary>
    void OnTacticEvent(TacticSlotEvent e) {

    	if(e.slotAssigned)
	    	tacticsAssigned++;
    	else
	    	tacticsAssigned--;

   		continuePlanButton.gameObject.SetActive(tacticsAssigned == 6);

    }

    /*#if DEBUG
    void OnGUI () {
    	GUILayout.Space (25);
    	if (GUILayout.Button ("unlock tactics")) {
    		PlayerData.UnlockImplementation ("unlockable_incentivise_providers_to_follow_protocols");
    		PlayerData.UnlockImplementation ("unlockable_improve_patient_and_provider_relationship");
    		PlayerData.UnlockImplementation ("unlockable_make_aesthetic_improvements");
    		PlayerData.UnlockImplementation ("unlockable_information_campaign_to_change_cultural_customs_and_behavior");
    		open = false;
    		Open ();
    	}
    }
    #endif*/
}
