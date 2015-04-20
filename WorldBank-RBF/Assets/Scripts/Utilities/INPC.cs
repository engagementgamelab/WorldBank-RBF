using System.Collections;
using UnityEngine.EventSystems;

public interface INPC : IEventSystemHandler {

	void OnNPCSelected (DataManager.NPC currNpc);

}