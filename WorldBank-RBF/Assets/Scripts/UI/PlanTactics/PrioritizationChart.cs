using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PrioritizationChart : MonoBehaviour {

	public TacticsColumn tacticsColumn;
	public PrioritiesColumn prioritiesColumn;
	public Button continuePlanButton;
	int tacticsAssigned = 0;

	void Awake () {
		// Listen for TacticSlotEvent
		Events.instance.AddListener<TacticSlotEvent>(OnTacticEvent);
	}

	void Start () {
		// TODO: FOR TESTING BUILD ONLY; since capital city is not ready, add these to inventory by default
		PlayerData.UnlockItem ("unlockable_vouchers_for_services");
		PlayerData.UnlockItem ("unlockable_information_campaign_to_explain_changes_to_system");
		PlayerData.UnlockItem ("unlockable_incentivise_providers_to_deliver_services");
	}

	void OnEnable () {
		tacticsColumn.gameObject.SetActive (true);
		prioritiesColumn.gameObject.SetActive (true);
	}

	void OnDisable () {
		tacticsColumn.gameObject.SetActive (false);
		prioritiesColumn.gameObject.SetActive (false);
	}

    /// <summary>
    // Callback for TacticSlotEvent, filtering for type of event
    /// </summary>
    void OnTacticEvent(TacticSlotEvent e) {

    	if (e.tactic.Priority > -1)
			PlayerData.TacticPriorityGroup.AddTactic (e.tactic);
		else
			PlayerData.TacticPriorityGroup.Remove (e.tactic);

   		continuePlanButton.gameObject.SetActive(PlayerData.TacticPriorityGroup.Count == 6);
    }
}
