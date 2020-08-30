
using UdonSharp;
using UnityEngine;
using UnityEngine.AI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class TurretAI : UdonSharpBehaviour
{
    public CoffinDanceAI CoffinAI;
    public GlobalSettingsManager Settings;
    public Transform Target;
    public AudioSource Audio;
    public AudioClip DeathClip;
    public AudioClip FireClip;
    public GameObject ExplosionPrefab;
    public GameObject HitPrefab;
    public Transform CleanUpSystem;

    public int MaxShots = 2;
    private int ShotsFired;

    private bool Dead;
    
    public float Health = 100f;

    public float DamagePerShot = 1f;

    public Transform Swivel;
    public Transform LaserOrigin;
    public GameObject LaserPrefab;

    private bool Timer;
    private float currentTime;
    public float ReloadTime = 1f;

    public void Start()
    {
        DamagePerShot *= Settings.DamageDampening;
        
        if (Settings.ModifyEnemyHealth)
        {
            switch (Settings.DifficultyRating)
            {
                case 0:
                    Health = Settings.EasyTurretCartHealth;
                    break;
                case 1:
                    Health = Settings.DefaultTurretCartHealth;
                    break;
                case 2:
                    Health = Settings.HardTurretCartHealth;
                    break;
            
            }
        }
        else
        {
            Health = Settings.DefaultTurretCartHealth;
        }

        Timer = true;
    }

    public void Update()
    {
        Swivel.LookAt(Target);

        if (Timer && !Dead && transform.GetChild(0).gameObject.activeSelf)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= ReloadTime && ShotsFired <= MaxShots && !Dead)
            {
                InstantiateLaser();
                currentTime = 0.0f;
                if (Networking.LocalPlayer.isMaster)
                {
                    CoffinAI.Health -= DamagePerShot;
                }

                ShotsFired++;
            }
        }

        if (!Networking.LocalPlayer.isMaster) return;
        
        if (Health <= 0.0f && !Dead)
        {
            Die();
            Dead = true;
        }
    }
    
    public void Die()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, "NetworkedDie");
        Dead = true;
    }

    public void NetworkedDie()
    {
        var Explosion = VRCInstantiate(ExplosionPrefab);
        Explosion.transform.SetPositionAndRotation(transform.position, transform.rotation);
        Destroy(Explosion, 2);
        Audio.PlayOneShot(DeathClip);
        transform.GetChild(0).gameObject.SetActive(false);
        Dead = true;
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col == null) return;

        if (col.gameObject.layer == 26)
        {
            Settings.PointsSystem.AddPoints(Settings.TurretCartPointsPerHit);
            
            if (col.gameObject.name.Contains("Pistol"))
            {
                SendCustomNetworkEvent(NetworkEventTarget.Owner, "HitPistol");
            }
            else if (col.gameObject.name.Contains("TripleBlast"))
            {
                SendCustomNetworkEvent(NetworkEventTarget.Owner, "HitTripleBlast");
            }
            else if (col.gameObject.name.Contains("ChargeBlaster"))
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, "HitChargeBlaster");
            }
            
            var Hit = VRCInstantiate(HitPrefab);
            Hit.transform.SetPositionAndRotation(transform.position, transform.rotation);
            Hit.transform.SetParent(CleanUpSystem);
        }
    }
    
    public void InstantiateLaser()
    {
        var laser = VRCInstantiate(LaserPrefab);
        laser.transform.SetPositionAndRotation(LaserOrigin.position, LaserOrigin.rotation);
        laser.transform.SetParent(CleanUpSystem);
        Audio.PlayOneShot(FireClip);
    }
    
    public void HitPistol()
    {
        Health -= 50;
    }
    
    public void HitTripleBlast()
    {
        Health -= 35;
    }
    
    public void HitChargeBlaster()
    {
        Health -= 300;
    }
}
