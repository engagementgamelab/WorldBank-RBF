using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider))]
public class ParallaxElement : ParallaxImage {
	
	protected override bool Forward {
		get { return true; }
	}

	float xPosition = 0;
	[ExposeInWindow, ExposeProperty] public float XPosition {
		get { return xPosition; }
		set {
			xPosition = value;
			Transform.SetLocalPositionX (xPosition);
		}
	}

	float yPosition = 0;
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

	BoxCollider collider = null;
	BoxCollider Collider {
		get {
			if (collider == null) {
				collider = GetComponent<BoxCollider> ();
			}
			return collider;
		}
	}

	public virtual void Reset () {
		texture = null;
		Texture = null;
		XPosition = 0;
		YPosition = 0;
		ColliderXPosition = 0;
		ColliderYPosition = 0;
		ColliderWidth = 1;
		ColliderHeight = 1;
	}
}
