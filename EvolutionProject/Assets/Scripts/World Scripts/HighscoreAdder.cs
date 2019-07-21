using UnityEngine;

public class HighscoreAdder : MonoBehaviour
{
	private const int numOfHighscores = 10;
	
	public static HighscoreAdder Instance;

	private void Start ()
	{
		if (Instance == null) Instance = this;
	}
	
	public void AddHighscore (int newScore)
	{
		for (int i = numOfHighscores - 1; i >= 0; i--)
		{
			int prevScore = PlayerPrefs.GetInt("highscoreInt" + i);
			if (prevScore > newScore)
			{
				if (i == numOfHighscores - 1) break;
				
				PlayerPrefs.SetString("highscoreString" + (i + 1), "NEWHIGHSCORE");
				PlayerPrefs.SetInt("highscoreInt" + (i + 1), newScore);
			}

			if (i == 0 && prevScore < newScore)
			{
				PlayerPrefs.SetString("highscoreString0", "NEWHIGHSCORE");
				PlayerPrefs.SetInt("highscoreInt0", newScore);
			}
		}
	}
}