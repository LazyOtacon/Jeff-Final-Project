using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource sfxSource;

    public static AudioManager instance;

    public AudioClip gunShoot;
    public AudioClip gunReload;
    public AudioClip playerJump;
    public AudioClip playerHit;
    public AudioClip playerDeath;
    public AudioClip playerJoin;
    public AudioClip playerLeave;


    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

}
