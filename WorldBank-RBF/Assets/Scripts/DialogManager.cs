using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour {

	public Button btnPrefab;
	public GameObject panel;

	public void LoadDialogForCity(string city)
	{
		// Data tests
		// Dictionary<string, IEnumerator> itr = DataManager.GetDataForCity(city);

  //       foreach(IEnumerator npcEnum in itr.Values) {

  //       	GenerateNPC(npcEnum);

  //       	while(npcEnum.MoveNext()) {

  //       		Debug.Log(DataManager.GetKVP(npcEnum.Current));
  //       		// Debug.Log(DataManager.GetKVP(npcEnum.Current).Value.GetType());
  //       		// MyButton.onClick.AddListener(() => { MyFunction("string literal"); MyOtherFunction(MyButton.name); });

  //       	}
        // }

	}

	private void GenerateNPC(IEnumerator npcEnum) {

		Button go = (Button)Instantiate(btnPrefab);
	  
	    go.transform.parent = panel.transform;
	    go.transform.localScale = new Vector3(1, 1, 1);

	    Text label = go.transform.FindChild("Text").GetComponent<Text>();

	    Debug.Log(npcEnum);
		
		// if (!npcEnum.TryGetValue("XML_File", out dialogueKey)) {
		// 	    label.text = 
		// }

	    // Button b = go.GetComponent<Button>();
	    // go.onClick.AddListener(() => MyMethod(a));

	}
}
