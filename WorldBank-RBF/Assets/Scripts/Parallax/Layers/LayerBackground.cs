using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerBackground : MB, IEditorPoolable {

	public int Index { get; set; }

	int tileCount = 0;
	int TileCount {
		get { return tileCount; }
		set { 
			tileCount = Mathf.Max (0, value);
			DestroyBackgrounds ();
			CreateBackgrounds ();
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
			for (int i = 0; i < TileCount; i ++) {
				LayerImageSettings settings = imageSettings[i];
				images[i].Index = settings.GetIndex ();
				images[i].Texture = settings.GetTexture2D ();
				images[i].ColliderWidth = settings.GetColliderWidth ();
				images[i].ColliderCenter = settings.GetColliderCenter ();
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
		if (imageCount == 0)
			return;
		for (int i = 0; i < imageCount; i ++) {
			EditorObjectPool.Destroy<LayerImage> (images[i].Transform);
		}
		images.Clear ();
	}

	public void CreateImage () {
		LayerImage image = EditorObjectPool.Create<LayerImage> ();
		image.SetParent (Transform, images.Count);
		image.Texture = null;
		images.Add (image);
	}

	public void RemoveImage () {
		LayerImage removeImage = images[images.Count-1];
		EditorObjectPool.Destroy<LayerImage> (removeImage.Transform);
		images.Remove (removeImage);
	}
}