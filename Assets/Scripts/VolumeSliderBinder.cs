using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSliderBinder : MonoBehaviour
{
    private enum VolumeChannel
    {
        Master,
        SFX,
        Music
    }

    [SerializeField] private Slider slider;
    [SerializeField] private VolumeChannel channel = VolumeChannel.Master;

    private void Reset()
    {
        slider = GetComponent<Slider>();
    }

    private void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();

        slider.minValue = 0.0001f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
    }

    private void OnEnable()
    {
        if (AudioSettingsManager.Instance == null)
            return;

        slider.SetValueWithoutNotify(GetCurrentChannelVolume());
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnDisable()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    private float GetCurrentChannelVolume()
    {
        switch (channel)
        {
            case VolumeChannel.SFX:
                return AudioSettingsManager.Instance.CurrentSFXVolumeLinear;
            case VolumeChannel.Music:
                return AudioSettingsManager.Instance.CurrentMusicVolumeLinear;
            default:
                return AudioSettingsManager.Instance.CurrentMasterVolumeLinear;
        }
    }

    private void OnSliderChanged(float value)
    {
        if (AudioSettingsManager.Instance == null)
            return;

        switch (channel)
        {
            case VolumeChannel.SFX:
                AudioSettingsManager.Instance.SetSFXVolume(value);
                break;
            case VolumeChannel.Music:
                AudioSettingsManager.Instance.SetMusicVolume(value);
                break;
            default:
                AudioSettingsManager.Instance.SetMasterVolume(value);
                break;
        }
    }
}