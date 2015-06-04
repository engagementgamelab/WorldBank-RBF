using UnityEngine;
using System.Collections;

public class PrioritizationChart : MonoBehaviour {

	public TacticsColumn tacticsColumn;
	public PrioritiesColumn prioritiesColumn;
	bool open = true;

	void Awake () {
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

	void OnGUI () {
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Open")) {
			Open ();
		}
		if (GUILayout.Button ("Close")) {
			Close ();
		}
		GUILayout.EndHorizontal ();
	}
}
