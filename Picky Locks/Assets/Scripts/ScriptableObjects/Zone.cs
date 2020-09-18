using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Zone : ScriptableObject
{
    public List<LevelData> levels;
    public int sceneBuildIndex;
    public Color fogGradientTopColor;
    public Color fogGradientBottomColor;
}
