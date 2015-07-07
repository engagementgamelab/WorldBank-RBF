using UnityEngine;
using System.Collections;

public class PlanTacticItem : ModelItem {
	
	public override string Name { get { return "Plan Tactic"; } }
	
	public string Title {
		get { return Model.title; }
	}

	public string Symbol {
		get { return Model.symbol; }
	}

	public string Description {
		get { return Model.description[0]; }
	}

	int priority = -1;
	public int Priority {
		get { return priority; }
		set { priority = value; }
	}
}

