using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class MouseInputUIBlocker : MonoBehaviour
{
	public static bool         BlockedByUI;
	private       EventTrigger eventTrigger;

	private void Start ()
	{
		eventTrigger = GetComponent<EventTrigger>();
		if (eventTrigger == null) return;

		// Pointer Enter
		EventTrigger.Entry enterUIEntry = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
		enterUIEntry.callback.AddListener(eventData => { EnterUI();});
		eventTrigger.triggers.Add(enterUIEntry);

		//Pointer Exit
		EventTrigger.Entry exitUIEntry = new EventTrigger.Entry {eventID = EventTriggerType.PointerExit};
		exitUIEntry.callback.AddListener(eventData => { ExitUI(); });
		eventTrigger.triggers.Add(exitUIEntry);
	}

	private void EnterUI ()
	{
		BlockedByUI = true;
	}

	private void ExitUI ()
	{
		BlockedByUI = false;
	}
}