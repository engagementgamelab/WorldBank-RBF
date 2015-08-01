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

			Debug.Log(PlayerData.TacticPriorityGroup.Count);

   		continuePlanButton.gameObject.SetActive(PlayerData.TacticPriorityGroup.Count == 6);
    }
}
