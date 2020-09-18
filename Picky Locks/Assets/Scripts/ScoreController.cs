using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public Text scoreText;
    public int score;

    public Text popupScoreText;
    public bool isFillingSection;

    public GoldBarController goldBarController;

    private static ScoreController _instance; 
    public static ScoreController Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ScoreController>();
            return _instance;
        }
    }

    public int OnSectionComplete(int filledPins, int unfilledPins, bool isGoldSection)
    {
        int scoreIncrement = 0;
        if (unfilledPins == 0)
        {
            scoreIncrement = isGoldSection ? (unfilledPins + 3) * 3 : unfilledPins + 3;
        } else
            scoreIncrement = isGoldSection ? unfilledPins * 3 : unfilledPins;

        score += scoreIncrement;
        scoreText.text = ""+score;
        return scoreIncrement;
    }

    public void OnPinFill(List<Pin> consecutivePins, bool isGoldSection)
    {
        int scoreIncrement = consecutivePins.Count;
        if (consecutivePins.Count == 5)
                scoreIncrement *= 3;
        if (isGoldSection)
                scoreIncrement *= 5;

        popupScoreText.text = "+" + scoreIncrement;

        score += consecutivePins.Count == 5 ? 11 : 1;
        scoreText.text = "" + score;

        goldBarController.OnScoreIncrement(scoreIncrement);
    }

}

public class DeathUIController
{
    public Canvas deathScreen;
    
    public void OnLevelLoss()
    {
        //deathScreen.SetActive();
    }
}

