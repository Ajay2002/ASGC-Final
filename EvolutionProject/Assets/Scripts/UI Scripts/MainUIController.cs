using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour
{
	public static MainUIController Instance;

	public List<GameObject> menuTabButtons;
	public List<GameObject> menuTabObjects;
	
	public readonly List<Tuple<GameObject, GameObject>> menuTabs = new List<Tuple<GameObject, GameObject>>();
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
		
		if (menuTabButtons.Count != menuTabObjects.Count) throw new Exception("menuTabButtons and menuTabObjects are different lengths.");

		for (int i = 0; i < menuTabButtons.Count; i++)
		{
			if (menuTabButtons[i] == null || menuTabObjects[i] == null) throw new Exception("gameobjects in menuTabButtons and menuTabObjects cannot be null.");
			menuTabs.Add(new Tuple<GameObject, GameObject>(menuTabButtons[i], menuTabObjects[i]));
		}
		
		MinimiseAllMenuTabs();
		ChangeSimSpeed(2);
	}

	private void OnMouseDrag ()
	{
		
	}

	public void OpenMenuTab (GameObject tab)
	{
		MinimiseAllMenuTabs();
		tab.SetActive(true);

		foreach ((GameObject menuTabButton, GameObject menuTab) in menuTabs)
		{
			if (menuTab != tab) continue;
			
			menuTabButton.GetComponent<Image>().color = menuTabButton.GetComponent<Button>().colors.pressedColor;
			break;
		}
	}

	public void MinimiseAllMenuTabs ()
	{
		foreach ((GameObject tabButton, GameObject tab) in menuTabs)
		{
			tabButton.GetComponent<Image>().color = tabButton.GetComponent<Button>().colors.normalColor;
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

	public void InstanceAsDragging (int index)
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;

		LayerMask mask = ~(1 << 2); //Collides with all layers except layer 2

		if (!Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) return;

		Vector3 currentMouseWorldPosition = hit.point;
		
		FoodSpawnerScriptableObject toInstanciate = MapManager.Instance.foodSpawnerScriptableObjects[index];

		GameObject fs = Instantiate(toInstanciate.prefab, currentMouseWorldPosition + Vector3.up, quaternion.identity);
		PlayerController.Instance.ClearSelect();
		PlayerController.Instance.AddTransformToSelect(fs.transform);
	}

	private String StringifyNum (int i)
	{
		return $"{i:#,##0.##}";
	}
}