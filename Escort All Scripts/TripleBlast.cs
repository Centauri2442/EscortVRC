
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class TripleBlast : UdonSharpBehaviour
{
    public GlobalPointsSystem Points;
    
    public Transform HitOrb;
    public Transform HitOrb2;
    public Transform HitOrb3;
    
    public Transform LaserOrigin;
    public Transform LaserOrigin2;
    public Transform LaserOrigin3;
    
    public Text Healthbar;
    public CoffinDanceAI Coffin;
    
    public Text Ownerbar;
    public GameObject Line;

    public AudioSource Audio;
    public AudioClip FireClip;


    private bool Reloading;
    private float currentReloadTime;
    public float ReloadTime = 1f;

    private GameObject SpawnedObject;
    private GameObject SpawnedObject2;
    private GameObject SpawnedObject3;


    private bool IsFirstReload = true;
    private bool IsSecondReload = true;
    
    private bool NetworkTimer;
    private float currentNetworkTime;
    private float wantedNetworkTime = 0.5f;

    private VRCPlayerApi Owner;

    public void Start()
    {
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
                SendCustomNetworkEvent(NetworkEventTarget.All, "ResetHitOrb");
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
                Destroy(SpawnedObject2, 1);
                Destroy(SpawnedObject3, 1);
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
    }

    public void OnPickupUseDown()
    {
        if (Reloading) return;
        
        HitOrb.gameObject.SetActive(true);
        HitOrb2.gameObject.SetActive(true);
        HitOrb3.gameObject.SetActive(true);

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
        SpawnedObject2 = VRCInstantiate(Line);
        SpawnedObject2.transform.SetPositionAndRotation(LaserOrigin2.position, LaserOrigin2.rotation);
        SpawnedObject3 = VRCInstantiate(Line);
        SpawnedObject3.transform.SetPositionAndRotation(LaserOrigin3.position, LaserOrigin3.rotation);
        

        IsFirstReload = true;
        IsSecondReload = true;
        
        Reloading = true;
        Audio.PlayOneShot(FireClip);
        
    }

    public void ResetHitOrb()
    {
        HitOrb.gameObject.SetActive(false);
        HitOrb2.gameObject.SetActive(false);
        HitOrb3.gameObject.SetActive(false);
    }

}
