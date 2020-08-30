
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SettingsManagerStuff : UdonSharpBehaviour
{
    public AudioSource Lvl1Music;
    public GameObject ToggleIdleMusic;


    public void ToggleMusic()
    {
        ToggleIdleMusic.SetActive(!ToggleIdleMusic);
        
        if (Lvl1Music.isPlaying)
        {
            Lvl1Music.playOnAwake = false;
        }
        else
        {
            Lvl1Music.playOnAwake = true;
        }
    }
}
