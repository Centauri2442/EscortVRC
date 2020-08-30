
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VisualRingStuff : UdonSharpBehaviour
{
    public GameObject[] ObjectToggles;
    public Animator Ring;
    public AudioSource Audio;
    public AudioClip Clip;

    public void OnTriggerEnter(Collider col)
    {
        if (col == null || col.gameObject.layer != 23) return;
        
        RingFlash();
    }
    
    public void RingFlash()
    {
        for (int i = 0; i < ObjectToggles.Length; i++)
        {
            ObjectToggles[i].SetActive(true);
        }
        Ring.SetBool("Active", true);
        Audio.PlayOneShot(Clip);
    }
}
