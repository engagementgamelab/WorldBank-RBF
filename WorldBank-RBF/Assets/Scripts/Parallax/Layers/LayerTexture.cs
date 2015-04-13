using UnityEngine;
using System.Collections;

// Handles positioning and scaling of a QuadImage on a DepthLayer (must be a child of DepthLayer)
// get rid of this shit !!!
public class LayerTexture : LayerComponent {

	public Texture2D texture;
	public virtual Texture2D Texture {
		get { return texture; }
		set {
			texture = value;
			/*if (quadImage != null) {
				quadImage.Material = Material;
			}*/
		}
	}

	Material material = null;
	protected Material Material {
		get {
			#if UNITY_EDITOR
			if (texture == null) {
				Debug.LogWarning (string.Format ("{0} is missing a texture reference", name));
				return null;
			}
			#endif
			material = MaterialsManager.CreateMaterialFromTexture (texture, texture.format.HasAlpha ());
			return material;
		}
	}

	QuadImage quadImage;
	float xPosition;
	bool colliderEnabled;

	/*protected QuadImage CreateImage (float xPosition, bool colliderEnabled=false) {
		//quadImage = EditorObjectPool.Create<QuadImage> ();
		quadImage = ObjectPool.Instantiate<QuadImage> ();
		return RefreshImage ();
	}

	protected QuadImage RefreshImage () {
		
		quadImage.Transform.SetParent (Transform);
		quadImage.Init (Material, colliderEnabled);

		float scale = (float)Material.mainTexture.height / 512f;
		float bottom = -0.5f + scale / 2;
		quadImage.Transform.SetLocalScale (new Vector3 (scale, scale, 1));
		quadImage.Transform.SetLocalPosition (new Vector3 (xPosition, bottom, 0));

		return quadImage;
	}*/
}
