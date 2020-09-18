using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    public List<LockData> lockStructure;
}

public enum LockData
{
    NormalLockOne,
    NormalLockTwo,
    NormalLockThree,
    RotatingLockOne,
    RotatingLockTwo,
    RotatingLockThree,
    RotatingLockFour,
    TimedLockOne,
    TimedLockTwo,
    TimedLockThree,
    ToggleLockOne,
    ToggleLockTwo,
    ToggleLockThree
}