using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PrioritizationChart : MonoBehaviour {

	public TacticsColumn tacticsColumn;
	public PrioritiesColumn prioritiesColumn;
	
	public Button continuePlanButton;

	bool open = true;

	int tacticsAssigned = 0;

	void Awake () {
		Close ();
		
		PlayerData.PopulateTestTactics ();

		// Listen for TacticSlotEvent
		Events.instance.AddListener<TacticSlotEvent>(OnTacticEvent);
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
		prioritiesColumn.SetPriorities (
			tacticsColumn.Init (PlayerData.PlanTacticGroup, PlayerData.TacticPriorityGroup));
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

	/*void OnGUI () {
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Open")) {
			Open ();
		}
		if (GUILayout.Button ("Close")) {
			Close ();
		}
		GUILayout.EndHorizontal ();
	}*/
}
