using UnityEngine;
using System.Collections;

/// <summary>
/// Contains the tactics that have been set as priorities (i.e. added to the plan).
/// </summary>
public class TacticPriorityGroup : ItemGroup<TacticItem> {
	
	public override string ID { get { return "priorities"; } }

	/// <summary>
	/// Gets an array of model symbols for each tactic.
	/// </summary>
	public string[] Tactics {
		get { return Items.ConvertAll (x => ((TacticItem)x).Symbol).ToArray (); }
	}

	/// <summary>
	/// Adds a tactic to the group if it doesn't already exist.
	/// </summary>
	/// <param name="tactic">The TacticItem to add to the plan.</param>
	public void AddTactic (TacticItem tactic) {
		if (!Contains (tactic))
			Add (tactic);
	}
}
