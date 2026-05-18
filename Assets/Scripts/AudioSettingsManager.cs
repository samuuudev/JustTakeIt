using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsManager : MonoBehaviour
{
    public static AudioSettingsManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string masterParameter = "Master";
    [SerializeField] private string sfxParameter = "SFX";
    [SerializeField] private string musicParameter = "Music";

    [Header("Defaults")]
    [Range(0.0001f, 1f)]
    [SerializeField] private float defaultVolume = 1f;

    private const string MasterVolumeKey = "MasterVolumeLinear";
    private const string SFXVolumeKey = "SFXVolumeLinear";
    private const string MusicVolumeKey = "MusicVolumeLinear";

    public float CurrentMasterVolumeLinear { get; private set; } = 1f;
    public float CurrentSFXVolumeLinear { get; private set; } = 1f;
    public float CurrentMusicVolumeLinear { get; private set; } = 1f;

    /// <summary>
    /// Ejecuta al iniciar la escena. Se asegura de que solo exista una instancia de AudioSettingsManager (singleton) y carga los valores guardados de volumen usando PlayerPrefs. Si no hay valores guardados, se usan los valores por defecto.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        float savedMaster = PlayerPrefs.GetFloat(MasterVolumeKey, defaultVolume);
        float savedSFX = PlayerPrefs.GetFloat(SFXVolumeKey, defaultVolume);
        float savedMusic = PlayerPrefs.GetFloat(MusicVolumeKey, defaultVolume);
        SetMasterVolume(savedMaster, save: false);
        SetSFXVolume(savedSFX, save: false);
        SetMusicVolume(savedMusic, save: false);
    }

    // Métodos públicos para que los sliders puedan llamarlos
    public void SetMasterVolume(float linearVolume)
    {
        SetMasterVolume(linearVolume, save: true);
    }
    
    public void SetSFXVolume(float linearVolume)
    {
        SetSFXVolume(linearVolume, save: true);
    }

    public void SetMusicVolume(float linearVolume)
    {
        SetMusicVolume(linearVolume, save: true);
    }

    /// <summary>
    /// Actualiza el volumen maestro (global, sfx y musica) en el mixer y guarda la preferencia usando PlayerPrefs.
    /// </summary>
    /// <param name="linearVolume"></param> Volumen a guardar en formato lineal (0.0001 a 1). Se convertirá a dB internamente.
    /// <param name="save"></param> Si es true, guarda el valor en PlayerPrefs. Si es false, solo lo aplica al mixer sin guardar (útil para cargar al inicio).
    private void SetMasterVolume(float linearVolume, bool save)
    {
        linearVolume = Mathf.Clamp(linearVolume, 0.0001f, 1f);
        CurrentMasterVolumeLinear = linearVolume;

        float dB = Mathf.Log10(linearVolume) * 20f;
        if (mixer != null)
            mixer.SetFloat(masterParameter, dB); // <- usar Master

        if (save)
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, linearVolume);
            PlayerPrefs.Save();
        }
    }
    
    /// <summary>
    /// Actualiza el volumen de SFX (sonidos) en el mixer y guarda la preferencia usando PlayerPrefs.
    /// </summary>
    /// <param name="linearVolume"></param> Volumen a guardar en formato lineal (0.0001 a 1). Se convertirá a dB internamente.
    /// <param name="save"></param> Si es true, guarda el valor en PlayerPrefs. Si es false, solo lo aplica al mixer sin guardar (útil para cargar al inicio).
    private void SetSFXVolume(float linearVolume, bool save)
    {
        linearVolume = Mathf.Clamp(linearVolume, 0.0001f, 1f);
        CurrentSFXVolumeLinear = linearVolume;

        float dB = Mathf.Log10(linearVolume) * 20f;
        if (mixer != null)
            mixer.SetFloat(sfxParameter, dB); // <- usar SFX

        if (save)
        {
            PlayerPrefs.SetFloat(SFXVolumeKey, linearVolume);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Actualiza el volumen de música en el mixer y guarda la preferencia usando PlayerPrefs.
    /// </summary>
    /// <param name="linearVolume"></param> Volumen a guardar en formato lineal (0.0001 a 1). Se convertirá a dB internamente.
    /// <param name="save"></param> Si es true, guarda el valor en PlayerPrefs. Si es false, solo lo aplica al mixer sin guardar (útil para cargar al inicio).
    private void SetMusicVolume(float linearVolume, bool save)
    {
        linearVolume = Mathf.Clamp(linearVolume, 0.0001f, 1f);
        CurrentMusicVolumeLinear = linearVolume;

        float dB = Mathf.Log10(linearVolume) * 20f;
        if (mixer != null)
            mixer.SetFloat(musicParameter, dB); // <- usar Music

        if (save)
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, linearVolume);
            PlayerPrefs.Save();
        }
    }
}