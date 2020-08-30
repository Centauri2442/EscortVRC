
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class BlasterGun : UdonSharpBehaviour
{
    public GlobalPointsSystem Points;
    
    public Transform HitOrb;
    public Transform LaserOrigin;

    public Text Healthbar;
    public CoffinDanceAI Coffin;
    
    public Text Ownerbar;
    public GameObject Line;

    public AudioSource Audio;
    public AudioClip FireClip;


    private bool Reloading;
    private float currentReloadTime;
    public float ReloadTime = 1f;

    private bool NetworkTimer;
    private float currentNetworkTime;
    private float wantedNetworkTime = 0.5f;

    private GameObject SpawnedObject;


    private bool IsFirstReload = true;
    private bool IsSecondReload = true;

    private VRCPlayerApi Owner;

    public void Start()
    {
        LaserOrigin.GetChild(0).gameObject.SetActive(false);
        if (Points.Settings.DifficultyRating == 0)
        {
            wantedNetworkTime = 0.5f;
        }
        else if (Points.Settings.DifficultyRating == 1)
        {
            wantedNetworkTime = 1f;
        }
        else
        {
            wantedNetworkTime = 2f;
        }
    }
    
    public void Update()
    {
        
        
        if (Reloading)
        {
            currentReloadTime += Time.deltaTime;

            if (IsSecondReload && !IsFirstReload)
            {
                ResetHitOrb();
                IsSecondReload = false;
            }

            if (IsFirstReload)
            {
                IsFirstReload = false;
            }

            if (currentReloadTime >= ReloadTime)
            {
                Reloading = false;
                currentReloadTime = 0f;
                Destroy(SpawnedObject, 1);
            }
        }

        if (NetworkTimer)
        {
            currentNetworkTime += Time.deltaTime;
            if (currentNetworkTime >= wantedNetworkTime)
            {
                NetworkTimer = false;
                currentNetworkTime = 0.0f;
            }
        }

        Healthbar.text = Convert.ToString(Points.Points);
    }

    public void OnPickup()
    {
        Owner = Networking.LocalPlayer;
        Ownerbar.text = Owner.displayName;
        LaserOrigin.GetChild(0).gameObject.SetActive(true);
    }

    public void OnDrop()
    {
        LaserOrigin.GetChild(0).gameObject.SetActive(false);
    }

    public void OnPickupUseDown()
    {
        if (Reloading) return;
        HitOrb.gameObject.SetActive(true);

        if (NetworkTimer)
        {
            Fire();
        }
        else
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "Fire");
            NetworkTimer = true;
        }
    }

    public void Fire()
    {
        SpawnedObject = VRCInstantiate(Line);
        SpawnedObject.transform.SetPositionAndRotation(LaserOrigin.position, LaserOrigin.rotation);
        

        IsFirstReload = true;
        IsSecondReload = true;
        
        Reloading = true;
        Audio.PlayOneShot(FireClip);
        
    }

    public void ResetHitOrb()
    {
        HitOrb.gameObject.SetActive(false);
    }
    
}
