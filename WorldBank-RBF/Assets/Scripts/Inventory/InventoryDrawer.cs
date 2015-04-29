using UnityEngine;
using System.Collections;

public class InventoryDrawer : MonoBehaviour {

	Inventory inventory;

	void Awake () {
		inventory = new Inventory ();
		inventory.Add (new DayGroup ());
	}

	void OnGUI () {
		GUILayout.BeginHorizontal ();
		DayGroup group = inventory.Get<DayGroup> ();
		GUILayout.Label (group.Name);
		if (GUILayout.Button ("Add")) {
			group.Add2 ();
		}
		if (GUILayout.Button ("Remove")) {
			group.Remove ();
		}
		GUILayout.EndHorizontal ();
		for (int i = 0; i < group.Items.Count; i ++) {
			GUILayout.Label (group.Items[i].Name);
		}
		/*for (int i = 0; i < inventory.Groups.Count; i ++) {
			ItemGroup group = inventory.Groups[i];
			GUILayout.BeginHorizontal ();
			GUILayout.Label (group.Name);
			if (GUILayout.Button ("Add")) {
				group.Add ();
			}
			if (GUILayout.Button ("Remove")) {
				// group.Remove ();
			}
			GUILayout.EndHorizontal ();
			GUILayout.BeginVertical ();
			for (int j = 0; j < group.Items.Count; i ++) {
				// GUILayout.Label (group.Items[i].Name);
			}
			GUILayout.EndVertical ();
		}*/
	}
}
