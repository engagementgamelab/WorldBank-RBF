using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class AnimatedQuadTexture : MB {

	protected Material _Material {
		get { return _MeshRenderer.sharedMaterial; }
		set { _MeshRenderer.sharedMaterial = value; }
	}

	MeshRenderer meshRenderer = null;
	MeshRenderer _MeshRenderer {
		get {
			if (meshRenderer == null) {
				meshRenderer = Transform.GetComponent<MeshRenderer> ();
			}
			return meshRenderer;
		}
	}

	[ExposeInWindow] public Texture2D texture = null;
	Texture2D cachedTexture = null;
	public Texture2D _Texture {
		get { 
			if (cachedTexture == null) {
				cachedTexture = (Texture2D)_Material.mainTexture;
			}
			return cachedTexture; 
		}
		set { 
			if (cachedTexture == value) return;
			cachedTexture = value;
			if (cachedTexture != null) {
				_Material = MaterialsManager.CreateMaterialFromTexture (cachedTexture, cachedTexture.format.HasAlpha ());
				gameObject.SetActive (!MaterialsManager.TextureIsBlank (_Texture));
			} else {
				_Material = MaterialsManager.Blank;
			}
		}
	}

	[SerializeField] int frameCount = 1;
	[SerializeField] int frame = 1;
	[SerializeField] float speed = 1f;
	[SerializeField] bool animating = false;
	float xScale;

	void Awake () {
		SetScale ();
		SetOffset ();
		_Texture = texture;
		StartAnimating ();
	}

	public void SetScale () {
		xScale = 1f / (float)frameCount;
		if (_Material != null) _Material.mainTextureScale = new Vector2 (xScale, 1f);		
	}

	public void SetOffset () {
		if (_Material != null) _Material.mainTextureOffset = new Vector2 (xScale * (float)frame, 0f);
	}

	public virtual void Refresh () {
		_Texture = texture;
	}

	public void StartAnimating () {
		if (!animating) animating = true;
		#if UNITY_EDITOR
		if (EditorState.InEditMode) {
			return;
		}
		#endif
		StartCoroutine (CoAnimate ());
	}

	public void StopAnimating () {
		animating = false;
	}

	IEnumerator CoAnimate () {
			
		float position = 0f;
		int prevFrame = 0;

		while (animating) {
			position += Time.deltaTime * speed;
			if (position >= 1f) {
				IterateFrame ();
				position = 0;
			}
			yield return null;
		}
	}

	void IterateFrame () {
		if (frame < frameCount-1) {
			frame ++;
		} else {
			frame = 0;
		}
		SetOffset ();
	}

	#if UNITY_EDITOR
	float _position = 0f;
	public void Animate (float deltaTime) {
		if (!EditorState.InEditMode) return;
		_position += deltaTime * speed;
		if (_position >= 1f) {
			IterateFrame ();
			_position = 0;
		}
	}
	#endif
}
