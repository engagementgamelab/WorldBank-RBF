using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Inventory {

	Dictionary<string, ItemGroup> groups = new Dictionary<string, ItemGroup> ();
	public Dictionary<string, ItemGroup> Groups {
		get { return groups; }
	}

	public ItemGroup this[string id] {
		get { return Groups[id]; }
	}

	public void Add (ItemGroup group) {
		group.Initialize (this);
		groups.Add (group.ID, group);
	}
}
