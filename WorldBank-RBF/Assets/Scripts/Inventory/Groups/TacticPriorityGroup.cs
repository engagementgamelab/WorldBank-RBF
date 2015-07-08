using UnityEngine;
using System.Collections;

public class TacticPriorityGroup : ItemGroup<TacticItem> {
	
	public override string Name { get { return "Priorities"; } }

	public string[] Tactics {
		get { return Items.ConvertAll (x => ((TacticItem)x).Symbol).ToArray (); }
	}

	public void AddTactic (TacticItem item) {
		if (!Contains (item))
			Add (item);
	}
}
