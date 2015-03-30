using UnityEngine;
using System.Collections;

public class MainCamera : MB {

	float fov = -1;
	public float FOV {
		get {
			if (fov == -1) {
				fov = Camera.fieldOfView;
			}
			return fov;
		}
	}

	float aspect = -1;
	public float Aspect {
		get {
			if (aspect == -1) {
				aspect = Camera.aspect;
			}
			return aspect;
		}
	}

	new Camera camera;
	Camera Camera {
		get {
			if (camera == null) {
				camera = GetComponent<Camera> ();
			}
			return camera;
		}
	}

	float speed = 10;
	float xMin = 0;
	Vector3 startDragPosition;
	Vector3 startDrag;
	bool dragging = false;

	void Awake () {
		Events.instance.AddListener<ReleaseEvent> (OnReleaseEvent);
		Events.instance.AddListener<DragDownEvent> (OnDragDownEvent);
		Events.instance.AddListener<DragUpEvent> (OnDragUpEvent);
	}

	public Vector3 WorldToViewportPoint (Vector3 worldPoint) {
		return Camera.WorldToViewportPoint (worldPoint);
	}

	public Vector3 ViewportToWorldPoint (Vector3 viewportPoint) {
		return Camera.ViewportToWorldPoint (viewportPoint);
	}

	void Move (float target) {
		StartCoroutine (CoMove (Position.x, Mathf.Max (0, target)));
	}

	IEnumerator CoMove (float start, float end) {
		
		float distance = Mathf.Abs (start - end);
		float time = distance / speed;
		float eTime = 0;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			Transform.SetPositionX (Mathf.SmoothStep (start, end, eTime / time));
			yield return null;
		}
	}

	IEnumerator CoDrag () {
		
		while (dragging) {
			Vector3 w1 = ViewportToWorldPoint (startDrag);
			Vector3 w2 = ViewportToWorldPoint (MouseController.ViewportMousePosition);
			float deltaX = (w1.x - w2.x);
			Transform.SetPositionX (Mathf.Max (xMin, startDragPosition.x + deltaX));
			yield return null;
		}
	}

	/**
	 *	Events
	 */

	void OnReleaseEvent (ReleaseEvent e) {
		//Move (MouseController.MousePosition.x);
	}

	void OnDragDownEvent (DragDownEvent e) {
		if (!dragging) {
			startDragPosition = Position;
			startDrag = MouseController.ViewportMousePosition;
			dragging = true;
			StartCoroutine (CoDrag ());
		}
	}

	void OnDragUpEvent (DragUpEvent e) {
		dragging = false;
	}
}