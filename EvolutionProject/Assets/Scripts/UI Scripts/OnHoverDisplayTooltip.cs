using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnHoverDisplayTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private const float HOVER_TIME_REQUIREMENT = 0.5f;

	public bool   displayTooltipWithoutText;
	public string tooltipID;

	public  TextMeshProUGUI text_TMP;
	private bool            hasText;

	private string currentlyActiveTooltip = "";

	private readonly Dictionary<string, GameObject> tooltipObjects = new Dictionary<string, GameObject>();

	private bool  currentlyOverThis;
	private float hoverStartTime;

	private void Start ()
	{
		if (text_TMP == null) text_TMP = GetComponent<TextMeshProUGUI>();
		hasText = text_TMP != null;
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

		if (!currentlyActiveTooltip.Equals(linkID) && hoverStartTime == 0) hoverStartTime = Time.unscaledTime;

		if (Time.unscaledTime - hoverStartTime > HOVER_TIME_REQUIREMENT) DisplayTooltip(linkID);
	}

	private bool IsLinkBeingHovered (int index)
	{
		return index != -1 ||
			   (!currentlyActiveTooltip.Equals("") && tooltipObjects[currentlyActiveTooltip].GetComponent<OnHoverDisplayTooltip>().IsBeingHovered());
	}

	private void WithoutTextMeshUpdate ()
	{
		if (!IsBeingHovered()) DeactivateActiveTooltip();
		if (Time.unscaledTime - hoverStartTime > HOVER_TIME_REQUIREMENT && IsBeingHovered()) DisplayTooltip(tooltipID);
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
		tooltipObjects[toolID].GetComponent<OnHoverDisplayTooltip>().hoverStartTime = 0;
		DeactivateActiveTooltip();
		currentlyActiveTooltip = toolID;
	}

	private void DeactivateActiveTooltip ()
	{
		if (currentlyActiveTooltip.Equals("")) return;

		hoverStartTime = 0;

		tooltipObjects[currentlyActiveTooltip].transform.parent = transform;
		tooltipObjects[currentlyActiveTooltip].GetComponent<OnHoverDisplayTooltip>().DeactivateActiveTooltip();
		tooltipObjects[currentlyActiveTooltip].SetActive(false);
		currentlyActiveTooltip = "";
	}

	private bool IsBeingHovered ()
	{
		return currentlyOverThis ||
			   (!currentlyActiveTooltip.Equals("") && tooltipObjects[currentlyActiveTooltip].GetComponent<OnHoverDisplayTooltip>().IsBeingHovered());
	}

	public void OnPointerEnter (PointerEventData eventData)
	{
		currentlyOverThis = true;
		if (displayTooltipWithoutText) hoverStartTime = Time.unscaledTime;
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		currentlyOverThis = false;
		if (displayTooltipWithoutText) hoverStartTime = -1;
	}
}