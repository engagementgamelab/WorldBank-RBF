﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MouseController : MonoBehaviour {
	protected const int LEFT = 0;
	protected const int RIGHT = 1;

	public static Vector3 MousePosition {
		get { return ScreenPositionHandler.ScreenToWorld (Input.mousePosition); }
	}

	public static Vector3 MousePositionWorldRay {
		get {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
				return hit.point;
			}
			return Vector3.zero;
		}
	}

	public static Vector3 MousePositionViewport {
		get { return ScreenPositionHandler.ScreenToViewport (Input.mousePosition); }
	}

	ClickManager clickManager;
	DragManager dragManager;
	ReleaseManager releaseManager;

	void Awake () {
		int[] layers = LayerController.Layers;
		dragManager = new DragManager (layers);
		clickManager = new ClickManager (layers);

		// if(enableLayers)
		// {
			releaseManager = new ReleaseManager (layers);

		// }
	}

	void LateUpdate () {
		// if(!enableLayers)
		// 	return;
		if (EventSystem.current.IsPointerOverGameObject ()) return;
		if (Input.GetMouseButton (LEFT)) {
			clickManager.HandleMouseDown (LEFT);
			dragManager.HandleMouseDown (LEFT);
			releaseManager.HandleMouseDown (LEFT);
		}
		if (!Input.GetMouseButton (LEFT)) {
			clickManager.HandleMouseUp (LEFT);
			dragManager.HandleMouseUp (LEFT);
			releaseManager.HandleMouseUp (LEFT);
		}
		if (Input.GetMouseButton (RIGHT)) {
			clickManager.HandleMouseDown (RIGHT);
			dragManager.HandleMouseDown (RIGHT);
			releaseManager.HandleMouseDown (RIGHT);
		}
		if (!Input.GetMouseButton (RIGHT)) {
			clickManager.HandleMouseUp (RIGHT);
			dragManager.HandleMouseUp (RIGHT);
			releaseManager.HandleMouseUp (RIGHT);
		}
	}

	#if UNITY_EDITOR
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
			Debug.DrawRay (ray.origin, ray.direction * 1000);
		}

	}
	#endif
}
