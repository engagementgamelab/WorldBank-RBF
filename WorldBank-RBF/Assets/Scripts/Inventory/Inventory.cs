using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The Inventory contains ItemGroups.
/// </summary>
public class Inventory {

	Dictionary<string, ItemGroup> groups = new Dictionary<string, ItemGroup> ();

	/// <summary>
	/// Get the ItemGroups.
	/// </summary>
	public Dictionary<string, ItemGroup> Groups {
		get { return groups; }
	}

	/// <summary>
	/// Get an ItemGroup using bracket notation.
	/// </summary>
	public ItemGroup this[string id] {
		get { return Groups[id]; }
	}

	/// <summary>
	/// Add an ItemGroup.
	/// </summary>
	/// <param name="group">The ItemGroup to add.</param>
	public void Add (ItemGroup group) {
		group.Initialize (this);
		groups.Add (group.ID, group);
	}
}
