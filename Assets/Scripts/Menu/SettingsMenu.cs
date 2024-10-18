using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class SettingsMenu : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("Res. Options")]
    [SerializeField] private TMP_Dropdown _resolutionMenu;

    [Header("Frame Rate Slider")]
    [SerializeField] private Slider FrameRateSlider;
    [SerializeField] private TextMeshProUGUI FrameRateText;

    [Header("Master vol Slider")]
    [SerializeField] private Slider masterVolSlider;

    [Header("SFX Vol Slider")]
    [SerializeField] private Slider sfxVolSlider;

    [Header("Menu SFX Slider")]
    [SerializeField] private Slider menuSFXVolSlider;
    //Resolution picker
    private Resolution[] availableResoltions;
    private int _resIndex;

    private int[] FrameRates;

    private bool _fullScreen;
    private void Start()
    {
        availableResoltions = Screen.resolutions;
        _resolutionMenu.ClearOptions();

        List<string> options = new List<string>();
        _resIndex = 0;

        for (int i = 0; i < availableResoltions.Length; i++)
        {
            string name = $"{availableResoltions[i].width} x {availableResoltions[i].height}";
            options.Add(name);
        
            if(availableResoltions[i].width == Screen.currentResolution.width && 
                availableResoltions[i].height == Screen.currentResolution.height)
            {
                _resIndex = i;
            }
        }

        _resolutionMenu.AddOptions(options);
        _resolutionMenu.value = _resIndex;
        _resolutionMenu.RefreshShownValue();

        FrameRateText.text = Application.targetFrameRate.ToString();
    }

    private void OnEnable()
    {
        FrameRateSlider.value = Application.targetFrameRate;
        FrameRateText.text = Application.targetFrameRate.ToString();

        if (_audioMixer.GetFloat("MasterVolume", out float master))
        {
            masterVolSlider.value = master;
        }
        if (_audioMixer.GetFloat("SFX_Volume", out float SFX))
        {
            sfxVolSlider.value = SFX;
        }
        if (_audioMixer.GetFloat("MenuSFX_Volume", out float MenuSFX))
        {
            menuSFXVolSlider.value = MenuSFX;
        }


    }
    public void SetMasterVolume(float vol)
    {
        _audioMixer.SetFloat("MasterVolume", vol);
    }
    public void SetSFXVolume(float vol)
    {
        _audioMixer.SetFloat("SFX_Volume", vol);
    }
    public void SetMenuVolume(float vol)
    {

        _audioMixer.SetFloat("MenuSFX_Volume", vol);
    }

    public void SetFrameRate(float target)
    {

        int t = Mathf.RoundToInt(target);
        Application.targetFrameRate = t;
        FrameRateText.text = Application.targetFrameRate.ToString();
    }


    public void SetGraphics(int _qualityLevl)
    {
        QualitySettings.SetQualityLevel(_qualityLevl);
    }

    public void ToggleFullScreen(bool isFul)
    {
        _fullScreen = isFul;
    }

    public void ChangeResolution(int resIndex)
    {
        Resolution newRes = availableResoltions[_resIndex];
        Screen.SetResolution(newRes.width, newRes.height, _fullScreen);
    }
}
