
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class HitDetection : UdonSharpBehaviour
{
    public EnemyAI AI;

    public void OnTriggerEnter(Collider col)
    {
        if (col == null || col.gameObject.layer != 26) return;

        if (col.gameObject.name.Contains("Pistol"))
        {
            AI.SendCustomNetworkEvent(NetworkEventTarget.Owner, "HitPistol");
        }
        else if (col.gameObject.name.Contains("TripleBlast"))
        {
            AI.SendCustomNetworkEvent(NetworkEventTarget.Owner, "HitTripleBlast");
        }
        else if (col.gameObject.name.Contains("ChargeBlaster"))
        {
            AI.SendCustomNetworkEvent(NetworkEventTarget.Owner, "HitChargeBlaster");
        }

        switch (AI.AIIdentifier)
        {
            case 0:
                AI.Settings.PointsSystem.AddPoints(AI.Settings.PointsPerHit);
                break;
            case 1:
                AI.Settings.PointsSystem.AddPoints(AI.Settings.BigPointsPerHit);
                break;
            case 2:
                AI.Settings.PointsSystem.AddPoints(AI.Settings.MassivePointsPerHit);
                break;
        }
        
        var Hit = VRCInstantiate(AI.HitPrefab);
        Hit.transform.SetPositionAndRotation(AI.AI.transform.position, AI.AI.transform.rotation);
        Hit.transform.SetParent(AI.CleanUpSystem);
    }
    
}
