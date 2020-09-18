using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldBarController : MonoBehaviour
{
    public float barFill;
    public float maxBarFill;
    public float barDepleteRate;

    public float goldBarDepleteRate;
    public bool isInGoldState;

    public Slider slider;
    public Sprite emptyBar;
    public Sprite filledBar;
    public Sprite goldenBar;

    public float minSliderValue = 0.063f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnScoreIncrement(1);
        }
        if (isInGoldState)
            barFill -= goldBarDepleteRate * Time.deltaTime;
        else
            barFill -= barDepleteRate * Time.deltaTime;

        if (barFill <= 0 && isInGoldState)
        {
            slider.fillRect.GetComponentInChildren<Image>().sprite = filledBar;
            isInGoldState = false;

            LevelGenerator.Instance.ConvertToBasicSections();
            UIManager.Instance.goldEffectUI.SetActive(true);
            UIManager.Instance.goldEffectUI.GetComponent<Animator>().Play("SplashScreenFadeOut");
            LevelGenerator.Instance.ChangeLockRotateSpeed(1f);
        }
        slider.value = minSliderValue + (barFill / maxBarFill) * (1 - minSliderValue);

        barFill = Mathf.Clamp(barFill, 0, maxBarFill * 1.2f);
    }
    public void OnScoreIncrement(int scoreIncrement)
    {
        //only increment if NOT in gold state
        if (!isInGoldState)
        {
            barFill += scoreIncrement;

            if (barFill >= maxBarFill * 0.98f)
            {
                slider.fillRect.GetComponentInChildren<Image>().sprite = goldenBar;
                isInGoldState = true;

                LevelGenerator.Instance.ConvertToGoldSections();
                UIManager.Instance.goldEffectUI.gameObject.SetActive(true);
                LevelGenerator.Instance.ChangeLockRotateSpeed(1.35f);
            }
        }
    }
}
