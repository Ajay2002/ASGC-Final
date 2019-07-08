using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnHoverDisplayTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public bool displayTooltipWithoutText;
	public string tooltipID;
	
	public TextMeshProUGUI text_TMP;
	private bool            hasText;

	private string currentlyActiveTooltip = "";

	private readonly Dictionary<string, GameObject> tooltipObjects = new Dictionary<string, GameObject>();

	private bool currentlyOverThis;

	private void Start ()
	{
		if (text_TMP == null) text_TMP = GetComponent<TextMeshProUGUI>();
		hasText  = text_TMP != null;
	}

	private void Update ()
	{
		if (hasText) TextMeshUpdate();
		else if (displayTooltipWithoutText) WithoutTextMeshUpdate();
	}

	private void TextMeshUpdate () //TODO: Make a fully opaque sprite for the background of the tooltip
	{
		int linkIndex = TMP_TextUtilities.FindIntersectingLink(text_TMP, Input.mousePosition, null);

		if (linkIndex == -1)
		{
			if (!IsLinkBeingHovered(linkIndex)) DeactivateActiveTooltip();
			return;
		}

		TMP_LinkInfo linkInfo = text_TMP.textInfo.linkInfo[linkIndex];
		string       linkID   = linkInfo.GetLinkID();

		DisplayTooltip(linkID);
	}
	
	private bool IsLinkBeingHovered (int index)
	{
		return index != -1 || (!currentlyActiveTooltip.Equals("") && tooltipObjects[currentlyActiveTooltip].GetComponent<OnHoverDisplayTooltip>().IsBeingHovered());
	}
	
	private void WithoutTextMeshUpdate ()
	{
		if (!IsBeingHovered()) DeactivateActiveTooltip();
		if (IsBeingHovered()) DisplayTooltip(tooltipID);
	}

	private void DisplayTooltip (string toolID)
	{
		if (currentlyActiveTooltip.Equals(toolID)) return;

		if (!tooltipObjects.ContainsKey(toolID))
		{
			GameObject g = MainUIController.Instance.CreateTooltip(toolID);
			if (g == null) return;
			
			g.transform.parent = transform;
			tooltipObjects.Add(toolID, g);
		}

		tooltipObjects[toolID].SetActive(true);
		tooltipObjects[toolID].transform.parent   = MainUIController.Instance.transform;
		tooltipObjects[toolID].transform.position = Input.mousePosition + new Vector3(-5f, 5f); //TODO: Make sure this does not go outside the screen
		DeactivateActiveTooltip();
		currentlyActiveTooltip = toolID;
	}

	private void DeactivateActiveTooltip ()
	{
		if (currentlyActiveTooltip.Equals("")) return;

		tooltipObjects[currentlyActiveTooltip].transform.parent = transform;
		tooltipObjects[currentlyActiveTooltip].GetComponent<OnHoverDisplayTooltip>().DeactivateActiveTooltip();
		tooltipObjects[currentlyActiveTooltip].SetActive(false);
		currentlyActiveTooltip = "";
	}

	private bool IsBeingHovered ()
	{
		return currentlyOverThis || (!currentlyActiveTooltip.Equals("") && tooltipObjects[currentlyActiveTooltip].GetComponent<OnHoverDisplayTooltip>().IsBeingHovered());
	}

	public void OnPointerEnter (PointerEventData eventData)
	{
		currentlyOverThis = true;
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		Debug.Log(name);
		currentlyOverThis = false;
	}
}