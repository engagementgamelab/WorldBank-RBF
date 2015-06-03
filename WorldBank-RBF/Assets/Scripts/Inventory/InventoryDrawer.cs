using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryDrawer : MonoBehaviour {

	Inventory inventory;
	PlanTacticGroup tactics;
	TacticPriorityGroup priorities;

	GUILayoutOption[] labelWidth = new GUILayoutOption[] { GUILayout.Width (450) };
	GUILayoutOption[] buttonWidth = new GUILayoutOption[] { GUILayout.Width (30) };

	void Awake () {
		inventory = new Inventory ();
		// inventory.Add (new DayGroup ());
		// inventory.Add (new CityGroup ());
		tactics = new PlanTacticGroup ();
		inventory.Add (tactics);
		tactics.Add (new PlanTacticItem (null, 1));
		tactics.Add (new PlanTacticItem (null, 2));
		tactics.Add (new PlanTacticItem (null, 3));

		priorities = new TacticPriorityGroup ();
		inventory.Add (priorities);
	}

	void OnGUI () {
		GUILayout.BeginHorizontal ();
		GUILayout.Label (tactics.Name);
		GUILayout.EndHorizontal ();
		List<PlanTacticItem> tacticItems = tactics.Items.ConvertAll (x => (PlanTacticItem)x);
		int tacticCount = tacticItems.Count;
		for (int i = 0; i < tacticCount; i ++) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label (tacticItems[i].Title, labelWidth);
			if (GUILayout.Button ("+", buttonWidth)) {
				tactics.Transfer (priorities, tacticItems[i]);
			}
			GUILayout.EndHorizontal ();
		}

		GUILayout.BeginHorizontal ();
		GUILayout.Label (priorities.Name);
		GUILayout.EndHorizontal ();
		List<PlanTacticItem> priorityItems = priorities.Items.ConvertAll (x => (PlanTacticItem)x);
		int priorityCount = priorityItems.Count;
		for (int i = 0; i < priorityCount; i ++) {
			PlanTacticItem item = priorityItems[i];
			GUILayout.BeginHorizontal ();
			GUILayout.Label (item.Title, labelWidth);
			if (GUILayout.Button ("-", buttonWidth)) {
				priorities.Transfer (tactics, item);
			}
			if (GUILayout.Button ("▲", buttonWidth)) {
				priorities.MoveItemUp (item);
			}
			if (GUILayout.Button ("▼", buttonWidth)) {
				priorities.MoveItemDown (item);	
			}
			GUILayout.EndHorizontal ();			
		}
		// DrawGroup<DayGroup> ();
		// DrawGroup<CityGroup> ();
	}

	void DrawGroup<T> () where T : ItemGroup {
		GUILayout.BeginHorizontal ();
		T group = inventory.Get<T> () as T;
		GUILayout.Label (group.Name);
		if (GUILayout.Button ("Add")) {
			group.Add ();
		}
		if (GUILayout.Button ("Remove")) {
			group.Remove ();
		}
		GUILayout.EndHorizontal ();
		for (int i = 0; i < group.Items.Count; i ++) {
			GUILayout.Label (group.Items[i].Name);
		}
	}
}
