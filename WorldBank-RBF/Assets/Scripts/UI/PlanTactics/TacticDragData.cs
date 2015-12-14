
public class TacticDragData : System.Object {

	public readonly Tactic Tactic;
	public DragLocation FromLocation { get; set; }
	public DragLocation ToLocation { get; set; }

	public TacticSlot FromSlot {
		get { return FromLocation as TacticSlot; }
	}

	public TacticsContainer FromContainer {
		get { return FromLocation as TacticsContainer; }
	}

	public TacticSlot ToSlot {
		get { return ToLocation as TacticSlot; }
	}

	public TacticsContainer ToContainer {
		get { return ToLocation as TacticsContainer; }
	}

	public bool WasMoved {
		get { return FromLocation != ToLocation; }
	}

	public TacticDragData (Tactic tactic, DragLocation fromLocation) {
		Tactic = tactic;
		FromLocation = fromLocation;
		ToLocation = null;
	}
}
