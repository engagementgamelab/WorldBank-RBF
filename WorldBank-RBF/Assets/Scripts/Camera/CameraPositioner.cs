#undef SHOW_SETTINGS
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraPositioner : MB {

	public bool enableMovement = true;

	public class CameraDrag {

		bool enabled = true;
		public bool Enabled {
			get { return enabled; }
			set { enabled = value; }
		}

		float MouseViewportX {
			get { return ScreenPositionHandler.ViewportToWorldRelative (
					MouseController.MousePositionViewport).x;
			}
		}

		float PositionerX {
			get { return positioner.XPosition; }
			set { positioner.XPosition = value; }
		}

		float speed = 1.67f;
		public float Speed {
			get { return speed; }
			set { speed = value; }
		}

		public int Smoothing {
			get { return smoothing.Buffer; }
			set { 
				smoothing.Buffer = value;
				velocity.Buffer = value;
			}
		}

		float damping = 0.9f;
		public float Damping {
			get { return damping; }
			set { damping = Mathf.Clamp (value, 0f, 0.95f); }
		}

		public bool Dragging {
			get { return dragging; }
		}

		CameraPositioner positioner;
		LowPassFilter smoothing = new LowPassFilter (6);
		LowPassFilter velocity = new LowPassFilter (6);
		float startDragWorld;
		float startDragViewport;
		bool dragging = false;
		bool dragReleased = false;
		float releaseVelocity = 0f;

		float target;

		public CameraDrag (CameraPositioner positioner) {
			this.positioner = positioner;
		}
		
		public void OnDragDown () {
			if (!Enabled || dragging) return;
			dragging = true;
			dragReleased = false;
			startDragWorld = PositionerX;
			startDragViewport = MouseViewportX;
		}

		public void OnDrag () {
			if (dragging) {
				smoothing.Add (startDragViewport - MouseViewportX);
				float startPosition = PositionerX;
				PositionerX = startDragWorld + smoothing.Average * speed;
				velocity.Add (PositionerX - startPosition);
			}

			if (dragReleased) {
				PositionerX += releaseVelocity * 10f * Time.deltaTime;
				releaseVelocity *= Damping;
				if (Mathf.Abs (releaseVelocity) <= 0.01f) {
					Stop ();
				}
			}
		}

		public void OnDragUp () {
			if (!dragReleased ) {
				dragReleased = true;
				dragging = false;
				releaseVelocity = velocity.Average;
				smoothing.Reset ();
				velocity.Reset ();
			}
		}

		void Stop () {
			dragging = false;
			dragReleased = false;
			smoothing.Reset ();
		}
	}
	
	float xMin;
	public float XMin { 
		get { return xMin; }
		set {
			xMin = value;
			Transform.SetPositionX (Mathf.Max (xMin, Position.x));
		}
	}

	float xMax;
	public float XMax {
		get { return xMax; }
		set {
			// xMax = value + 4.5f;
			xMax = value;
			Transform.SetPositionX (Mathf.Min (xMax, Position.x));
		}
	}

	public float XPosition {
		get { return Position.x; }
		set { Transform.SetPositionX (Mathf.Clamp (value, XMin, XMax)); }
	}

	CameraDrag drag;
	public CameraDrag Drag {
		get {
			if (drag == null) {
				drag = new CameraDrag (this);
			}
			return drag;
		}
	}

	void Awake () {
		Events.instance.AddListener<DragDownEvent> (OnDragDownEvent);
		Events.instance.AddListener<DragUpEvent> (OnDragUpEvent);
	}

	void Update () {

		if(!enableMovement) return;

		Drag.OnDrag ();
		if (Drag.Enabled && NPCFocusBehavior.Instance.FocusLevel == FocusLevel.Default)
			XPosition = Position.x + Input.GetAxis ("Horizontal") * Time.deltaTime * 10f;

	}

	void OnDragDownEvent (DragDownEvent e) {
		if(!enableMovement) return;

		Drag.OnDragDown ();
	}

	void OnDragUpEvent (DragUpEvent e) {
		if(!enableMovement) return;
		
		Drag.OnDragUp ();
	}

	void OnDestroy() {

		Events.instance.RemoveListener<DragDownEvent> (OnDragDownEvent);
		Events.instance.RemoveListener<DragUpEvent> (OnDragUpEvent);		

	}

	#if SHOW_SETTINGS
	bool showSettings = false;
	void OnGUI () {
		GUI.color = Color.black;
		showSettings = GUILayout.Toggle (showSettings, "Show drag settings");
		if (!showSettings) return;
		Drag.Speed = DrawFloatSlider (Drag.Speed, 0.1f, 5f, "speed");
		Drag.Smoothing = DrawIntSlider (Drag.Smoothing, 1, 20, "smoothing");
		Drag.Damping = DrawFloatSlider (Drag.Damping, 0f, 0.95f, "damping", 2);
	}

	float DrawFloatSlider (float value, float min, float max, string label, int decimalPlaces=1) {
		GUILayout.BeginHorizontal ();
		GUILayout.Label (label, new GUILayoutOption[] { GUILayout.Width (80f)});
		float returnValue = GUILayout.HorizontalSlider (value, min, max, new GUILayoutOption[] { GUILayout.Width (120f)});
		GUILayout.Label (value.RoundToDecimal (decimalPlaces).ToString (), new GUILayoutOption[] { GUILayout.Width (40f)});
		GUILayout.EndHorizontal ();
		return returnValue;
	}

	int DrawIntSlider (int value, int min, int max, string label) {
		GUILayout.BeginHorizontal ();
		GUILayout.Label (label, new GUILayoutOption[] { GUILayout.Width (80f)});
		int returnValue = (int)GUILayout.HorizontalSlider (value, min, max, new GUILayoutOption[] { GUILayout.Width (120f)});
		GUILayout.Label (value.ToString (), new GUILayoutOption[] { GUILayout.Width (40f)});
		GUILayout.EndHorizontal ();
		return returnValue;
	}
	#endif
}
