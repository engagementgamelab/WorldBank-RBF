using UnityEngine;
using System.Collections;

public class TacticPriorityGroup : ItemGroup<PlanTacticItem> {
	
	public override string Name { get { return "Priorities"; } }

	public string[] Tactics {
		get { return Items.ConvertAll (x => ((PlanTacticItem)x).Symbol).ToArray (); }
	}

	public void AddTactic (PlanTacticItem item) {
		if (!Contains (item))
			Add (item);
	}
}
