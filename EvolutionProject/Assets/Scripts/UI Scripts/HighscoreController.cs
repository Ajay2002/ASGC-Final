using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HighscoreController : MonoBehaviour
{
	public int numOfHighscores;
	
	public Scrollbar     scrollbar;
	public RectTransform container;
	public RectTransform highscoreContainer;
	public RectTransform backButton;

	public Transform highscorePrefab;

	private float scrollAmount;

	private TextMeshProUGUI newHighscoreName;
	private TMP_InputField nameInputField;
	private int newHighscoreIndex;
	private bool nameInputSelected;

	private void Start ()
	{
		for (int i = 0; i < numOfHighscores; i++)
		{
			string playerName = PlayerPrefs.GetString("highscoreString" + i);
			if (playerName.Equals("")) continue;			
			int playerScore = PlayerPrefs.GetInt("highscoreInt" + i);
			
			Transform t = Instantiate(highscorePrefab, highscoreContainer, false);

			if (playerName.Equals("NEWHIGHSCORE"))
			{
				newHighscoreIndex = i;
				
				newHighscoreName = t.GetChild(0).GetComponent<TextMeshProUGUI>();
				newHighscoreName.text = "#" + (i + 1) + " - ";
				nameInputField = t.GetChild(2).GetComponent<TMP_InputField>();
				nameInputField.gameObject.SetActive(true);
			}
			else
			{
				t.GetChild(0).GetComponent<TextMeshProUGUI>().text = "#" + (i + 1) + " - " + playerName;
			}
			
			t.GetChild(1).GetComponent<TextMeshProUGUI>().text = StringifyNum(playerScore);

		}

		highscorePrefab.gameObject.SetActive(false);
		
		LayoutRebuilder.ForceRebuildLayoutImmediate(highscoreContainer);
		
		Vector3 currentPosition = backButton.position;
		
		float highScoreLow = highscoreContainer.position.y - highscoreContainer.rect.height;

		currentPosition.y = Mathf.Min(currentPosition.y, highScoreLow - 10 - backButton.rect.height);

		backButton.position = currentPosition;

		scrollAmount = -currentPosition.y + 20;
	}

	private void Update ()
	{
		Vector2 p = container.anchoredPosition;
		p.y                        = scrollbar.value * scrollAmount;
		container.anchoredPosition = p;

		if (nameInputSelected && !nameInputField.text.Equals("") && Input.GetKeyDown(KeyCode.Return))
		{
			newHighscoreName.text += nameInputField.text;
			PlayerPrefs.SetString("highscoreString" + newHighscoreIndex, nameInputField.text);
			nameInputField.gameObject.SetActive(false);
		}
}

	public void BackToMenu ()
	{
		SceneManager.LoadScene(0);
	}

	public void SetHighscoreNameInputSelected (bool selected)
	{
		nameInputSelected = selected;
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