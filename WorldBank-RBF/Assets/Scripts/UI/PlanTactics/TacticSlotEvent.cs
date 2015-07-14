using UnityEngine;

public class TacticSlotEvent : GameEvent {

	public readonly TacticItem tactic;

	public TacticSlotEvent (TacticItem tactic, int priority=-1) {
		this.tactic = tactic;
		tactic.Priority = priority;
	}
}