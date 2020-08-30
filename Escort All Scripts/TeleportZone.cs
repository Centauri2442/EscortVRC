
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TeleportZone : UdonSharpBehaviour
{
    public Transform TeleportDestination;

    public void OnTriggerEnter(Collider col)
    {
        if (col == null || col.gameObject.layer != 10) return;
        
        Networking.LocalPlayer.TeleportTo(TeleportDestination.position, TeleportDestination.rotation);
    }
}
