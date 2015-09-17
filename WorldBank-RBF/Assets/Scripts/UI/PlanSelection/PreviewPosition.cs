using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PreviewPosition : MonoBehaviour {

	public List<Image> circles;

	int position = 0;
	public int Position {
		get { return position; }
		set { 
			
			Color transparent = Color.white;
			transparent.a = 0.5f;

			position = value; 
			foreach (Image c in circles) {
				c.color = transparent;
			}

			circles[position].color = Color.white;
		}
	}
}
