using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDeathUIController : MonoBehaviour
{
    public void ReloadScene()
    {
        SceneLoader.Instance.ReloadScene();
    }
}
