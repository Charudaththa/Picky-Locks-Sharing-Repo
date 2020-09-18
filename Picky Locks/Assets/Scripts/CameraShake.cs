using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Start is called before the first frame update
    private static CameraShake _instance;
    public static CameraShake Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<CameraShake>();
            }

            return _instance;
        }
    }
    private void Awake()
    {
    }


    public IEnumerator Shake (float magnitude, float duration)
    {
        float elapsed = 0;
        Vector3 originalPos = transform.localPosition;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;

        }

        transform.localPosition = Vector3.zero;
    }

    public static IEnumerator ShakeAnyObject(float magnitude, float duration, Transform objectToShake)
    {
        float elapsed = 0;
        Vector3 originalPos = objectToShake.localPosition;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            objectToShake.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;

        }

        objectToShake.localPosition = originalPos;
    }
}
