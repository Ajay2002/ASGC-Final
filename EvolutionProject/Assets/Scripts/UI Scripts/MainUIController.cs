using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainUIController : MonoBehaviour
{
	public static MainUIController Instance;

	public enum SimSpeed
	{
		PAUSED,
		SLOW,
		NORMAL,
		FAST
	}

	public GameObject tooltipPrefab;
	
	public List<TooltipScriptableObject> tooltips;

	public List<GameObject> menuTabButtons;
	public List<GameObject> menuTabObjects;

	public readonly List<Tuple<GameObject, GameObject>> menuTabs = new List<Tuple<GameObject, GameObject>>();
	public          List<GameObject>                    simSpeedButtons;

	public TextMeshProUGUI currencyText;
	public TextMeshProUGUI populationText;

	public float slowSimSpeed = 0.5f;
	public float fastSimSpeed = 5;

	private SimSpeed currentSimSpeed;

	public TextMeshProUGUI lettuceSpawnRateText;
	public TextMeshProUGUI mutationChanceText;
	public TextMeshProUGUI mutationChanceIncreaseCostText;

	public GameObject pauseMenu;

	private Transform currentUIDrag;

	private SimSpeed savedSimSpeed;

	private void Start ()
	{
		if (Instance == null) Instance = this;

		if (menuTabButtons.Count != menuTabObjects.Count) throw new Exception("menuTabButtons and menuTabObjects are different lengths.");

		for (int i = 0; i < menuTabButtons.Count; i++)
		{
			if (menuTabButtons[i] == null || menuTabObjects[i] == null) throw new Exception("gameobjects in menuTabButtons and menuTabObjects cannot be null.");
			menuTabs.Add(new Tuple<GameObject, GameObject>(menuTabButtons[i], menuTabObjects[i]));
		}

		TooltipScriptableObject[] objects = Resources.LoadAll<TooltipScriptableObject>("ScriptableObjects/Tooltips");
		tooltips.AddRange(objects);
		
		MinimiseAllMenuTabs();
		ChangeSimSpeed(2);
	}

	private void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) SetPauseMenu(!pauseMenu.activeSelf);
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

		currentSimSpeed = (SimSpeed) s;

		switch (currentSimSpeed)
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
	
	public void ChangeSimSpeed (SimSpeed s)
	{
		ChangeSimSpeed((int) s);
	}

	public void UpdateCurrencyDisplay (int currencyAmount)
	{
		currencyText.text = "<link=\"currency\">" + StringifyNum(currencyAmount) + "</link>";
	}

	public void InstanceAsDragging (int index)
	{
		const int cost = 100000;
		
		FoodSpawnerScriptableObject toInstantiate = null;
		if (index != -1)
		{
			toInstantiate = MapManager.Instance.foodSpawnerScriptableObjects[index];
			if (!CurrencyController.Instance.RemoveCurrency(toInstantiate.cost, true)) return;
		}
		else
		{
			if (!CurrencyController.Instance.RemoveCurrency(cost, true)) return;			
		}
		
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit;

		LayerMask mask = ~(1 << 2); //Collides with all layers except layer 2

		if (!Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) return;

		Vector3 currentMouseWorldPosition = hit.point;

		if (index != -1 && toInstantiate != null)
		{
			GameObject  fs          = Instantiate(toInstantiate.prefab, MapManager.Instance.NearestPointOnMap(currentMouseWorldPosition), quaternion.identity);
			FoodSpawner fsComponent = fs.GetComponent<FoodSpawner>();
			if (fsComponent != null) fsComponent.Initialise(toInstantiate);
		}
		else
		{
			Transform e = MapManager.Instance.SpawnEntity(MapManager.Instance.NearestPointOnMap(currentMouseWorldPosition),null);
			e.GetComponent<EntityManager>().initial = true;
		}
	}

	public void ChangeLettuceSpawnRate (float amount)
	{
		const int cost = 10000;
		
		if (1 / MapManager.Instance.worldSpawnedFood[0].Item1 + amount <= 0) return;
		if (!CurrencyController.Instance.RemoveCurrency(cost, true)) return;
		
		MapManager.Instance.worldSpawnedFood[0] =
			new Tuple<float, FoodScriptableObject>(1 / (1 / MapManager.Instance.worldSpawnedFood[0].Item1 + amount), MapManager.Instance.worldSpawnedFood[0].Item2);

		lettuceSpawnRateText.text = "Lettuce Spawn Rate: " + (1 / MapManager.Instance.worldSpawnedFood[0].Item1);
	}

	public void ChangeMutationChance (float amount)
	{
		const float m = 200000;
		const float c = -3000;
		
		if (MapManager.Instance.mutationChance + amount > 0.9f || MapManager.Instance.mutationChance + amount < 0.02f) return;
		
		int cost = Mathf.RoundToInt(m * MapManager.Instance.mutationChance - c);
		cost = amount > 0 ? cost : 1000;
		
		if (!CurrencyController.Instance.RemoveCurrency(cost, true)) return;

		MapManager.Instance.mutationChance += amount;
		
		mutationChanceText.text = "Mutation Chance: " + Mathf.Round(MapManager.Instance.mutationChance * 1000) / 10 + "%";
		mutationChanceIncreaseCostText.text = "(" + StringifyNum(Mathf.RoundToInt(m * MapManager.Instance.mutationChance - c)) + ")";
	}

	public GameObject CreateTooltip (string tooltipID)
	{
		TooltipScriptableObject tooltip = tooltips.Find(x => x.ID.Equals(tooltipID));

		if (tooltip == null) return null;
		
		GameObject tt = Instantiate(tooltipPrefab);
		tt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = tooltip.heading;
		tt.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = tooltip.text;
		tt.transform.GetChild(2).GetComponent<TextMeshProUGUI>().ForceMeshUpdate();
		tt.transform.GetChild(2).GetComponent<ResizeToFitText>().Resize();		
		return tt;
	}

	public void SetPauseMenu (bool enabled)
	{
		pauseMenu.SetActive(enabled);
		if (enabled)
		{
			MinimiseAllMenuTabs();
			savedSimSpeed = currentSimSpeed;
			ChangeSimSpeed(SimSpeed.PAUSED);
		}
		else ChangeSimSpeed(savedSimSpeed);
	}
	
	public void QuitGame ()
	{
		Application.Quit();
	}
	
	private string StringifyNum (int i)
	{
		return $"{i:#,##0.##}";
	}
	
	private string StringifyNum (float f)
	{
		return $"{f:#,##0.##}";
	}
}