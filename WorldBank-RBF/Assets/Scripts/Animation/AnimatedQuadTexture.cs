using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class AnimatedQuadTexture : MB {

	public Material _Material {
		get { return _MeshRenderer.sharedMaterial; }
		set {
			_MeshRenderer.sharedMaterial = value;
			if (value != null) UpdateSortingLayer ();
		}
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
	public Texture2D _Texture {
		set {
			#if UNITY_EDITOR
			TexturePath = MaterialsManager.GetTexturePath (value);
			#endif
		}
	}

	string texturePath = "";
	public string TexturePath {
		get { return texturePath; }
		set {
			texturePath = value;
			_Material = MaterialsManager.GetMaterialAtPath (texturePath, this);
			if (_Material == null) {
				gameObject.SetActive (false);
			} else {
				texture = (Texture2D)_Material.mainTexture;
				gameObject.SetActive (!MaterialsManager.TextureIsBlank (texture));
			}
		}
	}

	[SerializeField] int frameCount = 1;
	[SerializeField] int frame = 1;
	[Range (0, 10f), SerializeField] float speed = 1f;
	[SerializeField] bool animating = false;
	[SerializeField] bool useInterval = false;
	[SerializeField] float intervalMin = 0f;
	[SerializeField] float intervalMax = 0f;
	float xScale;
	float pauseTime;

	protected virtual void Awake () {
		SetScale ();
		SetOffset ();
		if (texture != null) _Texture = texture;
		animating = false;
	}

	public void SetScale () {
		xScale = 1f / (float)frameCount;
		if (_Material != null) _Material.mainTextureScale = new Vector2 (xScale, 1f);
	}

	public void SetOffset () {
		if (_Material == null) return;
		if (frameCount == 1) {
			_Material.mainTextureOffset = Vector2.zero;
		} else {
			_Material.mainTextureOffset = new Vector2 (xScale * (float)frame, 0f);
		}
	}

	public virtual void Refresh () {
		if (texture != null) _Texture = texture;
		UpdateSortingLayer ();
	}

	public void StartAnimating () {
		if (animating) return;
		animating = true;
		if (useInterval) {
			UpdatePauseTime ();
			#if PREVIEW_ANIMATIONS && UNITY_EDITOR
			if (EditorState.InEditMode) return;
			#endif
			StartCoroutine (CoPause ());
		} else {
			#if PREVIEW_ANIMATIONS && UNITY_EDITOR
			if (EditorState.InEditMode) return;
			#endif
			StartCoroutine (CoAnimate ());
		}
	}

	public void StopAnimating () {
		animating = false;
	}

	IEnumerator CoPause () {

		float time = pauseTime;
		float eTime = 0f;

		while (eTime < time) {
			eTime += Time.deltaTime;
			yield return null;
		}

		StartCoroutine (CoAnimate ());
	}

	IEnumerator CoAnimate () {

		float position = 0f;

		while (animating) {
			position += Time.deltaTime * speed;
			if (position >= 1f) {
				animating = !IterateFrame ();
				position = 0;
			}
			yield return null;
		}

		StartAnimating ();
	}

	bool IterateFrame () {
		if (frame < frameCount-1) {
			frame ++;
		} else {
			frame = 0;
		}
		SetOffset ();

		// Returns true if first frame was reached
		return frame == 0;
	}

	void UpdatePauseTime () {
		pauseTime = Random.Range (intervalMin, intervalMax);
	}

	protected void UpdateSortingLayer () {
		int sortingLayer = 10000 - (int)(Position.z * 100);
		if (_Material != null) {
			_Material.renderQueue = sortingLayer;
		} else {
			StartCoroutine (SetSortingLayerOnLoad ());
		}
	}

	IEnumerator SetSortingLayerOnLoad () {
		while (_Material == null) {
			yield return null;
		}
		UpdateSortingLayer ();
	}

	#if PREVIEW_ANIMATIONS && UNITY_EDITOR
	float _position = 0f;
	float _pausePosition = 0f;
	bool _pauseComplete = false;
	public void Animate (float deltaTime) {
		if (!EditorState.InEditMode) return;
		if (useInterval) {
			if (!_pauseComplete) {
				_pausePosition += deltaTime;
			}
			if (_pausePosition >= pauseTime) {
				UpdatePauseTime ();
				_pauseComplete = true;
				_pausePosition = 0f;
			}
		}
		if (!useInterval || _pauseComplete) {
			_position += deltaTime * speed;
			if (_position >= 1f) {
				_pauseComplete = !IterateFrame ();
				_position = 0;
			}
		}
	}
	#endif
}
