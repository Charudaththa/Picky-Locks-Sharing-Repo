using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pin : MonoBehaviour
{
    public Section owningSection;
    public Animator animator;
    public bool isPinActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key") && isPinActive && Key.isKeyActive)
        {
            isPinActive = false;
            owningSection.OnPinFill(this);
        }
    }

    public void ResetPin()
    {
        animator.SetTrigger("Reset");
    }
}