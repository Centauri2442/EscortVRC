using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VisualRingStuff2 : UdonSharpBehaviour
{
    public GameObject[] ObjectToggles;
    public Animator Ring;
    public AudioSource Audio;
    public AudioClip Clip;
    public Animator DestroyBridge;

    public void OnTriggerEnter(Collider col)
    {
        if (col == null || col.gameObject.layer != 23) return;
        RingFlash();
        DestroyBridge.SetBool("DestroyBridge", true);
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