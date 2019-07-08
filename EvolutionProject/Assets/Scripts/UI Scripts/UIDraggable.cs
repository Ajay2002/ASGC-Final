using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public int objectToInstantiate;

	public List<GameObject> objectsToDisableOnDrag;

	private Transform properParent;
	private int siblingIndex;

	private Transform blankSpot;

	private Image image;

	private void Start ()
	{
		siblingIndex = transform.GetSiblingIndex();
		properParent = transform.parent;
		image = GetComponent<Image>();
		blankSpot = properParent.GetChild(transform.parent.childCount - 1);
	}

	public void OnBeginDrag (PointerEventData eventData)
	{		
		transform.parent = transform.parent.parent;
		blankSpot.SetSiblingIndex(siblingIndex);
		image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);

		foreach (GameObject o in objectsToDisableOnDrag)
		{
			o.SetActive(false);
		}
	}

	public void OnDrag (PointerEventData eventData)
	{
		transform.position = eventData.position;
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		MainUIController.Instance.InstanceAsDragging(objectToInstantiate);
		transform.parent = properParent;
		blankSpot.SetSiblingIndex(properParent.childCount - 1);
		transform.SetSiblingIndex(siblingIndex);
		image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
		
		foreach (GameObject o in objectsToDisableOnDrag)
		{
			o.SetActive(true);
		}
	}
}