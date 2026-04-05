using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    public GameObject settingScreen;
    public GameObject unsavedChangesPopup;
    public TMP_Dropdown qualityDropdown;
    public UnityEngine.UI.Slider volumeSlider;
    public UnityEngine.UI.Toggle fullscreenToggle;
    public UnityEngine.UI.Toggle muteToggle;


    private Resolution[] resolutions;

    public bool hasUnsavedChanges { get; private set; }

    private int pendingResolutionIndex;
    private int pendingQualityIndex;
    private bool pendingFullscreen;
    private float pendingVolume;
    private bool pendingMuted;

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        unsavedChangesPopup.SetActive(false);

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        LoadSettings();
    }

    public void SetResolution(int resolutionIndex)
    {
        pendingResolutionIndex = resolutionIndex;
        MarkDirty();
    }

    public void SetVolume(float value)
    {
        pendingVolume = value;
        audioMixer.SetFloat("Volume", pendingMuted ? -80f : pendingVolume);
        MarkDirty();
    }
    public void SetQuality(int qualityIndex)
    {
        pendingQualityIndex = qualityIndex;
        MarkDirty();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        pendingFullscreen = isFullscreen;
        MarkDirty();
    }

    public void SetMute(bool isMuted)
    {
        pendingMuted = isMuted;
        MarkDirty();
    }

    public void SaveSettings()
    {
        Resolution res = resolutions[pendingResolutionIndex];
        Screen.SetResolution(res.width, res.height, pendingFullscreen);

        QualitySettings.SetQualityLevel(pendingQualityIndex);

        audioMixer.SetFloat("volume", pendingMuted ? -80f : pendingVolume);

        PlayerPrefs.SetInt("ResolutionIndex", pendingResolutionIndex);
        PlayerPrefs.SetInt("QualityIndex", pendingQualityIndex);
        PlayerPrefs.SetInt("Fullscreen", pendingFullscreen ? 1 : 0);
        PlayerPrefs.SetFloat("Volume", pendingVolume);
        PlayerPrefs.SetInt("Muted", pendingMuted ? 1 : 0);
        PlayerPrefs.Save();

        hasUnsavedChanges = false;
    }

    private void LoadSettings()
    {
        pendingResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", resolutionDropdown.value);
        pendingQualityIndex = PlayerPrefs.GetInt("QualityIndex", QualitySettings.GetQualityLevel());
        pendingFullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        pendingVolume = PlayerPrefs.GetFloat("Volume", 0f);
        pendingMuted = PlayerPrefs.GetInt("Muted", 0) == 1;

        resolutionDropdown.SetValueWithoutNotify(pendingResolutionIndex);
        qualityDropdown.SetValueWithoutNotify(pendingQualityIndex);
        fullscreenToggle.SetIsOnWithoutNotify(pendingFullscreen);
        muteToggle.SetIsOnWithoutNotify(pendingMuted);
        volumeSlider.SetValueWithoutNotify(pendingVolume);

        QualitySettings.SetQualityLevel(pendingQualityIndex);
        Screen.fullScreen = pendingFullscreen;
        audioMixer.SetFloat("volume", pendingMuted ? -80f : pendingVolume);

        hasUnsavedChanges = false;
    }

    private void MarkDirty()
    {
        hasUnsavedChanges = true;
    }

    public void CloseSettings()
    {
        if (!hasUnsavedChanges)
        {
            CloseIfOpen();
        }
        else if (hasUnsavedChanges)
        {
            unsavedChangesPopup.SetActive(true);
        }
    }

    public void CloseUnsavedChangesPopup()
    {
        unsavedChangesPopup.SetActive(false);
    }

    public void ExitWithoutSaving()
    {
        unsavedChangesPopup.SetActive(false);
        LoadSettings();          // restores DATA + UI
        CloseIfOpen();
    }


    void CloseIfOpen()
    {
        if (settingScreen.activeSelf)
            settingScreen.SetActive(false);
    }
}
