
public class DropTacticEvent : GameEvent {

	public readonly Tactic Tactic;
	
	public DropTacticEvent (Tactic tactic) {
		Tactic = tactic;
	}
}
