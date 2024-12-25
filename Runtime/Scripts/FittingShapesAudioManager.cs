using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{

    public AudioClip Tap;
    public AudioClip Touch;
    public AudioClip WrongTouch;
    public AudioClip FireworksSound;
    public AudioClip BackgroundMusic;

    [Range(0.0f, 1f)]
    public float TapVol;
    [Range(0.0f, 1f)]
    public float TouchVol;
    [Range(0.0f, 1f)]
    public float WrongTouchVol;
    [Range(0.0f, 1f)]
    public float FireworksSoundVolume;
    [Range(0.0f, 1f)]
    public float BackgroundMusVol;

    public AudioSource TapSrc;
    public AudioSource TouchSrc;
    public AudioSource WrongTouchSrc;
    public AudioSource FireworksSoundSrc;
    public AudioSource BackgroundMusicSrc;




    public void PlayTapSound()
    {
        TapSrc.volume = TapVol;
        TapSrc.PlayOneShot(Tap);
    }

    public void PlayTouchSound()
    {
        TouchSrc.volume = TouchVol;
        TouchSrc.PlayOneShot(Touch);
    }

    public void PlayWrongTouchSound()
    {
        WrongTouchSrc.volume = WrongTouchVol;
        WrongTouchSrc.PlayOneShot(WrongTouch);
    }


    public void PlayFireworksSound()
    {
        FireworksSoundSrc.volume = FireworksSoundVolume;
        FireworksSoundSrc.PlayOneShot(FireworksSound);
    }


    public void PlayBackgroundMusic()
    {
        BackgroundMusicSrc.volume = BackgroundMusVol;
        BackgroundMusicSrc.loop = true;
        BackgroundMusicSrc.clip = BackgroundMusic;
        BackgroundMusicSrc.Play();
    }


    // Start is called before the first frame update
    void Start()
    {
        PlayBackgroundMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
