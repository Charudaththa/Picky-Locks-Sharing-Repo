using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<UIManager>();
            return _instance;
        }
    }

    public GameObject onStartUI;
    public GameObject hudCanvas;
    public GameObject tapToOpenUI;
    public GameObject onLevelEndUI;
    public GameObject onZoneEndUI;
    public GameObject onDeathUI;
    public GameObject goldBarUI;
    public GameObject goldEffectUI;
    public Text levelCountText;
    public Text zoneCountText;
}