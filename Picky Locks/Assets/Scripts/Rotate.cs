using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update
    public float currentRotateSpeed = -0;
    public float rotateSpeedMultiplier = 1f;
    private static float maxRotateSpeed = -1.7f;
    public float randomizeVal;
    public bool randomize;
    // Update is called once per frame

    private void Start()
    {
        randomizeVal = Random.Range(0.95f, 1.25f);
        if (!randomize)
            randomizeVal = 1;
    }
    void FixedUpdate()
    {
        transform.Rotate(0, 0, currentRotateSpeed * rotateSpeedMultiplier * randomizeVal);
    }

    public IEnumerator EaseInSlowDown (float duration, bool easeOut = false, float easeOutDuration = 0)
    {
        float lerpValue = 0;
        currentRotateSpeed = -0.1f;
        while (lerpValue < 1)
        {
            lerpValue += Time.deltaTime / duration;
        }
        currentRotateSpeed = maxRotateSpeed;
        yield return null;
    }
}
