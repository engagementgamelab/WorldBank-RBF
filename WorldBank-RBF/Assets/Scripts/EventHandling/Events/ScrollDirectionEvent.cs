
public class ScrollDirectionEvent : GameEvent {

	public readonly bool Vertical;

	public ScrollDirectionEvent (bool vertical) {
		Vertical = vertical;
	}
}
