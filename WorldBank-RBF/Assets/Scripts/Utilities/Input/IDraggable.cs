using System.Collections;

public interface IDraggable {
	void OnDragEnter (DragSettings dragSettings);
	void OnDrag (DragSettings dragSettings);
	void OnDragExit (DragSettings dragSettings);
}