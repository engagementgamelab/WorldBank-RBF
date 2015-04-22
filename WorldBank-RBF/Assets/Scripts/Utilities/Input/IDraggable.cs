using System.Collections;

public interface IDraggable {
	bool MoveOnDrag { get; }
	void OnDragEnter (DragSettings dragSettings);
	void OnDrag (DragSettings dragSettings);
	void OnDragExit (DragSettings dragSettings);
}
