using UnityEngine;
using System.Collections;

/// <summary>
/// A plan tactic based on a data model.
/// </summary>
public class TacticItem : ModelItem {
	
	public override string Name { get { return "Tactic"; } }
	
	/// <summary>
	/// Gets the title.
	/// </summary>
	public string Title {
		get { return Model.title; }
	}

	/// <summary>
	/// Gets the model symbol.
	/// </summary>
	new public string Symbol {
		get { return Model.symbol; }
	}

	/// <summary>
	/// Gets the description.
	/// </summary>
	public string Description {
		get { return Model.description[0]; }
	}

	int priority = -1;
	/// <summary>
	/// Gets/sets the priority (if priority is -1, the tactic is interpreted as not being in the plan)
	/// </summary>
	public int Priority {
		get { return priority; }
		set { priority = value; }
	}
}

