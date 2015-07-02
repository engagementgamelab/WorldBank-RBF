using UnityEngine;
using System.Collections;

public class TacticPriorityGroup : ModelGroup<PlanTacticItem> {
	
	public override string Name { get { return "Priorities"; } }

	public TacticPriorityGroup () : base ("") {}
}
