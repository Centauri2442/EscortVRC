
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class WormController : UdonSharpBehaviour
{
    public GameObject[] RandomAI;
    public Transform WormDistanceCheck;
    public Transform WormTarget;
    public float DistanceFromTarget;
    public WormSegment[] Worm;
    public Animator AnimatorController;

    private bool AllDead;
    private bool HasWon;

    private float currentTime;

    [UdonSynced] public int CurrentAICount;

    public void Update()
    {
        if (HasWon || !Networking.LocalPlayer.isMaster) return;

        DistanceFromTarget = Vector3.Distance(WormDistanceCheck.position, WormTarget.position);
        
        currentTime += Time.deltaTime;
        
        for (int i = 0; i < Worm.Length; i++)
        {
            if (!Worm[i].Dead)
            {
                AllDead = false;
            }
            else
            {
                AllDead = true;
            }
        }

        if (AllDead)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "FinishGame");
            HasWon = true;
        }

        if (currentTime >= 10f && CurrentAICount < RandomAI.Length && !HasWon)
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "ActivateDrones");
            currentTime = 0.0f;
            CurrentAICount++;
        }
        
        
    }

    public void FinishGame()
    {
        for (int i = 0; i < Worm.Length; i++)
        {
            Worm[i].Explode();
        }
        AnimatorController.SetBool("FinishedGame", true);
    }

    public void ActivateDrones()
    {
        RandomAI[CurrentAICount].SetActive(true);
    }
}
