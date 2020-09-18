using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;
    public static SceneLoader Instance;

    int totalLevelCount;
    int zoneIndex;
    int zoneLevelIndex;

    public List<Zone> zoneData;
    public static int loadSceneIndex;
    public bool resetOnStart;

    public List<Material> materialsToSwitchColor;
    public List<Material> otherMaterialsToSwitchColor;
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        } else if (Instance!= this)
        {
            Destroy(gameObject);
        }
        loadSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void Start()
    {
        if (resetOnStart)
            ResetLevelAndZone();

        SceneManager.LoadSceneAsync(PlayerPrefs.GetInt("zone"));
        foreach (Material material in otherMaterialsToSwitchColor)
        {
            material.SetColor("Color_8F941365", zoneData[PlayerPrefs.GetInt("zone")].fogGradientTopColor);
            material.SetColor("Color_14BCE152", zoneData[PlayerPrefs.GetInt("zone")].fogGradientBottomColor);
        }

        foreach (Material material in materialsToSwitchColor)
        {
            material.SetColor("Color_86DA637B", zoneData[PlayerPrefs.GetInt("zone")].fogGradientTopColor);
            material.SetColor("Color_C5FB9E42", zoneData[PlayerPrefs.GetInt("zone")].fogGradientBottomColor);
        }

    }
    public void OnLevelEnd()
    {
        //TODO: Call this method on level end in level generator
        Key.isKeyActive = false;
        UIManager.Instance.goldBarUI.gameObject.SetActive(false);
        PlayerPrefs.SetInt("totalLevelCount", PlayerPrefs.GetInt("totalLevelCount") + 1);

        zoneLevelIndex = PlayerPrefs.GetInt("zoneLevelCount") + 1;
        zoneIndex = PlayerPrefs.GetInt("zone");

        //if new zone
        if (zoneLevelIndex > zoneData[zoneIndex].levels.Count - 1)
        {
            zoneIndex++;
            //if zone isnt too great
            if (zoneIndex < zoneData.Count)
            {
                PlayerPrefs.SetInt("zone", zoneIndex);
                PlayerPrefs.SetInt("zoneLevelCount", 0);
            }
            else
            {
                zoneIndex = 0;
                PlayerPrefs.SetInt("zone", 0);
                PlayerPrefs.SetInt("zoneLevelCount", 0);
            }
            loadSceneIndex = zoneData[zoneIndex].sceneBuildIndex;
            EndReward.Instance.SpawnReward(true);
        }
        else
        //if just next level in zone
        {
            PlayerPrefs.SetInt("zoneLevelCount", zoneLevelIndex);
            loadSceneIndex = SceneManager.GetActiveScene().buildIndex;
            EndReward.Instance.SpawnReward(false);
        }

        PlayerPrefs.Save();
    }
    public void LoadNextScene()
    {
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(loadSceneIndex);
        Key.isKeyActive = true;
        totalLevelCount = PlayerPrefs.GetInt("totalLevelCount");
        zoneIndex = PlayerPrefs.GetInt("zone");

        UIManager.Instance.onDeathUI.SetActive(false);
        UIManager.Instance.onLevelEndUI.SetActive(false);
        UIManager.Instance.onZoneEndUI.SetActive(false);
        UIManager.Instance.tapToOpenUI.SetActive(false);

        foreach(Material material in otherMaterialsToSwitchColor)
        {
            material.SetColor("Color_8F941365", zoneData[zoneIndex].fogGradientTopColor);
            material.SetColor("Color_14BCE152", zoneData[zoneIndex].fogGradientBottomColor);
        }

        foreach (Material material in materialsToSwitchColor)
        {
            material.SetColor("Color_86DA637B", zoneData[zoneIndex].fogGradientTopColor);
            material.SetColor("Color_C5FB9E42", zoneData[zoneIndex].fogGradientBottomColor);
        }
    }


    public void ReloadScene()
    {
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(loadSceneIndex);

        Key.isKeyActive = true;
        UIManager.Instance.onDeathUI.SetActive(false);
        UIManager.Instance.onLevelEndUI.SetActive(false);
        UIManager.Instance.onZoneEndUI.SetActive(false);
        UIManager.Instance.tapToOpenUI.SetActive(false);

        foreach (Material material in otherMaterialsToSwitchColor)
        {
            material.SetColor("Color_8F941365", zoneData[zoneIndex].fogGradientTopColor);
            material.SetColor("Color_14BCE152", zoneData[zoneIndex].fogGradientBottomColor);
        }

        foreach (Material material in materialsToSwitchColor)
        {
            material.SetColor("Color_86DA637B", zoneData[zoneIndex].fogGradientTopColor);
            material.SetColor("Color_C5FB9E42", zoneData[zoneIndex].fogGradientBottomColor);
        }
    }

    public void ResetLevelAndZone()
    {
        PlayerPrefs.SetInt("totalLevelCount", 0);
        PlayerPrefs.SetInt("zoneLevelCount", 0);
        PlayerPrefs.SetInt("zone", 0);
    }
}
