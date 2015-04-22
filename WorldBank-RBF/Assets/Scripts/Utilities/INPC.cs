using System.Collections;
using UnityEngine.EventSystems;

public interface INPC : IEventSystemHandler {

	void OnCitySelected (string strCityName);
	void OnNPCSelected (Models.NPC currNpc);

}