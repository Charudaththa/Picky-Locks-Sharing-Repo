using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    public List<Section> basicSections;
    public List<Animator> basicSectionsAnimators;
    public List<Section> goldSections;
    public List<Animator> goldSectionsAnimators;

    List<Section> completedGoldSections;
    List<Section> completedBasicSections;

    public Lock connectedGoldLock;

    public List<ExplosiveLockSection> explosiveSections;
    public Quaternion startRotation;
    public Quaternion endRotation;
    public float RotateSpeed = 1;

    public Texture2D freeAnglesTexture;
    public LevelGenerator levelGenerator;
    public int lockIndex;

    public bool isLockActive;

    public float testOffset;

    public GameObject crackEffect;
    public Transform rotationTransform;

    public GameObject sectionCompleteEffect;

    public ScoreController scoreController;
    //0 - 360
    private void Start()
    {
        completedGoldSections = new List<Section>();
        completedBasicSections = new List<Section>();
        scoreController = FindObjectOfType<ScoreController>();
        levelGenerator = FindObjectOfType<LevelGenerator>();
    }


    public IEnumerator DestroyAnimation()
    {
        yield return null;
    }

    public static float PositionToAngle(Vector2 position)
    {
        float angle = 90 - Mathf.Abs(Mathf.Tan(position.y / position.x)) * Mathf.Rad2Deg;
        if (position.x > 0 && position.y < 0)
            angle += 90;
        else if (position.x < 0 && position.y < 0)
            angle += 180;
        else if (position.x < 0 && position.y > 0)
            angle += 270;
        return angle;
    }

    public float GetAngle()
    {
        return rotationTransform.rotation.eulerAngles.z;
    }

    public void OnSectionComplete(Section lockSection)
    {
        basicSections.Remove(lockSection);
        completedBasicSections.Add(lockSection);

        if (basicSections.Count == 0)
        {
            foreach (ExplosiveLockSection explosiveLockSection in explosiveSections)
            {
                explosiveLockSection.animator.SetTrigger("OnLockComplete");
                explosiveLockSection.animator.Play("Activate", 1);
            }
            levelGenerator.OnLockUnlocked(this);
        }
    }

    public IEnumerator SlowDownRotate(float startDelay,float duration, bool pingPong = true)
    {
        float lerpValue = 0;
        Rotate rotator = rotationTransform.GetComponent<Rotate>();
        float startRotateSpeed = -1.7f;
        float endRotateSpeed = 0f;

        yield return new WaitForSeconds(startDelay); 
        while (lerpValue < 1)
        {
            lerpValue += Time.deltaTime / duration;
            rotator.currentRotateSpeed = Mathf.Lerp(startRotateSpeed, endRotateSpeed, lerpValue);
            yield return null;
        }

        while (pingPong && lerpValue >= 0)
        {
            lerpValue -= Time.deltaTime / duration;
            rotator.currentRotateSpeed = Mathf.Lerp(startRotateSpeed, endRotateSpeed, lerpValue);
            yield return null;
        }
    }

    public void ActivateLock(float delayRecieveInput = 0, bool playFadeIn = true)
    {
        rotationTransform.GetComponent<Rotate>().currentRotateSpeed = -1.7f;

        if (playFadeIn)
        {
            foreach (Section goldLockSection in goldSections)
            {
                foreach (Pin pin in goldLockSection.unfilledPins)
                {
                    
                    pin.animator.Play("InitializePinFadeIn", 0);
                }
            }
            foreach (Section basicLockSection in basicSections)
            {

                    foreach (Pin pin in basicLockSection.unfilledPins)
                    {
                        if (basicLockSection.isGoldSection)
                            pin.animator.Play("InitializeGoldPin", 0);
                        else
                            pin.animator.Play("InitializePinFadeIn", 0);
                }
            }

            foreach (ExplosiveLockSection explosiveLockSection in explosiveSections)
            {
                explosiveLockSection.animator.Play("ExplosiveSectionFadeIn");
            }
        }
        StartCoroutine(RecieveInput(delayRecieveInput));
    }

    public IEnumerator RecieveInput(float delayRecieveInput)
    {
        yield return new WaitForSeconds(delayRecieveInput);

        foreach (Section goldLockSection in goldSections)
        {
            goldLockSection.ActivateSection();
        }
        foreach (Section basicLockSection in basicSections)
        {
            basicLockSection.ActivateSection();
        }
        isLockActive = true;
        Key.isKeyActive = true;
    }
}

