using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public List<Zone> zones;
    public int pinAngleSpacing = 6;
    public int minConnectedPins;
    public int maxConnectedPins;
    public Pin pinPrefab;
    public Pin goldPinPrefab;

    //based on pin spacing. Figure out the angles possible for spawning.
    public Lock lockPrefab;

    public Vector2Int freeAngles;

    //need a prefab cos animator is added
    public Section goldSectionPrefab;
    public Section basicSectionPrefab;
    public ExplosiveLockSection explosiveSectionPrefab;

    public float lockSpacing;

    public Vector2Int anglesBetweenGoldAndExplosiveSection;
    public Vector2Int anglesBetweenBasicAndExplosiveSection;

    public Lock[] locks;
    public int activeLockIndex;
    public ParticleSystem basicParticleEffect;
    public ParticleSystem goldParticleEffect;

    public GameObject crackEffect;

    public EndReward endReward;

    public bool doOnce = true;

    private static LevelGenerator _instance;
    public static LevelGenerator Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<LevelGenerator>();
            return _instance;
        }
    }

    private void Start()
    {
        int zoneLevelCount = PlayerPrefs.GetInt("zoneLevelCount");
        int currentZone = PlayerPrefs.GetInt("zone");
        int totalLevelCount = PlayerPrefs.GetInt("totalLevelCount");


        GenerateLevel(currentZone, zoneLevelCount);
        UIManager.Instance.zoneCountText.text =" Zone " + (currentZone + 1);
        UIManager.Instance.levelCountText.text = "Level " + (totalLevelCount + 1);

        UIManager.Instance.goldBarUI.SetActive(false);
        UIManager.Instance.onDeathUI.SetActive(false);
        UIManager.Instance.onLevelEndUI.SetActive(false);
        UIManager.Instance.onZoneEndUI.SetActive(false);
        UIManager.Instance.tapToOpenUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && doOnce)
        {
            UIManager.Instance.onStartUI.GetComponent<Animator>().Play("StartTitleFadeOut", 0);
            UIManager.Instance.hudCanvas.gameObject.SetActive(true);
            UIManager.Instance.goldBarUI.gameObject.SetActive(true);
            locks[0].ActivateLock(0.3f);
            doOnce = false;

            //Key.isKeyActive = true;
            //locks[0].GetComponentInChildren<Rotate>().currentRotateSpeed = -1.7f;
        }
    }

    public void ConvertToGoldSections()
    {
        for (int i = activeLockIndex; i < 5; i++)
        {
            if (locks[i])
            {
                foreach (Section section in locks[i].basicSections)
                {
                    section.TurnPinsToGold();
                }
            }
        }

        if (locks[activeLockIndex])
            foreach (ExplosiveLockSection explosiveLockSection in locks[activeLockIndex].explosiveSections)
            {
                Key.isInvulnerable = true;
                explosiveLockSection.animator.Play("Deactivate", 1);
            }
    }

    public void ConvertToBasicSections()
    {
        for (int i = activeLockIndex; i < 5; i++)
        {
            if (locks[i])
            {
                foreach (Section section in locks[i].basicSections)
                {
                    section.TurnPinsToBasic();
                }
            }
        }
        if (locks[activeLockIndex])
            foreach (ExplosiveLockSection explosiveLockSection in locks[activeLockIndex].explosiveSections)
            {
                explosiveLockSection.animator.Play("Activate", 1);
                StartCoroutine(DelayedKeyInvulnerability());
            }

    }
    IEnumerator DelayedKeyInvulnerability()
    {
        yield return new WaitForSeconds(0.34f);
        Key.isInvulnerable = false;
    }
    public void GenerateLevel(int zone, int zoneLevel)
    {
        //generate gold section first
        Random.InitState(PlayerPrefs.GetInt("TotalLevelCount"));

        #region Spawning Locks
        locks = new Lock[5];


        //Initializing Empty Locks
        for (int i = 0; i < locks.Length; i++)
        {
            locks[i] = Instantiate(lockPrefab);
            locks[i].transform.parent = transform;
            locks[i].transform.localPosition = new Vector3(0, 0, lockSpacing * i);
            locks[i].gameObject.name = "lock " + i;
            locks[i].lockIndex = i;
        }

        activeLockIndex = 0;
        #endregion

        freeAngles = new Vector2Int(0, 360);

        #region Spawing Pins and Explosive Sections on each Lock
        int goldLockIndex = Random.Range(2, 4);

        Debug.Log("This is the " + zones[0].levels.Count);
        LevelData levelData = zones[zone].levels[zoneLevel];

        for (int i = 0; i < 5; i++)
        {
            LockData lockStructure = levelData.lockStructure[i];

            switch (lockStructure)
            {
                case LockData.NormalLockOne:
                    GenerateNormalLockOne(locks[i]);
                    break;
                case LockData.NormalLockTwo:
                    GenerateNormalLockTwo(locks[i]);
                    break;
                case LockData.NormalLockThree:
                    //GenerateNormalLockThree(locks[i]);
                    break;
                case LockData.RotatingLockOne:
                    GenerateSpecialLockOne(locks[i]);
                    break;
                case LockData.RotatingLockTwo:
                    GenerateSpecialLockTwo(locks[i]);
                    break;
                case LockData.RotatingLockThree:
                    GenerateSpecialLockThree(locks[i]);
                    break;
                case LockData.RotatingLockFour:
                    GenerateSpecialLockFour(locks[i]);
                    break;
            }
        }
        #endregion

        locks[0].ActivateLock();
        endReward.transform.parent = locks[4].transform;
        //endReward.transform.position = endReward.lockAttachPoint.position;
    }

    public void SpawnExplosiveSection(int angle, bool reverse, Lock lockRef)
    {
        ExplosiveLockSection explosiveLockSection = Instantiate(explosiveSectionPrefab);

        int endAngle = reverse ? freeAngles.y - angle : freeAngles.x + angle + explosiveLockSection.angleCovered - 18;
        int startAngle = reverse ? freeAngles.y - angle - explosiveLockSection.angleCovered : freeAngles.x + angle - 18;


        if (reverse)
        {
            freeAngles.y = startAngle - 1;
        }
        else
        {
            freeAngles.x = endAngle + 1;
        }

        int centerAngle = (startAngle + endAngle) / 2;

        explosiveLockSection.transform.rotation = Quaternion.Euler(0, 0, centerAngle);
        explosiveLockSection.transform.parent = lockRef.rotationTransform;
        explosiveLockSection.transform.localPosition = Vector3.zero;
        explosiveLockSection.owningLock = lockRef;
        lockRef.explosiveSections.Add(explosiveLockSection);
    }

    public void SpawnPinSection(int paddingAngle, int numberOfPins, bool goldSection, Lock owningLock) {
        Section section;
        if (goldSection)
        {
            section = Instantiate(goldSectionPrefab);
            section.name = "Gold Section";
        }
        else
            section = Instantiate(basicSectionPrefab);

        int startAngle = freeAngles.x + paddingAngle;
        int endAngle = startAngle + numberOfPins * pinAngleSpacing;
        int centerAngle = (startAngle + endAngle) / 2;

        section.transform.rotation = Quaternion.Euler(0, 0, centerAngle);
        section.owningLock = owningLock;
        section.transform.parent = owningLock.rotationTransform;
        section.transform.localPosition = Vector3.zero;

        freeAngles.x = endAngle + pinAngleSpacing/2;
        
        for (int i = 0; i < numberOfPins; i++)
        {
            int adjustedAngle = i > 259 ? i - 360 : i;
            Pin pin = goldSection ? Instantiate(goldPinPrefab) : Instantiate(pinPrefab);
            pin.transform.parent = section.transform;
            pin.transform.rotation = Quaternion.Euler(0, 0, startAngle + i * pinAngleSpacing);
            pin.transform.localPosition = Vector3.zero;
            section.unfilledPins.Add(pin);
            pin.owningSection = section;
        }

        section.isGoldSection = goldSection;
        if (goldSection)
            owningLock.goldSections.Add(section);
        else
            owningLock.basicSections.Add(section);
    }

    public void OnLockUnlocked(Lock unlockedLock, float delay = 0)
    {
        if (unlockedLock.lockIndex < 4)
        {
            StartCoroutine(AnimateNextLock(unlockedLock.lockIndex + 1, delay));
            StartCoroutine(SpawnCrackEffect(unlockedLock));
        }
        else
        {
            endReward.OnLevelEnd(ScoreController.Instance.score, unlockedLock);
            SceneLoader.Instance.OnLevelEnd();
        }
    }

    public IEnumerator SpawnCrackEffect(Lock lockToCrack)
    {

        yield return new WaitForSeconds(0.3f);
        Destroy(lockToCrack.gameObject);
        Instantiate(crackEffect);
    }

    IEnumerator AnimateNextLock(int lockIndex, float delay)
    {
        Key.isKeyActive = false;
        Vector3 targetPosition = new Vector3(0, 0, -lockSpacing * lockIndex);
        Vector3 startPosition = transform.position;

        yield return new WaitForSeconds(0.8f + delay);
        float lerpValue = 0;
        while (lerpValue <= 1)
        {
            lerpValue += Time.deltaTime * 2f;
            transform.position = Vector3.Lerp(startPosition, targetPosition, lerpValue);
            yield return null;
        }

        Key.isKeyActive = true;
        locks[lockIndex].ActivateLock();
        //if (Key.isInvulnerable && locks[activeLockIndex])
        //    foreach (ExplosiveLockSection explosiveLockSection in locks[activeLockIndex].explosiveSections)
        //        {
        //            explosiveLockSection.animator.Play("Deactivate", 1);
        //            Key.isInvulnerable = false;
        //        }

        activeLockIndex = lockIndex;

        if (Key.isInvulnerable)
        {
            foreach (ExplosiveLockSection newExplosiveSection in locks[activeLockIndex].explosiveSections)
            {
                newExplosiveSection.animator.Play("Deactivate", 1);
            }
            Debug.Log("DEACTIVATED");
        }
    }

    [System.Serializable]
    public struct SectionData
    {
        public MeshFilter prefab;
        public int angleCovered;
    }

    public void SpawnRotatingExplosiveSection(Lock lockRef, int angle = 0, int currentRotateSpeed = 2)
    {
        GameObject redSectionHolder = Instantiate(new GameObject());

        redSectionHolder.transform.parent = lockRef.transform;
        redSectionHolder.transform.localPosition = Vector3.zero;
        redSectionHolder.transform.localRotation = Quaternion.Euler(0, 0, 0);
        redSectionHolder.AddComponent(typeof(Rotate));
        redSectionHolder.GetComponent<Rotate>().currentRotateSpeed = currentRotateSpeed;
        redSectionHolder.GetComponent<Rotate>().randomize = false;

        ExplosiveLockSection explosiveLockSection = Instantiate(explosiveSectionPrefab);
        explosiveLockSection.transform.parent = redSectionHolder.transform;

        explosiveLockSection.owningLock = lockRef;
        explosiveLockSection.transform.parent = explosiveLockSection.transform;
        explosiveLockSection.transform.localPosition = Vector3.zero;
        explosiveLockSection.transform.localRotation = Quaternion.Euler(0, 0, angle);

        lockRef.explosiveSections.Add(explosiveLockSection);
    }

    public void GenerateNormalLockTwo(Lock lockRef)
    {
        int sectionCount = 6;

        for (int j = 0; j < sectionCount; j++)
        {
            if (j % 2 == 0)
            {
                SpawnExplosiveSection(Random.Range(anglesBetweenBasicAndExplosiveSection.x, anglesBetweenBasicAndExplosiveSection.y), false, lockRef);
            }
            else
            {
                SpawnPinSection(Random.Range(anglesBetweenBasicAndExplosiveSection.x, anglesBetweenBasicAndExplosiveSection.y), Random.Range(minConnectedPins, maxConnectedPins), false, lockRef);
            }
        }

        lockRef.rotationTransform.GetComponent<Rotate>().currentRotateSpeed = -9f;
    }

    public void GenerateNormalLockOne(Lock lockRef)
    {
        int sectionCount = 4;

        for (int j = 0; j < sectionCount; j++)
        {
            if (j % 2 == 0)
            {
                SpawnExplosiveSection(Random.Range(anglesBetweenBasicAndExplosiveSection.x, anglesBetweenBasicAndExplosiveSection.y + 20), false, lockRef);
            }
            else
            {
                SpawnPinSection(Random.Range(anglesBetweenBasicAndExplosiveSection.x, anglesBetweenBasicAndExplosiveSection.y + 20), Random.Range(minConnectedPins, maxConnectedPins), false, lockRef);
            }
        }

        lockRef.rotationTransform.GetComponent<Rotate>().currentRotateSpeed = -9f;
    }

    public void GenerateSpecialLockOne(Lock lockRef)
    {
        int sectionCount = 3;

        for (int i = 0; i < sectionCount; i++)
        {
            SpawnPinSection(Random.Range(anglesBetweenBasicAndExplosiveSection.x, anglesBetweenBasicAndExplosiveSection.y), Random.Range(minConnectedPins, maxConnectedPins), false, lockRef);
        }

        lockRef.rotationTransform.GetComponent<Rotate>().currentRotateSpeed = -9f;
        SpawnRotatingExplosiveSection(lockRef);
    }

    public void GenerateSpecialLockTwo(Lock lockRef)
    {
        int sectionCount = 2;

        for (int i = 0; i < sectionCount; i++)
        {
            SpawnPinSection(Random.Range(anglesBetweenBasicAndExplosiveSection.x, anglesBetweenBasicAndExplosiveSection.y), Random.Range(minConnectedPins, maxConnectedPins), false, lockRef);
            lockRef.rotationTransform.GetComponent<Rotate>().currentRotateSpeed = -8f;
        }
        SpawnRotatingExplosiveSection(lockRef, 120);
        SpawnRotatingExplosiveSection(lockRef);
    }

    //three explosive sections, one normal
    public void GenerateSpecialLockThree(Lock lockRef)
    {
        int sectionCount = 1;

        for (int i = 0; i < sectionCount; i++)
        {
            SpawnPinSection(Random.Range(anglesBetweenBasicAndExplosiveSection.x, anglesBetweenBasicAndExplosiveSection.y), Random.Range(minConnectedPins, maxConnectedPins), false, lockRef);
            lockRef.rotationTransform.GetComponent<Rotate>().currentRotateSpeed = -10f;
        }

        SpawnRotatingExplosiveSection(lockRef, 0);
        SpawnRotatingExplosiveSection(lockRef, 60);
        SpawnRotatingExplosiveSection(lockRef, 120);
    }

    //three explosive sections, two normal
    public void GenerateSpecialLockFour(Lock lockRef)
    {
        int sectionCount = 2;

        for (int i = 0; i < sectionCount; i++)
        {
            SpawnPinSection(Random.Range(anglesBetweenBasicAndExplosiveSection.x, anglesBetweenBasicAndExplosiveSection.y), Random.Range(minConnectedPins, maxConnectedPins), false, lockRef);
            lockRef.rotationTransform.GetComponent<Rotate>().currentRotateSpeed = -8f;
        }

        SpawnRotatingExplosiveSection(lockRef, 0);
        SpawnRotatingExplosiveSection(lockRef, 90);
    }


    public void ChangeLockRotateSpeed(float multiplier)
    {
        StartCoroutine(SmoothChangeRotate(multiplier));
    }
    IEnumerator SmoothChangeRotate(float targetMultiplier)
    {
        int activeLockIndex;
        if (locks[this.activeLockIndex])
            activeLockIndex = this.activeLockIndex;
        else
            activeLockIndex = this.activeLockIndex + 1;

        float startMultiplier;
        if (activeLockIndex <= 4)
        {

            startMultiplier = locks[activeLockIndex].rotationTransform.GetComponent<Rotate>().rotateSpeedMultiplier;

            float lerpValue = 0;
            float duration = 0.5f;

            while (lerpValue < 1)
            {
                for (int i = activeLockIndex; i < 5; i++)
                {
                    if (locks[i])
                        locks[i].rotationTransform.GetComponent<Rotate>().rotateSpeedMultiplier = Mathf.Lerp(startMultiplier, targetMultiplier, lerpValue);
                }
                lerpValue += Time.deltaTime / duration;
                yield return null;
            }
        }
    }
}

