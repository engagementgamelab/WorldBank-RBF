using UnityEngine;
using System.Collections;

public class SelectTacticEvent : GameEvent {

	public readonly TacticItem tactic;

	public SelectTacticEvent (TacticItem tactic) {
		this.tactic = tactic;
	}
}
