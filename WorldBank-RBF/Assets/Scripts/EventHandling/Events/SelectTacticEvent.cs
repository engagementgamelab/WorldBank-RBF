using UnityEngine;
using System.Collections;

public class SelectTacticEvent : GameEvent {

	public readonly PlanTacticItem tactic;

	public SelectTacticEvent (PlanTacticItem tactic) {
		this.tactic = tactic;
	}
}
