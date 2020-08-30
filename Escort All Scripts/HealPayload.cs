
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class HealPayload : UdonSharpBehaviour
{
    public CoffinDanceAI Payload;
    public bool RestoreHealth;
    public bool AddHealth;
    public float HealthToAdd;
    
    public void Start()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        if(RestoreHealth && Payload.Health < Payload.DefaultHealth) Payload.Health = Payload.DefaultHealth;

        if (AddHealth) Payload.Health += HealthToAdd;
    }

    public void OnEnable()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        if(RestoreHealth && Payload.Health < Payload.DefaultHealth) Payload.Health = Payload.DefaultHealth;

        if (AddHealth) Payload.Health += HealthToAdd;
    }
}
