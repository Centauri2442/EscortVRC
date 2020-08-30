
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RespawnChamber : UdonSharpBehaviour
{
    public Transform RespawnPoint;
    public Transform DestinationPoint;
    public ParticleSystem Zap;
    public AudioSource Audio;
    public AudioClip ActivateClip;
    public GameObject RespawnSet;

    private bool Timer;
    private float currentTime;
    private float wantedTime = 5f;

    public void OnTriggerEnter(Collider col)
    {
        if (col == null || col.gameObject.layer != 10) return;
        currentTime = 0.0f;
        RespawnPoint.SetPositionAndRotation(DestinationPoint.position, DestinationPoint.rotation);
        Audio.PlayOneShot(ActivateClip);
        Zap.Play();
        Timer = true;
        RespawnSet.SetActive(true);
    }

    public void Update()
    {
        if (Timer)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= wantedTime)
            {
                RespawnSet.SetActive(false);
                Timer = false;
                currentTime = 0.0f;
            }
        }
    }
}
