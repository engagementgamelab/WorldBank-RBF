using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	public List<ParallaxImage> images;
	public List<ParallaxNpc> npcs;
	public List<ParallaxZoomTrigger> zoomTriggers;

	float Scale {
		get { return Mathf.Tan (MainCamera.Instance.FOV / 2 * Mathf.Deg2Rad) * Position.z * 2; }
	}

	public float RightMax {
		get { 
			Transform rightImage = images[images.Count-1].Transform;
			return rightImage.position.x - rightImage.localScale.x * 0.5f;
		}
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

	#if UNITY_EDITOR
	public void ClearImages () {
		EditorObjectPool.Destroy (images);
		images.Clear ();
	}

	public void AddImage (ParallaxImage image) {
		images.Add (image);
		image.Parent = Transform;
		image.Transform.Reset ();
		image.Transform.SetLocalPositionX (images.Count-1);
		image.LayerPosition = (int)Position.z;
	}

	public void AddNpc (ParallaxNpc npc) {
		npcs.Add (npc);
		npc.Parent = Transform;
		npc.Transform.Reset ();
		npc.LayerPosition = (int)Position.z;
	}

	public void RemoveNpc (ParallaxNpc npc) {
		EditorObjectPool.Destroy<ParallaxNpc> (npc.Transform);
		npcs.Remove (npc);
	}

	public void AddZoomTrigger (ParallaxZoomTrigger zoomTrigger) {
		zoomTriggers.Add (zoomTrigger);
		zoomTrigger.Parent = Transform;
		zoomTrigger.Transform.Reset ();
		zoomTrigger.LayerPosition = (int)Position.z;
	}

	public void RemoveZoomTrigger (ParallaxZoomTrigger zoomTrigger) {
		EditorObjectPool.Destroy<ParallaxZoomTrigger> (zoomTrigger.Transform);
		zoomTriggers.Remove (zoomTrigger);
	}
	#endif
}
