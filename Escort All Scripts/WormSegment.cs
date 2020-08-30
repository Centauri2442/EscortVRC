using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

public class WormSegment : UdonSharpBehaviour
{
    public Transform CleanUpSystem;
    public CoffinDanceAI CoffinAI;
    public WormController Controller;
    public Transform AITarget;
    public GlobalSettingsManager Settings;
    public Transform Target;
    public Transform AI;
    public GameObject DeathPrefab;
    public GameObject ExplosionPrefab;
    public GameObject HitPrefab;
    public Transform LaserOrigin;
    public GameObject LaserPrefab;
    public AudioSource Audio;
    public AudioClip FireClip;
    public AudioClip[] DeathClip = new AudioClip[2];
    public AudioClip HitClip;

    public float Health = 750f;

    public float DamagePerShot = 1f;
    private bool CanFire;
    private float currentTime;
    public float FireRate = 2f;

    private float DistanceFromPayload;
    public float Range = 20f;

    public bool Dead;

    private bool FirstFire = true;
    
    public Color DeadColor;
    public MeshRenderer TargetMesh;


    public void Start()
    {
        DamagePerShot *= Settings.DamageDampening;
        if (Settings.ModifyEnemyHealth)
        {
            switch (Settings.DifficultyRating)
            {
                case 0:
                    Health = Settings.EasyDestroyerHealth;
                    break;
                case 1:
                    Health = Settings.DefaultDestroyerHealth;
                    break;
                case 2:
                    Health = Settings.HardDestroyerHealth;
                    break;
            
            }
        }
        else
        {
            Health = Settings.DefaultDestroyerHealth;
        }
    }
    
    public void Update()
    {
        var position = AI.position;
        position += (Target.position - position) * 0.05f;
        AI.position = position;

        AI.LookAt(Target);

        if (!Networking.LocalPlayer.isMaster || Dead) return;
        
        
        if (Controller.DistanceFromTarget <= Range && AI.position.y >= AITarget.position.y)
        {
            CanFire = true;
            if (FirstFire)
            {
                Fire();
                CoffinAI.Health -= DamagePerShot;
                FirstFire = false;
            }
        }
        else
        {
            CanFire = false;
            FirstFire = true;
        }

        if (CanFire)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= FireRate)
            {
                currentTime = 0.0f;
                Fire();
                CoffinAI.Health -= DamagePerShot;
            }
        }

        if (Health <= 0.0f)
        {
            Die();
        }
    }

    #region Fire Functions
    public void Fire()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, "NetworkFire");
    }

    public void NetworkFire()
    {
        var laser = VRCInstantiate(LaserPrefab);
        laser.transform.SetPositionAndRotation(LaserOrigin.position, LaserOrigin.rotation);
        laser.transform.SetParent(CleanUpSystem);
        Audio.PlayOneShot(FireClip);
    }
    #endregion
    
    #region Hit Functions

    public void HitPistol()
    {
        Health -= 50;
    }
    
    public void HitTripleBlast()
    {
        Health -= 50;
    }
    
    public void HitChargeBlaster()
    {
        Health -= 800;
    }
    
    #endregion
    
    #region Death Functions
    
    public void Die()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, "NetworkedDie");
    }

    public void NetworkedDie()
    {
        var Explosion = VRCInstantiate(DeathPrefab);
        Explosion.transform.SetPositionAndRotation(AI.transform.position, AI.transform.rotation);
        Explosion.transform.SetParent(CleanUpSystem);
        CanFire = false;
        currentTime = 0.0f;
        Audio.PlayOneShot(DeathClip[0]);
        Dead = true;
        TargetMesh.material.EnableKeyword("_EMISSION");
        TargetMesh.material.SetColor("_EmissionColor", DeadColor);
    }

    public void Explode()
    {
        var Explosion = VRCInstantiate(ExplosionPrefab);
        Explosion.transform.SetPositionAndRotation(AI.transform.position, AI.transform.rotation);
        Explosion.transform.SetParent(CleanUpSystem);
        Audio.PlayOneShot(DeathClip[1]);
        gameObject.SetActive(false);
    }
    
    #endregion
    
}
