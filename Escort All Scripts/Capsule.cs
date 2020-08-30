
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Capsule : UdonSharpBehaviour
{
    public Blaster Blaster;

    [UdonSynced] public int Team;
    private bool TimerActive;
    private float currentTime;
    private float wantedTime = 0.5f;

    public void Update()
    {
        if (TimerActive)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= wantedTime)
            {
                TimerActive = false;
                currentTime = 0.0f;
            }
        }
    }

    public void SetTeam()
    {
        Team = Blaster.Team;
    }

    public void Shot()
    {
        if (TimerActive) return;
        Blaster.GetShot();
        TimerActive = true;
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col == null || col.gameObject.layer != 24 || col.gameObject.name != "LaserHit") return;
        Blaster.GetShot();
    }
}
