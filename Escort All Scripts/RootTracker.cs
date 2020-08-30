
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RootTracker : UdonSharpBehaviour
{
    public Transform Root;

    private VRCPlayerApi Player;

    public void Start()
    {
        Player = Networking.LocalPlayer;
    }
    
    public void Update()
    {
        Root.position = Player.GetPosition();
    }
}
