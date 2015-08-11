using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (BoxCollider))]
public class ParallaxElement : ParallaxImage {
	
	protected override bool Forward {
		get { return true; }
	}

	[SerializeField, HideInInspector] float xPosition = 0;
	[ExposeInWindow, ExposeProperty] public float XPosition {
		get { return xPosition; }
		set {
			xPosition = value;
			Transform.SetLocalPositionX (xPosition);
		}
	}

	[SerializeField, HideInInspector] float yPosition = 0;
	[ExposeInWindow, ExposeProperty] public float YPosition {
		get { return yPosition; }
		set { 
			yPosition = value;
			Transform.SetLocalPositionY (yPosition); 
		}
	}

	[ExposeInWindow, ExposeProperty] public float ColliderXPosition {
		get { return Collider.center.x; }
		set { Collider.SetCenterX (value); }
	}

	[ExposeInWindow, ExposeProperty] public float ColliderYPosition {
		get { return Collider.center.y; }
		set { Collider.SetCenterY (value); }
	}

	[ExposeInWindow, ExposeProperty] public float ColliderWidth {
		get { return Collider.size.x; }
		set { Collider.SetSizeX (value); }
	}

	[ExposeInWindow, ExposeProperty] public float ColliderHeight {
		get { return Collider.size.y; }
		set { Collider.SetSizeY (value); }
	}

	new BoxCollider collider = null;
	BoxCollider Collider {
		get {
			if (collider == null) {
				collider = GetComponent<BoxCollider> ();
			}
			return collider;
		}
	}

	float scale = 1f;
	public float Scale {
		get { return scale; }
		set {
			scale = Mathf.Lerp (ScaleConstraints.x, ScaleConstraints.y, value);
			LocalScale = new Vector3 (scale, scale, 1f);
			LocalPosition = new Vector3 (
				XPosition - ColliderXPosition * (scale-1f), 
				YPosition - zFocusPoint * (scale-1f),
				0f);
		}
	}

	Vector2 scaleConstraints = Vector2.zero;
	Vector2 ScaleConstraints {
		get {
			if (scaleConstraints.Equals (Vector2.zero)) {
				// MinScale, MaxScale
				scaleConstraints = new Vector2 (1f, 2.32f - 1.31f * ColliderHeight);
			}
			return scaleConstraints;
		}
	}

	float zFocusPoint {
		get { 
			if (ColliderHeight > 0.8f) {
				return ColliderYPosition + ColliderHeight * 0.67f; 
			} else {
				return ColliderYPosition;
			}
		}
	}

	public float GetPositionAtScale (float scale) {
		return Position.x + ColliderXPosition * 4f * (Mathf.Lerp (ScaleConstraints.x, ScaleConstraints.y, scale));
	}

	public virtual void Reset () {
		texture = null;
		_Texture = null;
		XPosition = 0;
		YPosition = 0;
		ColliderXPosition = 0;
		ColliderYPosition = 0;
		ColliderWidth = 1;
		ColliderHeight = 1;
		scaleConstraints = Vector2.zero;
	}
}