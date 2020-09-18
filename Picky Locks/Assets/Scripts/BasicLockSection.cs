using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BasicLockSection : MonoBehaviour
{
    public Lock owningLock;

    //currently filled sections
    public List<int> unfilledAngles;
    //the angles that this covers
    public Vector2Int angleRange;

    public int tempPositionHolder;

    public bool isActive;
    public LevelGenerator generator;

    private void Start()
    {
        generator = FindObjectOfType<LevelGenerator>();
    }
    public void ActivateSection()
    {
        isActive = true;
    }
    public void DeactivateSection()
    {
        isActive = false;
    }
    public void Initialize(LevelGenerator generateLevelTest)
    {
        unfilledAngles = new List<int>();

        int startAngle = angleRange.x;
        int endAngle = angleRange.y < angleRange.x ? angleRange.y + 360 : angleRange.y;
        for (int i = startAngle; i <= endAngle; i++)
        {
            unfilledAngles.Add(i >= 360 ? i - 360 : i);
        }

        generator = generateLevelTest;
    }

    private void FixedUpdate()
    {
        //if (isActive && !generator.deactivateAll)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        tempPositionHolder = Mathf.FloorToInt(owningLock.GetAngle());
        //    }
        //    if (Input.GetMouseButton(0))
        //    {

        //        int angle = Mathf.FloorToInt(owningLock.GetAngle());

        //        //if (angle < tempPositionHolder)
        //            ///angle += 360;
        //        //        else
        //        //the lock angle should not change? 
        //        int startingAngle = angle - 10;

        //        for (int i = startingAngle; i <= angle; i++)
        //        {

        //            if (unfilledAngles.Remove(i >= 360 ? (i - 360) : 360-i))
        //            {
        //                //generator.basicParticleEffect.Simulate(0f, true, true);
        //                //generator.basicParticleEffect.Play();
        //                //int testing = i + 180;
        //                //generator.texture.SetPixel(i >= 360 ? (i - 360) : i, 0, Color.black);
        //            }


        //            if (unfilledAngles.Count == 0)
        //            {
        //                if (isActive)
        //                {
        //                    owningLock.OnSectionComplete(this);
        //                    isActive = false;
        //                }

        //            }
        //        }
        //    }
        //    if (Input.GetMouseButtonUp(0))
        //    {
        //        //generator.basicParticleEffect.Simulate(0f, true, true);
        //    }
        //    if (unfilledAngles.Count == 0)
        //    {
        //        owningLock.OnSectionComplete(this);
        //        isActive = false;
        //    }
        //}
    }

    private void OnTriggerEnter(Collider other)
    {

    }
}