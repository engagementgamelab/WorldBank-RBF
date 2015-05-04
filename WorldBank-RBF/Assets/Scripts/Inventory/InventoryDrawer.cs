using UnityEngine;
using System.Collections;

public class InventoryDrawer : MonoBehaviour {

	Inventory inventory;

	void Awake () {
		inventory = new Inventory ();
		inventory.Add (new DayGroup ());
		inventory.Add (new CityGroup ());
	}

	void OnGUI () {
		DrawGroup<DayGroup> ();
		DrawGroup<CityGroup> ();
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
