using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider))]
public class ParallaxElement : ParallaxImage {
	
	protected override bool Forward {
		get { return true; }
	}

	float xPosition = 0;
	[WindowExposed, ExposeProperty] public float XPosition {
		get { return xPosition; }
		set {
			xPosition = value;
			Transform.SetLocalPositionX (xPosition);
		}
	}

	float yPosition = 0;
	[WindowExposed, ExposeProperty] public float YPosition {
		get { return yPosition; }
		set { 
			yPosition = value;
			Transform.SetLocalPositionY (yPosition); 
		}
	}

	[WindowExposed, ExposeProperty] public float ColliderXPosition {
		get { return Collider.center.x; }
		set { Collider.SetCenterX (value); }
	}

	[WindowExposed, ExposeProperty] public float ColliderYPosition {
		get { return Collider.center.y; }
		set { Collider.SetCenterY (value); }
	}

	[WindowExposed, ExposeProperty] public float ColliderWidth {
		get { return Collider.size.x; }
		set { Collider.SetSizeX (value); }
	}

	[WindowExposed, ExposeProperty] public float ColliderHeight {
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
}
