using UnityEngine;
using System.Collections;

public class CameraPositioner : MB {

	bool dragEnabled = true;
	public bool DragEnabled {
		get { return dragEnabled; }
		set { dragEnabled = value; }
	}
	
	public float XMin { get; set; }

	float speed = 10;
	Vector3 startDragPosition;
	Vector3 startDrag;
	bool dragging = false;
	bool moving = false;

	void Awake () {
		Events.instance.AddListener<DragDownEvent> (OnDragDownEvent);
		Events.instance.AddListener<DragUpEvent> (OnDragUpEvent);
	}

	IEnumerator CoDrag () {
		
		while (dragging) {
			Vector3 w1 = ScreenPositionHandler.ViewportToWorld (startDrag);
			Vector3 w2 = ScreenPositionHandler.ViewportToWorld (MouseController.MousePositionViewport);
			float deltaX = (w1.x - w2.x);
			Transform.SetPositionX (Mathf.Max (XMin, startDragPosition.x + deltaX));
			yield return null;
		}
	}

	public void MoveToTarget (float target, float duration=-1) {
		if (moving) return;
		moving = true;
		StartCoroutine (CoMove (Position.x, target, duration));
	}

	IEnumerator CoMove (float start, float end, float duration=-1) {

		float distance = Mathf.Abs (start - end);
		float time = (duration == -1) ? distance / speed : duration;
		float eTime = 0;

		while (eTime < time) {
			eTime += Time.deltaTime;
			Transform.SetPositionX (Mathf.SmoothStep (start, end, eTime / time));
			yield return null;
		}

		moving = false;
	}

	void OnDragDownEvent (DragDownEvent e) {
		if (DragEnabled && !dragging) {
			startDragPosition = Position;
			startDrag = MouseController.MousePositionViewport;
			dragging = true;
			StartCoroutine (CoDrag ());
		}
	}

	void OnDragUpEvent (DragUpEvent e) {
		dragging = false;
	}
}
