
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class EnemyAI : UdonSharpBehaviour
{
    public int AIIdentifier;

    public bool HasDied;
    
    public CoffinDanceAI CoffinAI;
    public GlobalSettingsManager Settings;
    public Transform Target;
    public Transform Coffin;
    public Transform AI;
    public Animator AIAnimator;
    public GameObject ExplosionPrefab;
    public GameObject HitPrefab;
    public Transform LaserOrigin;
    public GameObject LaserPrefab;
    public AudioSource Audio;
    public AudioClip FireClip;
    public AudioClip[] DeathClip;

    public float Health = 100f;

    public float DamagePerShot = 1f;

    public int NumberOfRandomPositions = 6;

    private bool Timer;
    private float currentTime;
    private float wantedTime = 6.5f;

    private bool Timer2;
    private float currentTime2;
    public float ReloadTime = 1f;

    private bool MoveTimer;
    private float currentTimeMove;
    private float wantedTimeMove = 5f;

    private bool First = true;
    private bool Dead;

    public Transform CleanUpSystem;

    public bool IsAssigning;
    private bool IsMaster;
    private float PreviousHealth;
    private bool IsFirst = true;
    private bool IsFirstOther = false;

    #region Main Functions

    #region Start Functions
    public void Start()
    {
        if (IsAssigning) return;
        if (!First) return;
        
        //DamagePerShot *= Settings.DamageDampening;

        if (Networking.LocalPlayer.isMaster)
        {
            if (Settings.ModifyEnemyHealth)
            {
                switch (Settings.DifficultyRating)
                {
                    case 0:
                        if (AIIdentifier == 0)
                        {
                            Health = Settings.EasyDroneHealth;
                        }
                
                        if (AIIdentifier == 1)
                        {
                            Health = Settings.EasyBigDroneHealth;
                        }
                
                        if (AIIdentifier == 2)
                        {
                            Health = Settings.EasyMassiveDroneHealth;
                        }
                        break;
                    case 1:
                        if (AIIdentifier == 0)
                        {
                            Health = Settings.DefaultDroneHealth;
                        }
                
                        if (AIIdentifier == 1)
                        {
                            Health = Settings.DefaultBigDroneHealth;
                        }
                
                        if (AIIdentifier == 2)
                        {
                            Health = Settings.DefaultMassiveDroneHealth;
                        }
                        break;
                    case 2:
                        if (AIIdentifier == 0)
                        {
                            Health = Settings.HardDroneHealth;
                        }
                
                        if (AIIdentifier == 1)
                        {
                            Health = Settings.HardBigDroneHealth;
                        }
                
                        if (AIIdentifier == 2)
                        {
                            Health = Settings.HardMassiveDroneHealth;
                        }
                        break;
            
                }
            }
            else
            {
                if (AIIdentifier == 0)
                {
                    Health = Settings.DefaultDroneHealth;
                }
                
                if (AIIdentifier == 1)
                {
                    Health = Settings.DefaultBigDroneHealth;
                }
                
                if (AIIdentifier == 2)
                {
                    Health = Settings.DefaultMassiveDroneHealth;
                }
            }

            IsMaster = true;
        }

        
        AIAnimator.enabled = true;
        Timer = true;
        var random = Random.Range(0, 4);
        AIAnimator.SetInteger("EntrancePath", random);
    }
    
    public void OnEnable()
    {
        if (IsAssigning) return;
        if (First) return;

        if (Networking.LocalPlayer.isMaster)
        {
            if (Settings.ModifyEnemyHealth)
            {
                switch (Settings.DifficultyRating)
                {
                    case 0:
                        if (AIIdentifier == 0)
                        {
                            Health = Settings.EasyDroneHealth;
                        }
                
                        if (AIIdentifier == 1)
                        {
                            Health = Settings.EasyBigDroneHealth;
                        }
                
                        if (AIIdentifier == 2)
                        {
                            Health = Settings.EasyMassiveDroneHealth;
                        }
                        break;
                    case 1:
                        if (AIIdentifier == 0)
                        {
                            Health = Settings.DefaultDroneHealth;
                        }
                
                        if (AIIdentifier == 1)
                        {
                            Health = Settings.DefaultBigDroneHealth;
                        }
                
                        if (AIIdentifier == 2)
                        {
                            Health = Settings.DefaultMassiveDroneHealth;
                        }
                        break;
                    case 2:
                        if (AIIdentifier == 0)
                        {
                            Health = Settings.HardDroneHealth;
                        }
                
                        if (AIIdentifier == 1)
                        {
                            Health = Settings.HardBigDroneHealth;
                        }
                
                        if (AIIdentifier == 2)
                        {
                            Health = Settings.HardMassiveDroneHealth;
                        }
                        break;
            
                }
            }
            else
            {
                if (AIIdentifier == 0)
                {
                    Health = Settings.DefaultDroneHealth;
                }
                
                if (AIIdentifier == 1)
                {
                    Health = Settings.DefaultBigDroneHealth;
                }
                
                if (AIIdentifier == 2)
                {
                    Health = Settings.DefaultMassiveDroneHealth;
                }
            }

            IsMaster = true;
        }


        AIAnimator.enabled = true;
        Timer = true;
        var random = Random.Range(0, 4);
        AIAnimator.SetInteger("EntrancePath", random);
    }
    
    #endregion

    public void Update()
    {
        if (IsAssigning) return;

        var position = AI.position;
        position += (Target.position - position) * 0.05f;
        AI.position = position;
        
        AI.LookAt(Coffin);
/*
        if (Settings.InstaDeath && IsMaster && IsFirst)
        {
            PreviousHealth = Health;
            Health = 1f;
            IsFirst = false;
            IsFirstOther = true;
        }
        else if (!Settings.InstaDeath && IsMaster && IsFirstOther)
        {
            IsFirst = true;
            IsFirstOther = false;
            Health = PreviousHealth;
        }
*/
        #region Timers
        if (Timer)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= wantedTime)
            {
                Timer = false;
                currentTime = 0.0f;
                MoveTimer = true;
                var random = Random.Range(1, NumberOfRandomPositions + 1);
                AIAnimator.SetInteger("Position", random);
                Timer2 = true;
            }
        }

        if (Timer2)
        {
            currentTime2 += Time.deltaTime;
            if (currentTime2 >= ReloadTime)
            {
                InstantiateLaser();
                //SendCustomNetworkEvent(NetworkEventTarget.All, "InstantiateLaser");
                currentTime2 = 0.0f;
                if (Networking.LocalPlayer.isMaster)
                {
                    CoffinAI.Health -= DamagePerShot;
                }
            }
        }

        if (MoveTimer)
        {
            currentTimeMove += Time.deltaTime;
            if (currentTimeMove >= wantedTimeMove)
            {
                var random = Random.Range(1, NumberOfRandomPositions + 1);
                AIAnimator.SetInteger("Position", random);
                currentTimeMove = 0.0f;
            }
        }
        #endregion

        if (Health <= 0.0f && !Dead && IsMaster)
        {
            Die();
            Dead = true;
        }
    }
    


    #region Death Functions
    public void Die()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, "NetworkedDie");
        gameObject.SetActive(false);
    }

    public void NetworkedDie()
    {
        var Explosion = VRCInstantiate(ExplosionPrefab);
        Explosion.transform.SetPositionAndRotation(AI.transform.position, AI.transform.rotation);
        Explosion.transform.SetParent(CleanUpSystem);
        AIAnimator.SetInteger("Position", 0);
        Timer2 = false;
        currentTime2 = 0.0f;
        MoveTimer = false;
        currentTimeMove = 0.0f;
        currentTime = 0.0f;
        Audio.PlayOneShot(DeathClip[Random.Range(0, 2)]);
        gameObject.SetActive(false);

    }
    #endregion

    public void InstantiateLaser()
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
}
