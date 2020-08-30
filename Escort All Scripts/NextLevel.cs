
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class NextLevel : UdonSharpBehaviour
{
    public Transform TeleportPoint;
    public GameObject NextLvl;
    public GameObject[] OtherLvls;

    public void Interact()
    {
        NextLvl.SetActive(true);
        Networking.LocalPlayer.TeleportTo(TeleportPoint.position, TeleportPoint.rotation);
        for (int i = 0; i < OtherLvls.Length; i++)
        {
            OtherLvls[i].SetActive(false);
        }
    }
}
