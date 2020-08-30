using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class CoffinDanceAI : UdonSharpBehaviour
{
    [HideInInspector] public VRCPlayerApi Owner;
    public GlobalPointsSystem Points;
    public Text HealthDisplay;

    public GameObject[] Payloads;
    public GameObject[] SongSelection;
    public GameObject[] PayloadIdleMusic;
    [UdonSynced] public int Selection;

    public GameObject TimedObjects;

    public NavMeshAgent Agent;
    public bool IsMoving;
    public Transform[] Waypoints;
    public Animator[] Animators;
    public AudioSource Audio;
    public GameObject IdleStuff;
    public GameObject Chip;
    public Text CountdownText;

    [UdonSynced] public float Health = 150f;

    public bool IsEditor;

    public GameObject DeathStuff;
    public GameObject VictoryStuff;
    public AIController[] AIControllers;

    private bool IsMaster;

    [HideInInspector] public float DefaultHealth;

    private bool Timer;
    private float currentTime;
    private float wantedTime = 2f;
    private int currentTarget = 0;

    private bool StartTimer;
    private float currentTimeStart;
    public float wantedTimeStart = 19f;
    
    private bool ten = true;
    private bool nine = true;
    private bool eight = true;
    private bool seven = true;
    private bool six = true;
    private bool five = true;
    private bool four = true;
    private bool three = true;
    private bool two = true;
    private bool one = true;

    private bool SongTimeActive;
    private float TimeInSong;
    private bool HasWon = true;

    private bool StopMovingTimer;
    private float StopMovingTime;
    private float WantedStopMovingTime = 5f;

    [Header("Audio Countdown")] 
    
    public AudioClip Clip1;
    public AudioClip Clip2;
    public AudioClip Clip3;
    public AudioClip Clip4;
    public AudioClip Clip5;
    public AudioClip Clip6;
    public AudioClip Clip7;
    public AudioClip Clip8;
    public AudioClip Clip9;
    public AudioClip Clip10;
    public AudioClip ClipGo;
    
    public void Start()
    {
        if (Networking.LocalPlayer.isMaster)
        {
            Owner = Networking.LocalPlayer;
            Networking.SetOwner(Owner, gameObject);
            Agent.enabled = true;
            IsMaster = true;
        }
        else
        {
            Agent.enabled = false;
            IsMaster = false;
        }

        DefaultHealth = Health;

    }

    public void NetworkStart()
    {
        Debug.Log("Starting!");
        SendCustomNetworkEvent(NetworkEventTarget.All, "StartAudio");
    }

    public void StartAudio()
    {
        Audio.gameObject.SetActive(true);
        SongSelection[Selection].SetActive(true);
        foreach (var Audio in PayloadIdleMusic)
        {
            Audio.SetActive(false);
        }

        StartTimer = true;
        IdleStuff.SetActive(false);
        Chip.SetActive(true);
        HasWon = false;
    }

    public void StartRound()
    {
        Agent.SetDestination(Waypoints[0].position);
        Audio.PlayOneShot(ClipGo);
        CountdownText.text = "LETS MOVE!";
        TimedObjects.SetActive(true);
        Points.StartGeneration();
    }
    

    public void Update()
    {
        HealthDisplay.text = Convert.ToString(Health);

        if (StopMovingTimer)
        {
            StopMovingTime += Time.deltaTime;
            if (StopMovingTime >= WantedStopMovingTime)
            {
                IsMoving = false;
                StopMovingTime = 0.0f;
                StopMovingTimer = false;
            }
        }
        
        
        if (Timer)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= wantedTime)
            {
                Timer = false;
                currentTime = 0.0f;
            }
        }

        if (SongTimeActive)
        {
            TimeInSong += Time.deltaTime;
            if (TimeInSong >= 199f && !HasWon)
            {
                HasWon = true;
            }
        }
        
        if (StartTimer)
        {
            currentTimeStart += Time.deltaTime;

            if (currentTimeStart > 9 && ten)
            {
                Audio.PlayOneShot(Clip10);
                ten = false;
                CountdownText.text = "TEN!";
            }
            
            if (currentTimeStart > 10 && nine)
            {
                Audio.PlayOneShot(Clip9);
                nine = false;
                CountdownText.text = "NINE!";
            }
            
            if (currentTimeStart > 11 && eight)
            {
                Audio.PlayOneShot(Clip8);
                eight = false;
                CountdownText.text = "EIGHT!";
            }
            
            if (currentTimeStart > 12 && seven)
            {
                Audio.PlayOneShot(Clip7);
                seven = false;
                CountdownText.text = "SEVEN!";
            }
            
            if (currentTimeStart > 13 && six)
            {
                Audio.PlayOneShot(Clip6);
                six = false;
                CountdownText.text = "SIX!";
            }
            
            if (currentTimeStart > 14 && five)
            {
                Audio.PlayOneShot(Clip5);
                five = false;
                CountdownText.text = "FIVE!";
            }
            
            if (currentTimeStart > 15 && four)
            {
                Audio.PlayOneShot(Clip4);
                four = false;
                CountdownText.text = "FOUR!";
            }
            
            if (currentTimeStart > 16 && three)
            {
                Audio.PlayOneShot(Clip3);
                three = false;
                CountdownText.text = "THREE!";
            }
            
            if (currentTimeStart > 17 && two)
            {
                Audio.PlayOneShot(Clip2);
                two = false;
                CountdownText.text = "TWO!";
            }
            
            if (currentTimeStart > 18 && one)
            {
                Audio.PlayOneShot(Clip1);
                one = false;
                CountdownText.text = "ONE!";
            }


            if (currentTimeStart >= wantedTimeStart)
            {
                StartTimer = false;
                currentTimeStart = 0.0f;
                IsMoving = true;
                
                ten = true;
                nine = true;
                eight = true;
                seven = true;
                six = true;
                five = true;
                four = true;
                three = true;
                two = true;
                one = true;

                StartRound();
            }
        }
        
        
        if (!IsMoving)
        {
            for (int i = 0; i < Animators.Length; i++)
            {
                Animators[i].SetBool("Moving", false);
            }
        }
        else
        {
            for (int i = 0; i < Animators.Length; i++)
            {
                Animators[i].SetBool("Moving", true);
            }
        }
        
        if (!IsMaster || !IsMoving) return;

        if (Vector3.Distance(transform.position, Agent.destination) < 0.25f && !Timer)
        {
            Timer = true;

            if (currentTarget == Waypoints.Length - 1) return;
            Agent.SetDestination(Waypoints[currentTarget + 1].position);
            currentTarget++;
        }

        if(Vector3.Distance(transform.position, Waypoints[(Waypoints.Length-1)].position) < 10f && !VictoryStuff.activeSelf) SendCustomNetworkEvent(NetworkEventTarget.All, "Victory");
        
        if (Health <= 0.0f && !DeathStuff.activeSelf) SendCustomNetworkEvent(NetworkEventTarget.All, "Die");

    }

    public void Victory()
    {
        VictoryStuff.SetActive(true);

        StopMovingTimer = true;

        if (!Networking.LocalPlayer.isMaster) return;
        foreach (var Controller in AIControllers)
        {
            Controller.KillImmediate();
        }
        
        foreach (var Audio in SongSelection)
        {
            Audio.SetActive(false);
        }
    }

    public void Die()
    {
        DeathStuff.SetActive(true);
        gameObject.SetActive(false);
        
        if (!Networking.LocalPlayer.isMaster) return;
        foreach (var Controller in AIControllers)
        {
            Controller.KillAll();
        }
        foreach (var Audio in SongSelection)
        {
            Audio.SetActive(false);
        }
    }
}
