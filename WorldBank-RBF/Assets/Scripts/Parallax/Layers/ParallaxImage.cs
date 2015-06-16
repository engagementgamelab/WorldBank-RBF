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

	float layerPosition = 10000;
	public float LayerPosition {
		get { return layerPosition; }
		set {
			layerPosition = 10000 - value * 100;
			
			// Prevents z fighting
			if (_Material != null) {
				_Material.renderQueue = (int)layerPosition;
			}
		}
	}

	protected virtual bool Forward {
		get { return false; }
	}

	#if UNITY_EDITOR
	public string TexturePath {
		get { return AssetDatabase.GetAssetPath (_Texture); }
		set { 
			_Texture = AssetDatabase.LoadAssetAtPath (value, typeof (Texture2D)) as Texture2D;
			texture = _Texture;
		}
	}
	#endif

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
		_Texture = null;
		SetRenderQueue ();
		Reset ();
	}

	public override void Refresh () {
		base.Refresh ();
		SetRenderQueue ();
		Reset ();
	}

	void Reset () {
		Transform.Reset ();
		if (Parent != null) gameObject.layer = Parent.gameObject.layer;
		if (Forward) Transform.SetLocalPositionZ (-0.01f);
		Transform.SetLocalPositionX (XOffset);
		LayerPosition = Position.z;
	}

	void SetRenderQueue () {
		if (_Material != null)
			_Material.renderQueue = (int)LayerPosition;
	}
}