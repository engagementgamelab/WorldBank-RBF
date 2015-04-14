using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerBackground : MB, IEditorPoolable {

	public int Index { get; set; }

	int tileCount = 1;
	public int TileCount {
		get { return tileCount; }
		set { 
			tileCount = Mathf.Max (1, value);
			DestroyBackgrounds ();
			CreateBackgrounds ();
		}
	}

	Texture2D texture;
	new public virtual Texture2D Texture {
		get { return texture; }
		set {
			texture = value;
			for (int i = 0; i < images.Count; i ++) {
				images[i].Texture = texture;
			}
		}
	}

	List<LayerImage> images = new List<LayerImage> ();
	List<Texture2D> textures = new List<Texture2D> ();
	public List<Texture2D> Textures {
		get { return textures; }
		set { 
			textures = value;
			TileCount = textures.Count;
			for (int i = 0; i < textures.Count; i ++) {
				images[i].Texture = textures[i];
			}
		}
	}
	
	public void Init () {
		Debug.Log ("init");
		LayerImage image = Transform.GetChildOfType<LayerImage> ();
		if (image != null) {
			image.SetParent (Transform);
			image.Texture = Texture;
			images.Clear ();
			images.Add (image);
		} else {
			CreateImage (0);
		}
	}

	void CreateBackgrounds () {
		for (int i = 0; i < tileCount; i ++) {
			CreateImage (i);
		}
	}

	void DestroyBackgrounds () {
		int imageCount = images.Count;
		if (imageCount == 0) return;
		for (int i = 0; i < imageCount; i ++) {
			EditorObjectPool.Destroy<LayerImage> (images[i].Transform);
		}
		images.Clear ();
	}

	void CreateImage (float xPosition=0) {
		LayerImage image = EditorObjectPool.Create<LayerImage> ();
		image.SetParent (Transform, xPosition);
		image.Texture = Texture;
		images.Add (image);
	}

	void OnEnable () {
		/*Debug.Log ("enable");
		Debug.Log (images.Count);
		if (images.Count == 0) {
			foreach (Transform child in Transform) {
				images.Add (child.GetScript<LayerImage> ());
			}
		}
		Debug.Log (images.Count);
		Debug.Log ("--------");*/
	}
}