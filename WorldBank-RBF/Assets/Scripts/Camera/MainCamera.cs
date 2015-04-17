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
	public Camera Camera {
		get {
			if (camera == null) {
				camera = GetComponent<Camera> ();
			}
			return camera;
		}
	}

	float zoom = 0;
	float Zoom {
		get { return zoom; }
		set {
			zoom = Mathf.Clamp (value, 0, 15);
			float y = -Mathf.Tan (FOV / 2f * Mathf.Deg2Rad) * zoom;
			//Transform.SetPosition (new Vector3 (Position.x, y, zoom)); 		// Zoom to bottom
			Transform.SetPosition (new Vector3 (Position.x, Position.y, zoom)); // Zoom to middle
			xMin = y * Aspect;
			Transform.SetPositionX (Mathf.Max (xMin, Position.x));
		}
	}

	float altitude = 0;
	float Altitude {
		get { return altitude; }
		set {
			// rotating on x set the altitude 
			// (which affects y and z positions)
			altitude = value;
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

	void Start () {
		Zoom = 2;
	}

	void Update () {
		float delta = Input.GetAxis ("Mouse ScrollWheel");
		if (delta != 0) {
			Zoom += delta;
		}
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
			Vector3 w1 = ScreenPositionHandler.ViewportToWorld (startDrag);
			Vector3 w2 = ScreenPositionHandler.ViewportToWorld (MouseController.ViewportMousePosition);
			float deltaX = (w1.x - w2.x);
			Transform.SetPositionX (Mathf.Max (xMin, startDragPosition.x + deltaX));
			yield return null;
		}
	}

	/**
	 *	Events
	 */

	void OnReleaseEvent (ReleaseEvent e) {
		if (!e.releaseSettings.left) {
			Move (MouseController.MousePosition.x);
		}
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