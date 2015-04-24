using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayerBackground : MB, IEditorPoolable {

	public int Index { get; set; }

	public int Layer {
		get { return gameObject.layer; }
		set {
			gameObject.layer = value;
			foreach (LayerImage image in Images) {
				image.Layer = value;
			}
		}
	}

	int tileCount = 0;
	int TileCount {
		get { return tileCount; }
		set { 
			tileCount = Mathf.Max (0, value);
			DestroyBackgrounds ();
			CreateBackgrounds ();
		}
	}

	[SerializeField, HideInInspector] List<LayerImage> images;
	public List<LayerImage> Images {
		get { 
			if (images == null) {
				images = new List<LayerImage> ();
			}
			return images; 
		}
		set { images = value; }
	}

	#if UNITY_EDITOR
	List<LayerImageSettings> imageSettings = new List<LayerImageSettings> ();
	public List<LayerImageSettings> ImageSettings {
		get { return imageSettings; }
		set {
			if (value == null) return;
			imageSettings = value;
			TileCount = imageSettings.Count;
			for (int i = 0; i < TileCount; i ++) {
				LayerImageSettings settings = imageSettings[i];
				Images[i].Index = settings.index;
				Images[i].NPCSymbol = settings.npc_symbol;
				Images[i].Texture = settings.GetTexture ();
				Images[i].ColliderEnabled = settings.collider_enabled;
				Images[i].ColliderWidth = settings.collider_width;
				Images[i].ColliderHeight = settings.collider_height;
				Images[i].ColliderCenterX = settings.collider_center_x;
				Images[i].ColliderCenterY = settings.collider_center_y;
			}
		}
	}
	#endif

	public void Init () {}

	void CreateBackgrounds () {
		for (int i = 0; i < tileCount; i ++) {
			CreateImage ();
		}
	}

	void DestroyBackgrounds () {
		int imageCount = Images.Count;
		if (imageCount == 0)
			return;
		for (int i = 0; i < imageCount; i ++) {
			EditorObjectPool.Destroy<LayerImage> (Images[i].Transform);
		}
		Images.Clear ();
	}

	public void CreateImage () {
		LayerImage image = EditorObjectPool.Create<LayerImage> ();
		image.SetParent (Transform, Images.Count);
		image.Texture = null;
		image.Layer = gameObject.layer;
		Images.Add (image);
	}

	public void RemoveImage () {
		LayerImage removeImage = Images[Images.Count-1];
		EditorObjectPool.Destroy<LayerImage> (removeImage.Transform);
		Images.Remove (removeImage);
	}
}