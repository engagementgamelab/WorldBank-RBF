using UnityEngine;
using System.Collections;

[JsonSerializable (typeof (Models.ParallaxLayer))]
public class ParallaxLayer : MB, IEditorPoolable {

	readonly float layerSeparation = 10f;

	[SerializeField, HideInInspector] int index;
	public int Index {
		get { return index; }
		set { index = value; }
	}

	float localSeparation = 0f;
	[WindowExposed, ExposeProperty] public float LocalSeparation {
		get { return localSeparation; }
		set { 
			localSeparation = value; 
			UpdatePosition ();
		}
	}

	public ParallaxImage image;

	float Scale {
		get { return Mathf.Tan (MainCamera.Instance.FOV / 2 * Mathf.Deg2Rad) * Position.z * 2; }
	}

	public void Init () {
		UpdatePosition ();
	}

	void UpdatePosition () {
		SetPosition ();
		SetScale ();
	}

	void SetScale () {
		Transform.localScale = new Vector3 (Scale, Scale, 1);
	}

	void SetPosition () {
		Vector3 target = ScreenPositionHandler.ViewportToWorld (new Vector3 (0, 0.5f, 0));
		target.x -= LocalScale.x / 2;
		target.z = (Index+1) * layerSeparation + LocalSeparation;
		Transform.SetPosition (target);
	}
}
