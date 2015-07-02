using UnityEngine;
using System.Collections;

public class PlanTacticItem : ModelItem {
	
	public override string Name { get { return "Plan Tactic"; } }
	
	public string Title {
		get { return model.title; }
	}

	public string Symbol {
		get { return model.symbol; }
	}

	public string Description {
		get { return model.description[0]; }
	}

	int priority = -1;
	public int Priority {
		get { return priority; }
		set { priority = value; }
	}
}

