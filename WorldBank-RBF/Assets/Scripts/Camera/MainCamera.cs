using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainCamera : MB {

	// Inspector Vars
	public float initialZoomLevel;

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

	LineOfSight lineOfSight = null;
	public LineOfSight LineOfSight {
		get {
			if (lineOfSight == null) {
				lineOfSight = GameObject.Find ("LineOfSight").GetScript<LineOfSight> ();
			}
			return lineOfSight;
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

	Camera myCamera;
	public Camera Camera {
		get {
			if (myCamera == null) {
				myCamera = GetComponent<Camera> ();
			}                                
			return myCamera;
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
			XMin = y * Aspect;
			Positioner.XMin = XMin;
		}
	}

	float targetZoom = 0;
	public float TargetZoom {
		get { return targetZoom; }
		set { targetZoom = value; }
	}

	float zoomVelocity = 5f;
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

	public float XMin { get; private set; }

	CameraPositioner positioner = null;
	public CameraPositioner Positioner {
		get {
			if (positioner == null) {
				positioner = Transform.parent.GetScript<CameraPositioner> ();
			}
			return positioner;
		}
	}

	const float MIN_ZOOM = 0f;
	const float MAX_ZOOM = 12f;

	void Awake () {
		// ParallaxLayerManager.Instance.onLoad += OnLoadCity;
		Events.instance.AddListener<ArriveInCityEvent> (OnArriveInCityEvent);
	}

	void Start () {
		Zoom = 0;
		TargetZoom = initialZoomLevel;
	}

	void Update () {
		/*float delta = Input.GetAxis ("Mouse ScrollWheel");
		if (delta != 0) {
			Zoom += delta;
		}*/
		/*if (!Mathf.Approximately (Zoom, TargetZoom)) {
			Zoom = Mathf.Lerp (Zoom, TargetZoom, ZoomVelocity * Time.deltaTime);
		}*/
	}

	public void ZoomToPercentage (float p, float velocity=-1) {
		ZoomTo (Mathf.Lerp (MIN_ZOOM, MAX_ZOOM, p), velocity);
	}

	public void ZoomTo (float target, float velocity=-1) {
		TargetZoom = target;
		if (velocity != -1) ZoomVelocity = velocity;
	}

	void OnLoadCity () {
		// List<ParallaxLayer> layers = ParallaxLayerManager.Instance.Layers;
		// Positioner.XMax = layers[0].RightMax;
		Positioner.XMax = ParallaxLayerManager.Instance.NearestLayer.RightMax;
	}

	void OnArriveInCityEvent (ArriveInCityEvent e) {

		string city = e.City;
		float xMax = 0f;

		switch (city) {
			case "malcom": xMax = 71f; break;
			case "mile": xMax = 57f; break;
			case "kibari": xMax = 70f; break;
			case "crup": xMax = 69f; break; 
			case "zima": xMax = 46f; break;
			case "capitol": xMax = 31f; break;
			case "valeria": xMax = 33f; break;
		}

		Positioner.XMax = xMax;
		Positioner.XPosition = ParallaxLayerManager.Instance.CameraStartPosition;
	}
}