
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class ChargeBlaster : UdonSharpBehaviour
{
    public GlobalPointsSystem Points;
    
    public Transform HitOrb;
    public Transform LaserOrigin;

    public Text Healthbar;
    public CoffinDanceAI Coffin;
    
    public Text Ownerbar;
    public GameObject Line;

    public GameObject Audio;


    private bool Charging;
    private float currentReloadTime;
    public float ChargeTime = 2f;
    

    private GameObject SpawnedObject;


    private bool IsFirstReload = true;
    private bool IsSecondReload = true;

    private VRCPlayerApi Owner;
    
    public void Start()
    {
        LaserOrigin.GetChild(0).gameObject.SetActive(false);
        
    }
    
    public void Update()
    {
        
        
        if (Charging)
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

            if (currentReloadTime >= ChargeTime)
            {
                Charging = false;
                currentReloadTime = 0f;
                SendCustomNetworkEvent(NetworkEventTarget.All, "Fire");
                
            }
        }
        else
        {
            HitOrb.gameObject.SetActive(false);
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
        Charging = true;
        Audio.SetActive(true);
    }

    public void OnPickupUseUp()
    {
        Charging = false;
        currentReloadTime = 0f;
        Audio.SetActive(false);
    }

    public void Fire()
    {
        SpawnedObject = VRCInstantiate(Line);
        SpawnedObject.transform.SetPositionAndRotation(LaserOrigin.position, LaserOrigin.rotation);
        
        Destroy(SpawnedObject, 1);
        HitOrb.gameObject.SetActive(true);

        IsFirstReload = true;
        IsSecondReload = true;

    }

    public void ResetHitOrb()
    {
        HitOrb.gameObject.SetActive(false);
    }
    
}
