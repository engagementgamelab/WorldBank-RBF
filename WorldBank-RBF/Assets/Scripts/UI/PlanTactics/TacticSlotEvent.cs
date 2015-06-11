using UnityEngine;

public class TacticSlotEvent : GameEvent {

	public readonly int slotIndex;
	public readonly bool slotAssigned;

	public TacticSlotEvent(int index, bool selected=true) {
		this.slotIndex = index;
		this.slotAssigned = selected;
	}

}