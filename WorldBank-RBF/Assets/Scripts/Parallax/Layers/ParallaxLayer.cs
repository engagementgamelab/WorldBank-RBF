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
	[ExposeInWindow, ExposeProperty] public float LocalSeparation {
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
			if (images == null || images.Count == 0) return 0;
			
			ParallaxImage i = null;
			int index = images.Count-1;
			while (i == null && index > -1) {
				if (images[index].gameObject.activeSelf)
					i = images[index];
				index --;
			}
			return i.Transform.position.x;
		}
	}

	public void Init () {
		UpdateTransform ();
	}

	public void Destroy () {
		EditorObjectPool.Destroy (images);
		EditorObjectPool.Destroy (npcs);
		EditorObjectPool.Destroy (zoomTriggers);
		images.Clear ();
		npcs.Clear ();
		zoomTriggers.Clear ();
		ObjectPool.Destroy<ParallaxLayer> (Transform);
	}

	void UpdateTransform () {
		
		Vector3 target = ScreenPositionHandler.ViewportToWorld (new Vector3 (0, 0.5f, 0));
		target.z = (Index+1) * layerSeparation + LocalSeparation;
		Transform.SetPosition (target);
		Transform.localScale = new Vector3 (Scale, Scale, 1);

		float aspectRatio = MainCamera.Instance.Aspect;
		float offset = 0;

		if (Mathf.Approximately (aspectRatio, 2f)) {
			offset = -LocalScale.x / 2f;
		} else if (aspectRatio > 1.77f) {
			offset = -LocalScale.x / 2.575f;
		} else if (aspectRatio > 1.59f) {
			offset = -LocalScale.x / 3.33f;
		} else if (aspectRatio > 1.49f) {
			offset = -LocalScale.x / 4f;
		} else if (aspectRatio > 1.32f) {
			offset = -LocalScale.x / 6f;
		} else if (aspectRatio > 1.24f) {
			offset = -LocalScale.x / 8f;
		} else {
			offset = 0;
		}

		Transform.SetPositionX (offset);
	}

	/*public void ClearImages () {
		EditorObjectPool.Destroy (images);
		images.Clear ();
	}*/

	public void CreateImages (List<string> texturePaths) {
		for (int i = 0; i < texturePaths.Count; i ++) {
			ParallaxImage image = EditorObjectPool.Create<ParallaxImage> ();
			image.TexturePath = texturePaths[i];
			AddImage (image);
		}
	}

	public void AddImage (ParallaxImage image) {
		images.Add (image);
		image.Parent = Transform;
		image.Transform.Reset ();
		image.XOffset = images.Count-1;
		image.gameObject.layer = gameObject.layer;
		image.Refresh ();
	}

	public void AddNpc (ParallaxNpc npc) {
		npcs.Add (npc);
		npc.Parent = Transform;
		npc.Transform.Reset ();
		npc.gameObject.layer = gameObject.layer;
		npc.Refresh ();
	}

	public void RemoveNpc (ParallaxNpc npc) {
		EditorObjectPool.Destroy<ParallaxNpc> (npc.Transform);
		npcs.Remove (npc);
	}

	public void AddZoomTrigger (ParallaxZoomTrigger zoomTrigger) {
		zoomTriggers.Add (zoomTrigger);
		zoomTrigger.Parent = Transform;
		zoomTrigger.Transform.Reset ();
		zoomTrigger.gameObject.layer = gameObject.layer;
	}

	public void RemoveZoomTrigger (ParallaxZoomTrigger zoomTrigger) {
		EditorObjectPool.Destroy<ParallaxZoomTrigger> (zoomTrigger.Transform);
		zoomTriggers.Remove (zoomTrigger);
	}
}
