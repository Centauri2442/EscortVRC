
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class Blaster : UdonSharpBehaviour
{
    [UdonSynced] public bool GunActive;
    public GameObject PlayerCapsule;
    public Transform HitOrb;
    public Transform LaserOrigin;
    public Transform RespawnPoint;
    public Text Healthbar;
    public Text Ownerbar;
    public string OwnerName;
    public LineRenderer Line;
    public int Team;

    public AudioSource Audio;
    public AudioClip ReloadClip;
    public AudioClip DieClip;
    public AudioClip HitClip;
    public AudioClip FireClip;

    private bool Reloading;
    private float currentReloadTime;
    public float ReloadTime = 1f;


    private VRCPlayerApi Owner;
    private float Health = 100f;

    private bool OwnershipTimer;
    private float currentTime;
    private float wantedTime = 1f;

    public void Start()
    {
        Owner = Networking.LocalPlayer;
        GunActive = false;
    }

    public void Update()
    {
        if (GunActive)
        {
            PlayerCapsule.transform.position = Owner.GetPosition();
        }

        if (OwnershipTimer)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= wantedTime)
            {
                OwnershipTimer = false;
                currentTime = 0.0f;
                PlayerCapsule.GetComponent<Capsule>().SetTeam();
                OwnerName = Owner.displayName;
                Ownerbar.text = OwnerName;
            }
        }

        if (Health <= 0f)
        {
            CommitNotAlive();
        }

        if (Reloading)
        {
            currentReloadTime += Time.deltaTime;
            if (currentReloadTime * 2 >= ReloadTime)
            {
                Line.SetPosition(1, LaserOrigin.position);
            }
            
            if (currentReloadTime >= ReloadTime)
            {
                Reloading = false;
                currentReloadTime = 0f;
                Audio.PlayOneShot(ReloadClip);
            }
        }

        Healthbar.text = Convert.ToString(Health);
    }

    public void OnPickup()
    {
        Owner = Networking.LocalPlayer;
        Networking.SetOwner(Owner, gameObject);
        Networking.SetOwner(Owner, PlayerCapsule);
        OwnershipTimer = true;
        GunActive = true;
    }

    public void OnPickupUseDown()
    {
        if (Reloading) return;
        SendCustomNetworkEvent(NetworkEventTarget.All, "Fire");
    }

    public void OnDrop()
    {
        GunActive = false;
        OwnershipTimer = false;
        currentTime = 0.0f;
    }

    public void Fire()
    {
        RaycastHit hit;
        if (Physics.Raycast(LaserOrigin.position, LaserOrigin.forward, out hit, 40f) && hit.transform.gameObject.layer == 23 && hit.transform.GetComponent<Capsule>().Team != Team)
        {
            Line.SetPosition(0, LaserOrigin.position);
            Line.SetPosition(1, hit.point);
            HitOrb.position = hit.point;
            Reloading = true;
            Audio.PlayOneShot(FireClip);
        }
    }

    public void GetShot()
    {
        Health -= 20f;
        Audio.PlayOneShot(HitClip);
    }

    public void CommitNotAlive()
    {
        Health = 100f;
        Owner.TeleportTo(RespawnPoint.position, RespawnPoint.rotation);
        Audio.PlayOneShot(DieClip);
    }
}
