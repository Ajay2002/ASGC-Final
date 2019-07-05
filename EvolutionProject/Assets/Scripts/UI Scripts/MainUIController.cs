using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour
{
	public static MainUIController Instance;

	[FormerlySerializedAs("MenuTabs")]
	public List<GameObject> menuTabs;
	public List<GameObject> simSpeedButtons;

	public TextMeshProUGUI currencyText;
	public TextMeshProUGUI populationText;

	public float slowSimSpeed = 0.5f;
	public float fastSimSpeed = 5;

	public enum SimSpeed
	{
		PAUSED,
		SLOW,
		NORMAL,
		FAST
	}

	private void Start ()
	{
		if (Instance == null) Instance = this;
	}

	public void OpenMenuTab (GameObject tab)
	{
		MinimiseAllMenuTabs();
		tab.SetActive(true);
	}

	public void MinimiseAllMenuTabs ()
	{
		foreach (GameObject tab in menuTabs)
		{
			tab.SetActive(false);
		}
	}

	public void ChangeSimSpeed (int s)
	{
		foreach (GameObject button in simSpeedButtons)
		{
			button.GetComponent<Image>().color = button.GetComponent<Button>().colors.normalColor;
		}
		
		simSpeedButtons[s].GetComponent<Image>().color = simSpeedButtons[s].GetComponent<Button>().colors.pressedColor;
		
		SimSpeed simSpeed = (SimSpeed) s;

		switch (simSpeed)
		{
			case SimSpeed.PAUSED:
				Time.timeScale = 0;
				break;

			case SimSpeed.SLOW:
				Time.timeScale = slowSimSpeed;
				break;

			case SimSpeed.NORMAL:
				Time.timeScale = 1;
				break;

			case SimSpeed.FAST:
				Time.timeScale = fastSimSpeed;
				break;

			default:
				throw new Exception("SimSpeed chosen not valid.");
		}
	}

	public void UpdateCurrencyDisplay (int currencyAmount)
	{
		currencyText.text = StringifyNum(currencyAmount);
	}

	private String StringifyNum (int i)
	{
		return $"{i:#,##0.##}";
	}
}