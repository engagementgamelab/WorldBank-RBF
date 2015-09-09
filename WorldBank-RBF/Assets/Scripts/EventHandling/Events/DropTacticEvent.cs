
public class DropTacticEvent : GameEvent {

	public readonly Tactic Tactic;
	public readonly DragLocation DropLocation;
	
	public DropTacticEvent (Tactic tactic, DragLocation dropLocation) {
		Tactic = tactic;
		DropLocation = dropLocation;
	}
}
