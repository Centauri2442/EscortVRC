
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class RepairHammer : UdonSharpBehaviour
{
    public CoffinDanceAI Payload;
    public float RepairPerHit = 5f;
    public AudioSource Audio;
    public AudioClip Clip;
    public GameObject HammerHit;
    public Transform HitPoint;
    public Transform CleanUpPoint;

    private bool Timer;
    private float currentTime;
    private float wantedTime = 1f;

    public void OnTriggerEnter(Collider col)
    {
        if (col == null || col.gameObject.layer != 27 || Timer || !col.gameObject.name.Contains("RepairLayer")) return;

        SendCustomNetworkEvent(NetworkEventTarget.All, "Repair");

        var Hit = VRCInstantiate(HammerHit);
        Hit.transform.position = HitPoint.position;
        Hit.transform.SetParent(CleanUpPoint);
        Audio.PlayOneShot(Clip);
    }

    public void Update()
    {
        if (Timer)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= wantedTime)
            {
                currentTime = 0.0f;
                Timer = false;
            }
        }
    }

    public void Repair()
    {
        if (Networking.LocalPlayer.isMaster)
        {
            if (Payload.Health < Payload.DefaultHealth)
            {
                if (Payload.Health >= Payload.DefaultHealth)
                {
                    return;
                }
                
                if (Payload.Health < Payload.DefaultHealth - RepairPerHit)
                {
                    Payload.Health += RepairPerHit;
                }
                else if (Payload.Health > Payload.DefaultHealth - RepairPerHit)
                {
                    Payload.Health += Payload.DefaultHealth - Payload.Health;
                }
                
            }
        }
        
        Timer = true;
    }
}
