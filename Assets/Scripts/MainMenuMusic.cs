using MelenitasDev.SoundsGood;
using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    private Music music;

    private void Start()
    {
        float initialVolume = 1f;

        if (AudioSettingsManager.Instance != null)
            initialVolume = AudioSettingsManager.Instance.CurrentMusicVolumeLinear;
        else
            Debug.LogWarning("AudioSettingsManager.Instance es null en Start(). Uso volumen 1f por defecto.");

        music = new Music(Track.MainMenu)
            .SetLoop(true)
            .SetVolume(initialVolume)
            .SetOutput(Output.Music);

        music.Play();
    }

    public void RefreshMusicVolume()
    {
        if (music == null)
            return;

        float volume = 1f;
        if (AudioSettingsManager.Instance != null)
            volume = AudioSettingsManager.Instance.CurrentMusicVolumeLinear;

        music.SetVolume(volume);
    }
}