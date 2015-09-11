using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NpcActionButton : MB {

	Image image = null;
	public Image Icon {
		get {
			if (image == null) {
				image = Content.GetChild (0).GetComponent<Image> ();
			}
			return image;
		}
	}

	FadeText text = null;
	public FadeText Text {
		get {
			if (text == null) {
				text = Content.GetChild (1).GetComponent<FadeText> ();
			}
			return text;
		}
	}

	Button button = null;
	public Button Button {
		get {
			if (button == null) {
				button = GetComponent<Button> ();
			}
			return button;
		}
	}

	Transform content = null;
	Transform Content {
		get {
			if (content == null) {
				content = Transform.GetChild (0);
			}
			return content;
		}
	}

	CanvasGroup canvasGroup = null;
	CanvasGroup CanvasGroup {
		get {
			if (canvasGroup == null) {
				canvasGroup = GetComponent<CanvasGroup> ();
			}
			return canvasGroup;
		}
	}

	Color? defaultColor = null;
	Color? color = null;
	public Color Color {
		get {
			if (color == null) {
				defaultColor = Button.colors.normalColor;
				color = defaultColor;
			}
			return (Color)color;
		}
		set {
			if (defaultColor == null) {
				defaultColor = Button.colors.normalColor;
			}
			color = value;
			var colors = Button.colors;
			colors.normalColor = (Color)color;
			Button.colors = colors;
		}
	}

	public Color DefaultColor {
		get { 
			if (defaultColor == null) {
				defaultColor = Button.colors.normalColor;
			}
			return (Color)defaultColor; 
		}
	}

	public void FadeIn (float time) {
		StartCoroutine (CoFade (0f, 1f, time));
	}

	public void FadeOut (float time) {
		StartCoroutine (CoFade (1f, 0f, time));
	}

	IEnumerator CoFade (float from, float to, float time) {
		
		float eTime = 0f;
	
		while (eTime < time) {
			eTime += Time.deltaTime;
			float progress = Mathf.SmoothStep (from, to, eTime / time);
			CanvasGroup.alpha = progress;			
			yield return null;
		}
	}
}
