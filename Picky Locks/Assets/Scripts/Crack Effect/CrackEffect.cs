using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackEffect : MonoBehaviour
{
    [SerializeField]
    public List<RbAndMagnitude> rigidbodyAndImpulseMagnitudes;

    public IEnumerator StartCracking()
    {
        foreach(RbAndMagnitude rbAndMagnitude in rigidbodyAndImpulseMagnitudes)
        {
            rbAndMagnitude.rb.AddExplosionForce(rbAndMagnitude.magnitude, Vector3.zero, 10);
        }
        yield return new WaitForSeconds(0.05f);
        foreach (RbAndMagnitude rbAndMagnitude in rigidbodyAndImpulseMagnitudes)
        {
            rbAndMagnitude.rb.useGravity = true;
        }
    }

    public void ShakeCrackedLock()
    {
        StartCoroutine(CameraShake.Instance.Shake(0.03f, 0.5f));
        StartCoroutine(CameraShake.ShakeAnyObject(0.03f, 0.5f, transform));
    }

    [System.Serializable]
    public struct RbAndMagnitude
    {
        public Rigidbody rb;
        public float magnitude;
    }
}
