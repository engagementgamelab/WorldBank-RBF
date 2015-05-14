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
			UpdateTransform ();
		}
	}

	public ParallaxImage image;

	float Scale {
		get { return Mathf.Tan (MainCamera.Instance.FOV / 2 * Mathf.Deg2Rad) * Position.z * 2; }
	}

	public void Init () {
		UpdateTransform ();
	}

	void UpdateTransform () {
		Vector3 target = ScreenPositionHandler.ViewportToWorld (new Vector3 (0, 0.5f, 0));
		target.z = (Index+1) * layerSeparation + LocalSeparation;
		Transform.SetPosition (target);
		Transform.localScale = new Vector3 (Scale, Scale, 1);
		Transform.SetPositionX (-LocalScale.x / 2);
	}
}
