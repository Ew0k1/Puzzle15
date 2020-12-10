using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    [SerializeField] private Sprite soundOn, soundOff;
    [SerializeField] private Button soundButton;
    public AudioMixerGroup mixer;

    private Image img;
    private bool isMuted = false;

    private void Start()
    {
        img = soundButton.GetComponent<Image>();

        if (PlayerPrefs.HasKey("IsMuted"))
        {
            isMuted = PlayerPrefs.GetInt("IsMuted") == 1;
        }
        SetImage();
        SetVolume();
        
    }

    public void Mute()
    {
        isMuted = !isMuted;

        SetImage();
        SetVolume();
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
    }


    void SetImage()
    {
        if (isMuted)
        {
            img.sprite = soundOff;
        }
        else
        {
            img.sprite = soundOn;
        }
    }

    void SetVolume()
    {
        if (isMuted)
        {
            mixer.audioMixer.SetFloat("MasterVolume", -80f);
        }
        else
        {
            mixer.audioMixer.SetFloat("MasterVolume", 0f);
        }
    }
}
