using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ExplosiveLockSection : MonoBehaviour
{
    public Lock owningLock;
    public int angleCovered;
    public Vector2Int angleRange;
    public Animator animator;

    public Material transparentMatRef;
    public Material opaqueMatRef;

    public Texture deactivatedTexture;
    public Texture activatedTexture;
}
