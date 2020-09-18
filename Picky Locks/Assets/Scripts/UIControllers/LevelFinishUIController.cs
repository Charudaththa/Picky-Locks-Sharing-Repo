using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinishUIController : MonoBehaviour
{
    public void LoadNextLevel()
    {
        FindObjectOfType<SceneLoader>().LoadNextScene();
    }
}

