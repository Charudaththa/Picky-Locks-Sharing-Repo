using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndReward : MonoBehaviour
{
    public Transform endLevelCameraTransform;

    public Transform cameraHolder;
    public GameObject nextZoneKeyUnlock;

    public Animator chestAnimator;

    public Transform giftAttachPoint;
    public Transform lockAttachPoint;
    public bool canOpenChest = false;

    public bool isZoneEnd;
    private static EndReward _instance;
    public static EndReward Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<EndReward>();
            return _instance;
        }
    }

    public void OnLevelEnd(float score, Lock finalLock)
    {
        canOpenChest = true;
        StartCoroutine(LerpCamera(0.5f, finalLock));
        transform.parent = null;
        finalLock.transform.parent = transform;
    }

    private void Update()
    {
        if (canOpenChest && Input.GetMouseButtonDown(0))
        {
            OpenChest();
        }
    }

    public void SpawnReward(bool isZoneEnd)
    {
        this.isZoneEnd = isZoneEnd;
        //TODO 
        if (isZoneEnd)
        {
            giftAttachPoint.transform.gameObject.SetActive(true);

        }
        else
        {
            giftAttachPoint.transform.gameObject.SetActive(false);
        }
    }
    public IEnumerator LerpCamera(float duration, Lock finalLock)
    {
        yield return new WaitForSeconds(0.4f);
        float time = 0f;
        Quaternion startRotation = cameraHolder.rotation;
        Vector3 startPosition = cameraHolder.position;
        Camera camera = cameraHolder.GetComponentInChildren<Camera>();
        while(time < 1)
        {
            time += Time.deltaTime / duration;
            cameraHolder.transform.position = Vector3.Lerp(startPosition, endLevelCameraTransform.localPosition, time);
            cameraHolder.transform.rotation = Quaternion.Lerp(startRotation, endLevelCameraTransform.localRotation, time);
            camera.orthographicSize = Mathf.Lerp(3, 11f, time);
            yield return null;
        }

        cameraHolder.transform.position = endLevelCameraTransform.localPosition;
        finalLock.transform.parent = lockAttachPoint;
        yield return new WaitForSeconds(0.3f);
        canOpenChest = true;
        UIManager.Instance.tapToOpenUI.SetActive(true);
        UIManager.Instance.onLevelEndUI.SetActive(false);
        UIManager.Instance.onZoneEndUI.SetActive(false);
    }
    
    public void OpenChest()
    {
        chestAnimator.Play("OpenChest");
        Key.Instance.OnLevelComplete();

        UIManager.Instance.tapToOpenUI.gameObject.SetActive(false);

        //TODO 
        if (isZoneEnd)
        {
            UIManager.Instance.onZoneEndUI.SetActive(true);
        }
        else
        {
            UIManager.Instance.onLevelEndUI.SetActive(true);
            UIManager.Instance.onLevelEndUI.GetComponentInChildren<Animator>().Play("FadeIn");
        }
    }
}