﻿using UnityEngine;
using System.Collections;

public abstract class MouseButtonHandler<T> where T : class {

	protected const int LEFT = 0;
	protected const int RIGHT = 1;
	protected readonly bool left = true;
	bool mouseDown = false;

	T moused = null;
	protected T Moused {
		get { return moused; }
	}

	Vector2 mousePosition = Vector2.zero;
	protected Vector2 MousePosition {
		get { return mousePosition; }
		set { mousePosition = value; }
	}
	
	int layer = -1;
	protected int Layer { get { return layer; } }

	bool layerHit = false;
	protected bool LayerHit { get { return layerHit; } }

	public MouseButtonHandler (bool left, int layer) {
		this.left = left;
		this.layer = layer;
	}

	public virtual void HandleMouseDown () {
		MousePosition = Input.mousePosition;
		if (!mouseDown) {
			moused = GetMouseOver ();
			OnDown ();
			mouseDown = true;
		} else {
			OnHold ();
		}
	}

	public virtual void HandleMouseUp () {
		if (mouseDown) {
			OnUp ();
			moused = null;
			mouseDown = false;
		}
	}

	protected virtual void OnDown () {}
	protected virtual void OnHold () {}
	protected virtual void OnUp () {}

	protected T GetMouseOver () {
		Ray ray = Camera.main.ScreenPointToRay (MousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, Mathf.Infinity, 1 << layer)) {
			layerHit = true;
			return hit.transform.GetScript<T> ();
		} else {
			layerHit = false;
			return null;
		}
	}
}
