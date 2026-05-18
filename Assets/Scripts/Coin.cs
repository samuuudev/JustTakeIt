using System;
using MelenitasDev.SoundsGood;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Sound sound;

    private void Start()
    {
        sound = new Sound(SFX.coin).SetRandomClip(true).SetOutput(Output.SFX).SetVolume(AudioSettingsManager.Instance.CurrentSFXVolumeLinear);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.AddPoint();
            Destroy(gameObject);
            sound.Play();
        }
    }
}