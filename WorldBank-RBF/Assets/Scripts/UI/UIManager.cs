using UnityEngine;

// Just sets the state of the UI when the game begins

public class UIManager : MonoBehaviour {

	public NpcDialogBox dialogBox;
	public UnlockNotification unlockNotification;
	public InteractionsCounter interactions;
	public GameObject mapCanvas;
	public GameObject tacticsScreen;

	void Awake () {
		mapCanvas.SetActive (false);
		tacticsScreen.SetActive (false);
		dialogBox.gameObject.SetActive (true);
		unlockNotification.gameObject.SetActive (true);
		interactions.gameObject.SetActive (true);
	}
}
