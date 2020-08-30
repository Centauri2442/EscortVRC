
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class StartButton : UdonSharpBehaviour
{
    public Animator AnimationController;
    public CoffinDanceAI CoffinAI;

    public Text MasterText;
    [UdonSynced] public string Master;

    public GameObject Instructions;

    public void Start()
    {
        if (Networking.LocalPlayer.isMaster) Master = Networking.LocalPlayer.displayName;
    }
    
    public void StartMatch()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        CoffinAI.NetworkStart();
        SendCustomNetworkEvent(NetworkEventTarget.All, "StartMatchNetwork");
    }

    public void Update()
    {
        MasterText.text = Master;
    }
    

    public void StartMatchNetwork()
    {
        AnimationController.SetBool("Start", true);
        Instructions.SetActive(false);
    }
}
