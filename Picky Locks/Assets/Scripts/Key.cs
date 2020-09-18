using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    // Start is called before the first frame update
    public Coroutine moveCoroutine;
    public Vector3 startPosition;
    public Vector3 insertedPosition;
    public float insertSpeed;
    public GameObject brokenKeyMesh;
    public ParticleSystem particleEffect;
    public GameObject basicSectionEffect;
    public GameObject goldSectionEffect;

    public GameObject textEffect;

    public LevelGenerator generator;
    public float timeToBreak;

    public static bool isKeyActive;
    public static bool isInvulnerable;

    private static Key _instance;
    public static Key Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<Key>();
            return _instance;
        }
    }

    // Update is called once per frame
    private void Start()
    {
        particleEffect.Clear();
        particleEffect.Pause();
        isKeyActive = false;
        isInvulnerable = false;
        UIManager.Instance.onDeathUI.SetActive(false);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isKeyActive)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
                moveCoroutine = StartCoroutine(ToggleKey(true));

            if (generator.locks != null)
                generator.locks[generator.activeLockIndex].transform.Find("MainPinHolder").GetComponentInChildren<Animator>().Play("MainPinPopUp", 0);
        } else if (Input.GetMouseButtonUp(0))
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(ToggleKey(false));

            if (generator.locks != null && generator.locks[generator.activeLockIndex])
                generator.locks[generator.activeLockIndex].transform.Find("MainPinHolder").GetComponentInChildren<Animator>().Play("MainPinHide", 0);
        }
    }

    IEnumerator ToggleKey(bool insert)
    {
        float lerpValue = 0;
        Vector3 targetPosition;

        if (insert)
            targetPosition = insertedPosition;
        else
            targetPosition = startPosition;

        Vector3 startingPosition = transform.position;
        while (lerpValue  <= 1)
        {
            lerpValue += Time.deltaTime * insertSpeed;
            transform.position = Vector3.Lerp(startingPosition, targetPosition, lerpValue);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isKeyActive)
        {
            if (other.CompareTag("Explosive Section") && !isInvulnerable)
            {
                //generator.deactivateAll = true;
                Debug.Log("Hit");
                particleEffect.Simulate(0f, true, true);
                particleEffect.Play();
                //textEffect.SetActive(true);
                if (transform.GetChild(0) != null)
                    transform.GetChild(0).gameObject.SetActive(false);
                GetComponentInChildren<MeshRenderer>().enabled = false;
                StartCoroutine(CameraShake.Instance.Shake(0.1f, 0.5f));
                brokenKeyMesh = Instantiate(brokenKeyMesh);
                brokenKeyMesh.transform.position = transform.position - new Vector3(0, 0, 1.61f);
                brokenKeyMesh.transform.rotation = transform.rotation;

                Lock hitLock = other.GetComponentInParent<Lock>();
                StartCoroutine(hitLock.SlowDownRotate(0, 0.15f, false));
                hitLock.GetComponent<Animator>().Play("LockStopRotatingTexture", 0);
                isKeyActive = false;

                UIManager.Instance.onDeathUI.gameObject.SetActive(true);
                UIManager.Instance.goldBarUI.gameObject.SetActive(false);
                UIManager.Instance.onDeathUI.GetComponentInChildren<Animator>().Play("FadeIn");
            }
            else if (other.CompareTag("Basic Section"))
            {
                basicSectionEffect.SetActive(true);
            }
            else if (other.CompareTag("Gold Section"))
            {
                goldSectionEffect.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Basic Section"))
        {
            basicSectionEffect.SetActive(false);
        } else if (other.CompareTag("Gold Section"))
        {
            goldSectionEffect.SetActive(false);
        }
    }

    public void OnLevelComplete()
    {
        if (!(brokenKeyMesh.gameObject.scene.name == null))
            Destroy(brokenKeyMesh);
        isKeyActive = false;
        gameObject.SetActive(false);
    }
}
