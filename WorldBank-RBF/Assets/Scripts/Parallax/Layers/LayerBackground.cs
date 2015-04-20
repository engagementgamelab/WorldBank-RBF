using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerBackground : MB, IEditorPoolable {

	public int Index { get; set; }

	int tileCount = 0;
	public int TileCount {
		get { return tileCount; }
		set { 
			tileCount = Mathf.Max (0, value);
			DestroyBackgrounds ();
			CreateBackgrounds ();
		}
	}

	Texture2D texture;
	public virtual Texture2D Texture {
		get { return texture; }
		set {
			texture = value;
			for (int i = 0; i < images.Count; i ++) {
				images[i].Texture = texture;
			}
		}
	}

	[SerializeField, HideInInspector] List<LayerImage> images = new List<LayerImage> ();
	public List<LayerImage> Images {
		get { return images; }
		set { images = value; }
	}

	List<LayerImageSettings> imageSettings = new List<LayerImageSettings> ();
	public List<LayerImageSettings> ImageSettings {
		get { return imageSettings; }
		set {
			if (value == null) return;
			imageSettings = value;
			TileCount = imageSettings.Count;
			Debug.Log (TileCount + "...." + gameObject.GetInstanceID ());
			for (int i = 0; i < TileCount; i ++) {
				images[i].Texture = imageSettings[i].GetTexture2D ();
				images[i].ColliderWidth = imageSettings[i].GetColliderWidth ();
				images[i].ColliderCenter = imageSettings[i].GetColliderCenter ();
			}
		}
	}

	public void Init () {
		/*LayerImage image = Transform.GetChildOfType<LayerImage> ();
		if (image != null) {
			image.SetParent (Transform);
			image.Texture = Texture;
			images.Clear ();
			images.Add (image);
		} else {
			CreateImage ();
		}*/
	}

	void CreateBackgrounds () {
		for (int i = 0; i < tileCount; i ++) {
			CreateImage ();
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

	public void CreateImage () {
		LayerImage image = EditorObjectPool.Create<LayerImage> ();
		image.SetParent (Transform, images.Count);
		image.Texture = Texture;
		images.Add (image);
	}
}