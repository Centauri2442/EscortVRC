
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class KillBarrier : UdonSharpBehaviour
{
    public AIController[] Controllers;
    public Animator MusicAnimator;
    public bool IsShop;
    public int MusicTarget;

    public void OnTriggerEnter(Collider col)
    {
        if (col == null || col.gameObject.layer != 23 || !Networking.LocalPlayer.isMaster) return;
        SendCustomNetworkEvent(NetworkEventTarget.All, "NetworkSet");
    }

    public void NetworkSet()
    {
        foreach (var controller in Controllers)
        {
            controller.KillImmediate();
        }
        MusicAnimator.SetInteger("MusicTarget", MusicTarget);
        MusicAnimator.SetBool("Shop", IsShop);
    }
}
