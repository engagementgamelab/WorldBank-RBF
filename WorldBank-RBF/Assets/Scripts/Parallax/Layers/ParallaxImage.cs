#define TEST_STANDALONE_LOAD
using UnityEngine;
// Run only if inside editor
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

/**
  * Inheritance structure:
  *	ParallaxImage
  *		> ParallaxElement
  *			> ParallaxNpc
  *			> ParallaxZoomTrigger
  */

[RequireComponent (typeof (MeshRenderer), typeof (MeshFilter))]
[JsonSerializable (typeof (Models.ParallaxImage))]
public class ParallaxImage : AnimatedQuadTexture, IEditorPoolable, IEditorRefreshable {

	[HideInInspector] public int index;
	public int Index {
		get { return index; }
		set { index = value; }
	}

	protected virtual bool Forward {
		get { return false; }
	}

	public float LocalPositionX {
		get { return LocalPosition.x; }
		set { Transform.SetLocalPositionX (value); }
	}

	[SerializeField, HideInInspector] float xOffset = 0;
	public float XOffset {
		get { return xOffset; }
		set { 
			xOffset = value;
			LocalPositionX = xOffset;
		}
	}

	public void Init () {
		texture = null;
		Reset ();
	}

	public override void Refresh () {
		base.Refresh ();
		Reset ();
	}

	void Reset () {
		Transform.Reset ();
		if (Parent != null) gameObject.layer = Parent.gameObject.layer;
		if (Forward) Transform.SetLocalPositionZ (-0.01f);
		Transform.SetLocalPositionX (XOffset);
		UpdateSortingLayer ();
	}
}