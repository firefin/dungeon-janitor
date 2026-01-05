using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SettingsController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown graphicsDropdown;
    [SerializeField] private Slider volumeSlider;   

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;       

    [Header("Behavior")]
    [SerializeField] private bool applyOnSceneLoad = true;

    private Resolution[] resolutions;
    private List<(int w, int h)> uniqueSizes;              

    // PlayerPrefs keys 
    const string KEY_RES_W = "settings.resolution.width";
    const string KEY_RES_H = "settings.resolution.height";
    const string KEY_QLT = "settings.qualityIndex";
    const string KEY_VOL = "settings.masterVolumeDb";

    void Awake()
    {
        resolutions = Screen.resolutions;
        if (applyOnSceneLoad) SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (applyOnSceneLoad) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        BuildResolutionDropdown();     
        BuildGraphicsDropdown();

        // Load saved 
        int savedW = PlayerPrefs.GetInt(KEY_RES_W, Screen.currentResolution.width);
        int savedH = PlayerPrefs.GetInt(KEY_RES_H, Screen.currentResolution.height);
        int savedQ = PlayerPrefs.GetInt(KEY_QLT, QualitySettings.GetQualityLevel());
        float savedDb = PlayerPrefs.GetFloat(KEY_VOL, 0f);     

        // Reflect in UI
        if (resolutionDropdown)
        {
            resolutionDropdown.value = IndexOfSize(savedW, savedH);
            resolutionDropdown.RefreshShownValue();
        }
        if (graphicsDropdown)
        {
            graphicsDropdown.value = Mathf.Clamp(savedQ, 0, QualitySettings.names.Length - 1);
            graphicsDropdown.RefreshShownValue();
        }
        if (volumeSlider)
        {
            volumeSlider.minValue = -80f; volumeSlider.maxValue = 0f;
            volumeSlider.value = Mathf.Clamp(savedDb, -80f, 0f);
        }

        // Apply to engine once on start
        ApplyResolution(savedW, savedH);
        QualitySettings.SetQualityLevel(savedQ);
        if (audioMixer) audioMixer.SetFloat("volume", savedDb);

        // Hook events
        if (resolutionDropdown) resolutionDropdown.onValueChanged.AddListener(OnResolutionDropdown);
        if (graphicsDropdown) graphicsDropdown.onValueChanged.AddListener(SetQuality);
        if (volumeSlider) volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    // ---------- UI Events ----------
    private void OnResolutionDropdown(int dropdownIndex)
    {
        var (w, h) = uniqueSizes[Mathf.Clamp(dropdownIndex, 0, uniqueSizes.Count - 1)];
        ApplyResolution(w, h);
        PlayerPrefs.SetInt(KEY_RES_W, w);
        PlayerPrefs.SetInt(KEY_RES_H, h);
        PlayerPrefs.Save();
    }

    public void SetQuality(int qualityIndex)
    {
        qualityIndex = Mathf.Clamp(qualityIndex, 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt(KEY_QLT, qualityIndex);
        PlayerPrefs.Save();
    }

    public void SetVolume(float volumeDb) 
    {
        if (audioMixer) audioMixer.SetFloat("volume", volumeDb);
        PlayerPrefs.SetFloat(KEY_VOL, volumeDb);
        PlayerPrefs.Save();
    }

    // ---------- Apply helpers ----------
    private void OnSceneLoaded(Scene _, LoadSceneMode __)
    {
        int w = PlayerPrefs.GetInt(KEY_RES_W, Screen.currentResolution.width);
        int h = PlayerPrefs.GetInt(KEY_RES_H, Screen.currentResolution.height);
        int q = PlayerPrefs.GetInt(KEY_QLT, QualitySettings.GetQualityLevel());
        float db = PlayerPrefs.GetFloat(KEY_VOL, 0f);

        ApplyResolution(w, h);
        QualitySettings.SetQualityLevel(q);
        if (audioMixer) audioMixer.SetFloat("volume", db);
    }

    private void ApplyResolution(int w, int h)
    {

        RefreshRate currentRR = Screen.currentResolution.refreshRateRatio;
        RefreshRate bestRR = currentRR;

        foreach (var r in resolutions)
        {
            if (r.width == w && r.height == h)
            {
                if (r.refreshRateRatio.numerator * 1f / Mathf.Max(1, r.refreshRateRatio.denominator) >
                    bestRR.numerator * 1f / Mathf.Max(1, bestRR.denominator))
                {
                    bestRR = r.refreshRateRatio;
                }
            }
        }

        Screen.SetResolution(w, h, Screen.fullScreenMode, bestRR);
    }

    // ---------- Builders ----------
    private void BuildResolutionDropdown()
    {
        uniqueSizes = new List<(int, int)>();
        var labels = new List<string>();
        var seen = new HashSet<string>();

        foreach (var r in resolutions)
        {
            string key = r.width + "x" + r.height;
            if (seen.Add(key))
            {
                uniqueSizes.Add((r.width, r.height));
                labels.Add($"{r.width} x {r.height}");
            }
        }

        if (resolutionDropdown)
        {
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(labels);
            // Default to current size
            resolutionDropdown.value = IndexOfSize(Screen.currentResolution.width, Screen.currentResolution.height);
            resolutionDropdown.RefreshShownValue();
        }
    }

    private void BuildGraphicsDropdown()
    {
        if (!graphicsDropdown) return;
        graphicsDropdown.ClearOptions();
        graphicsDropdown.AddOptions(new List<string>(QualitySettings.names));
    }

    private int IndexOfSize(int w, int h)
    {
        for (int i = 0; i < uniqueSizes.Count; i++)
            if (uniqueSizes[i].w == w && uniqueSizes[i].h == h) return i;
        return Mathf.Clamp(uniqueSizes.Count - 1, 0, int.MaxValue); 
    }
}
