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
		get { 
			try {
				return Groups[id];
			}	 
			catch(System.Exception e) {
				throw new System.Exception("Unable to find ItemGroup for ID '" + id + "'");
			} 
		}
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
