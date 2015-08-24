using UnityEngine;
using UnityEngine;

public class AdvisorButton : PortraitTextBox {

	public Animator containerAnimator;
	public Animator animator;
	public AdvisorButtonContainer container;

	void Awake () {
		container.onEndHide += OnEndHide;
	}

	void OnEnable () {
		Button.interactable = true;
	}

	public void Show () {
		containerAnimator.Play ("AdvisorShow");
	}

	public void Hide () {
		containerAnimator.Play ("AdvisorHide");
	}

	public void OnButtonPress () {
		Button.interactable = false;
	}

	void OnEndHide () {
		animator.Play ("AdvisorShrink");
	}

	public void OnEndShrink () {
		ObjectPool.Destroy<AdvisorButton> (transform);
	}
}
