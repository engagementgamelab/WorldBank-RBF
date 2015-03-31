using UnityEngine;
using System.Collections;

// Handles positioning and scaling of QuadImages on a DepthLayer
// (Must be a child of DepthLayer)
public class LayerTexture : LayerComponent {

	public Texture2D texture;

	Material material = null;
	protected Material Material {
		get {
			if (material == null) {
				#if UNITY_EDITOR
				if (texture == null) {
					Debug.LogError (string.Format ("{0} is missing a texture reference", name));
				}
				#endif
				material = MaterialsManager.CreateMaterialFromTexture (texture, texture.format.HasAlpha ());
			}
			return material;
		}
	}

	protected QuadImage CreateImage (float xPosition, bool colliderEnabled=false) {
		QuadImage quadImage = ObjectManager.Instance.Create<QuadImage> ();
		quadImage.Transform.SetParent (Transform);
		quadImage.Init (Material, colliderEnabled);

		float scale = (float)Material.mainTexture.height / 512f;
		float bottom = -0.5f + scale / 2;
		quadImage.Transform.SetLocalScale (new Vector3 (scale, scale, 1));
		quadImage.Transform.SetLocalPosition (new Vector3 (xPosition, bottom, 0));

		return quadImage;
	}
}
