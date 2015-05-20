using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraPositioner : MB {

	class InertialScroll {

		public float AverageVelocity {
			get {
				if (velocities.Count == 0) return 0;
				float[] vs = new float[velocities.Count];
				velocities.CopyTo (vs, 0);
				float total = 0;
				for (int i = 0; i < vs.Length; i ++) {
					total += vs[i];
				}
				return total / (float)vs.Length;
			}
		}

		public readonly float multiplier = 100f;
		public readonly float damping = 6f;

		Queue<float> velocities = new Queue<float> ();
		int maxVelocityCount = 6;

		public void AddVelocity (float v) {
			if (velocities.Count+1 > maxVelocityCount) {
				velocities.Dequeue ();
			}
			velocities.Enqueue (v);
		}
	}

	bool dragEnabled = true;
	public bool DragEnabled {
		get { return dragEnabled; }
		set { dragEnabled = value; }
	}
	
	float xMin;
	public float XMin { 
		get { return xMin; }
		set {
			xMin = value;
			Transform.SetPositionX (Mathf.Max (xMin, Position.x));
		}
	}

	float speed = 10;
	float scrollSpeed = 200f;
	float maxSpeed = 2f;
	Vector3 startDragPosition;
	Vector3 startDrag;
	bool dragging = false;
	bool moving = false;
	InertialScroll inertialScroll = new InertialScroll ();

	void Awake () {
		Events.instance.AddListener<DragDownEvent> (OnDragDownEvent);
		Events.instance.AddListener<DragUpEvent> (OnDragUpEvent);
	}

	void OnDestroy() {
		Events.instance.RemoveListener<DragDownEvent> (OnDragDownEvent);
		Events.instance.RemoveListener<DragUpEvent> (OnDragUpEvent);
    }

	// TODO: This can be cleaned up
	IEnumerator CoDrag () {
		
		float prevX = Position.x;
		float velocity = 0f;

		InertialScroll dragScroll = new InertialScroll ();

		while (dragging) {
			
			// Positioning
			Vector3 w1 = ScreenPositionHandler.ViewportToWorld (startDrag);
			Vector3 w2 = ScreenPositionHandler.ViewportToWorld (MouseController.MousePositionViewport);
			float deltaX = (w1.x - w2.x);
			dragScroll.AddVelocity (deltaX);
			Transform.SetPositionX (Mathf.Max (XMin, Position.x + dragScroll.AverageVelocity));
			startDrag = MouseController.MousePositionViewport;
			
			// Inertia
			float currX = Position.x;
			velocity = (currX - prevX) * Time.deltaTime;
			inertialScroll.AddVelocity (velocity);
			prevX = currX;
			yield return null;
		}

		float inertia = dragScroll.AverageVelocity;
		while (inertia != 0 && !dragging && !moving) {
			Transform.SetPositionX (
				Mathf.Max (
					XMin,
					Position.x + inertia * inertialScroll.multiplier * Time.deltaTime
				)
			);
			inertia += -inertia * inertialScroll.damping * Time.deltaTime;
			if (Mathf.Abs (inertia) <= 0.0005f) {
				inertia = 0;
			}
			yield return null;
		}
	}

	void Update () {
		if (Input.GetKey (KeyCode.LeftArrow)) {
			Transform.SetPositionX (Mathf.Max (XMin, Position.x - 0.5f));
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			Transform.SetPositionX (Mathf.Max (XMin, Position.x + 0.5f));
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
			float newPosition = Mathf.SmoothStep (start, end, eTime / time);
			Transform.SetPositionX (Mathf.Max (XMin, newPosition));
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
