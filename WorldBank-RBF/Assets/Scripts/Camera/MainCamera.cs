using UnityEngine;
using System.Collections;

public class MainCamera : MB {

	static MainCamera instance = null;
	static public MainCamera Instance {
		get {
			if (instance == null) {
				instance = Object.FindObjectOfType (typeof (MainCamera)) as MainCamera;
				if (instance == null) {
					GameObject go = new GameObject ("MainCamera");
					DontDestroyOnLoad (go);
					instance = go.AddComponent<MainCamera>();
				}
			}
			return instance;
		}
	}

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
	public float Zoom {
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

	float targetZoom = 0;
	public float TargetZoom {
		get { return targetZoom; }
		set { targetZoom = value; }
	}

	float zoomVelocity = 3f;
	public float ZoomVelocity {
		get { return zoomVelocity; }
		set { zoomVelocity = value; }
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
	bool moving = false;

	void Awake () {
		Events.instance.AddListener<ReleaseEvent> (OnReleaseEvent);
		Events.instance.AddListener<DragDownEvent> (OnDragDownEvent);
		Events.instance.AddListener<DragUpEvent> (OnDragUpEvent);
	}

	void Start () {
		Zoom = 2;
		TargetZoom = 2;
	}

	void Update () {
		/*float delta = Input.GetAxis ("Mouse ScrollWheel");
		if (delta != 0) {
			Zoom += delta;
		}*/

		Zoom = Mathf.Lerp (Zoom, TargetZoom, ZoomVelocity * Time.deltaTime);
	}

	public void ZoomTo (float target, float velocity=-1) {
		TargetZoom = target;
		if (velocity != -1) ZoomVelocity = velocity;
	}

	/*public void ZoomTo (float to, float duration) {
		StartCoroutine (CoZoomTo (Zoom, to, duration));
	}

	IEnumerator CoZoomTo (float from, float to, float duration) {
		
		float eTime = 0f;
	
		while (eTime < duration) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (0, 1, eTime / duration);
			Zoom = Mathf.Lerp (from, to, progress);
			yield return null;
		}
	}*/
	
	public void MoveToTarget (float target, float duration=-1) {
		if (moving) return;
		moving = true;
		StartCoroutine (CoMove (Position.x, Mathf.Max (0, target), duration));
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

	IEnumerator CoDrag () {
		
		while (dragging) {
			Vector3 w1 = ScreenPositionHandler.ViewportToWorld (startDrag);
			Vector3 w2 = ScreenPositionHandler.ViewportToWorld (MouseController.MousePositionViewport);
			float deltaX = (w1.x - w2.x);
			Transform.SetPositionX (Mathf.Max (xMin, startDragPosition.x + deltaX));
			yield return null;
		}
	}

	/**
	 *	Events
	 */

	void OnReleaseEvent (ReleaseEvent e) {
		/*if (!e.releaseSettings.left) {
			MoveToTarget (MouseController.MousePositionWorldRay.x);
		}*/
	}

	void OnDragDownEvent (DragDownEvent e) {
		if (!dragging) {
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