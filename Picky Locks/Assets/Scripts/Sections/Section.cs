using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour
{
    public Lock owningLock;
    public bool isActive;

    //new
    public List<Pin> unfilledPins;
    public List<Pin> filledPins;
    public List<Pin> consecutivePins;

    public ScoreController scoreController;
    public Animator popupScoreCanvas;
    public Coroutine rotateFloatingScore;
    public bool isFilling = false;

    public bool isGoldSection;
    public int angleCovered;
    public void OnPinFill(Pin pin)
    {
        unfilledPins.Remove(pin);

        filledPins.Add(pin);

        consecutivePins.Add(pin);

        if (consecutivePins.Count == 3)
        {
            consecutivePins[1].animator.Play("GoodGlowAnimation", 1);
            popupScoreCanvas.Play("GoodScore", 0, 0f);
            StartCoroutine(CameraShake.Instance.Shake(0.015f, 0.2f));

        }

        if (consecutivePins.Count == 5)
        {
            consecutivePins[2].animator.Play("PerfectGlowAnimation", 1);
            popupScoreCanvas.Play("PerfectScore", 0, 0f);
        }
        else
        {
            popupScoreCanvas.Play("BasicScoreFlash", 0, 0f);
            StartCoroutine(CameraShake.Instance.Shake(0.007f, 0.1f));
        }


        scoreController.OnPinFill(consecutivePins, isGoldSection);

        if (isGoldSection)
            pin.GetComponent<Animator>().SetTrigger("Gold Pop Up");
        else
            pin.GetComponent<Animator>().SetTrigger("Pop Up");

        if (unfilledPins.Count == 0)
        {
            owningLock.OnSectionComplete(this);
            StartCoroutine(RemoveAllPinsFromSection(0.05f));
            StartCoroutine(CameraShake.Instance.Shake(0.03f, 0.48f));
        }

        if (consecutivePins.Count == 1)
        {
            isFilling = true;
            //rotateFloatingScore = StartCoroutine(RotateFloatingScore());
        }
    }
    public void TurnPinsToGold()
    {
        isGoldSection = true;

        if (isActive)
        {
            foreach (Pin pin in unfilledPins)
            {
                pin.animator.Play("FadeToGoldUnfilled", 0);
            }

            foreach (Pin pin in filledPins)
            {
                pin.animator.Play("FadeToGoldFilled", 0);
            }
        }
    }

    public void TurnPinsToBasic()
    {
        isGoldSection = false;

        if (isActive)
        {
            foreach (Pin pin in unfilledPins)
            {
                pin.animator.Play("FadeToBasicUnfilled");
            }
            foreach (Pin pin in filledPins)
            {
                pin.animator.Play("FadeToBasicFilled");
            }
        }
    }

    public IEnumerator RemoveAllPinsFromSection(float interval, float waitForSeconds = 0.13f)
    {
        yield return new WaitForSeconds(waitForSeconds);


        foreach (Pin pin in filledPins)
        {
            if (isGoldSection)
                pin.animator.Play("GoldSectionCompletePopOut", 0, 0f);
            else
                pin.animator.Play("SectionCompletePopOut", 0, 0f);

            CameraShake.Instance.Shake(0.02f, interval);
            yield return new WaitForSeconds(interval);
        }

    }
    private IEnumerator RotateFloatingScore()
    {
       while (isFilling)
        {
            Quaternion scoreDisplayRotation;
            if (consecutivePins.Count > 0)
            {
                if (consecutivePins.Count % 2 == 0)
                    scoreDisplayRotation = Quaternion.Lerp(consecutivePins[consecutivePins.Count / 2].transform.rotation, consecutivePins[consecutivePins.Count / 2 + 1].transform.rotation, 0.5f);
                else
                    scoreDisplayRotation = consecutivePins[consecutivePins.Count / 2 + 1].transform.rotation;

                popupScoreCanvas.transform.parent.rotation = scoreDisplayRotation;
            }
            yield return null;
        }
    }

    public void ActivateSection()
    {
        isActive = true;
    }
    public void DeactivateSection()
    {
        isActive = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            consecutivePins = new List<Pin>();
            isFilling = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            consecutivePins = new List<Pin>();
        }
    }

    private void Start()
    {
        filledPins = new List<Pin>();
        consecutivePins = new List<Pin>();
        isFilling = false;
    }
}

