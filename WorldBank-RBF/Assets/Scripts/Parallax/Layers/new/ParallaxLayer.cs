using UnityEngine;
using System.Collections;

[JsonSerializable (typeof (Models.ParallaxLayer))]
public class ParallaxLayer : MB, IEditorPoolable {

	readonly float layerSeparation = 10f;

	public int Index { get; set; }

	float localSeparation = 0f;
	[WindowExposed, ExposeProperty] public float LocalSeparation {
		get { return localSeparation; }
		set { 
			localSeparation = value; 
			UpdatePosition ();
		}
	}

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
		Vector3 target = ScreenPositionHandler.ViewportToWorld (new Vector3 (
			0, 0.5f, (Index+1) * layerSeparation + LocalSeparation));
		target.x += LocalScale.x / 2;
		Transform.SetPosition (target);
	}
}
