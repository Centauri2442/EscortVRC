
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class AIController : UdonSharpBehaviour
{
    public Transform[] AIs;

    //public GlobalSettingsManager Settings;
    //private int DifficultyRating;

    private bool Timer;
    private float currentTime;
    public float wantedTime = 1f;
    private int CurrentCount = 0;

    private bool KillTimer;
    private float currentTimeKill;
    private float wantedTimeKill;

    public bool AutoFillArray = true;
    
    public void Start()
    {

        if (AutoFillArray)
        {
            AIs = new Transform[transform.childCount];
            for (int i = 0; i < AIs.Length; i++)
            {
                AIs[i] = transform.GetChild(i);
            }
        }
        
        Timer = true;
        SetDroneActive();
        wantedTimeKill = AIs.Length * wantedTime + 1;
    }

    public void Update()
    {
        //if (!Networking.LocalPlayer.isMaster) return;
        if (Timer && CurrentCount < AIs.Length)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= wantedTime)
            {
                currentTime = 0.0f;
                SetDroneActive();
            }
        }
        else
        {
            Timer = false;
            CurrentCount = 0;
        }

        if (KillTimer)
        {
            currentTimeKill += Time.deltaTime;
            if (currentTimeKill >= wantedTimeKill)
            {
                KillImmediate();

                CurrentCount = 0;
                KillTimer = false;
            }
        }
    }

    public void SetDroneActive()
    {
        AIs[CurrentCount].gameObject.SetActive(true);
        CurrentCount++;
    }

    public void KillAll()
    {
        KillTimer = true;
    }

    public void KillImmediate()
    {
        foreach (var AI in AIs)
        {
            if (AI.gameObject.activeSelf)
            {
                if (AI.GetComponent<EnemyAI>() != null)
                {
                    AI.GetComponent<EnemyAI>().Die();
                }
                else if (AI.GetComponent<WormSegment>() != null)
                {
                    AI.GetComponent<WormSegment>().Die();
                }
            }
        }
    }
    
}
