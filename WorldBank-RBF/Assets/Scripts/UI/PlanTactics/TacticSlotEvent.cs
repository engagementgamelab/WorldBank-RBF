using UnityEngine;

public class TacticSlotEvent : GameEvent {

	public readonly PlanTacticItem tactic;

	public TacticSlotEvent (PlanTacticItem tactic, int priority=-1) {
		this.tactic = tactic;
		tactic.Priority = priority;
	}
}