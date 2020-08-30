
using UdonSharp;
using UnityEngine;
using UnityEngine.AI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class ExploderAI : UdonSharpBehaviour
{
    public CoffinDanceAI Payload;
    public GlobalSettingsManager Settings;
    public Transform Target;
    public NavMeshAgent Agent;

    public AudioSource Audio;
    public AudioClip[] HighQualityContent;
    public AudioClip DeathClip;
    public AudioClip ExplodeClip;
    
    public GameObject ExplosionPrefab;

    [UdonSynced] public float Health = 50f;

    private bool Dead;

    private float currentTime;
    private float wantedTime = 2.0f;

    void Start()
    {

    }

    public void Update()
    {
        Agent.SetDestination(Target.position);

        if (Health <= 0.0f && !Dead)
        {
            Die();
            Dead = false;
        }

        currentTime += Time.deltaTime;
        if (currentTime >= wantedTime)
        {
            currentTime = 0.0f;
            Audio.PlayOneShot(HighQualityContent[Random.Range(0, HighQualityContent.Length - 1)]);
        }
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
            
        }
        else if (col.gameObject.layer == 23 && Networking.LocalPlayer.isMaster) 
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "NetworkedExplode");
        }
    }
    
    public void HitPistol()
    {
        Health -= 100;
    }
    
    public void HitTripleBlast()
    {
        Health -= 70;
    }
    
    public void HitChargeBlaster()
    {
        Health -= 300;
    }

    public void Die()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, "NetworkedDie");
        gameObject.SetActive(false);
    }

    public void NetworkedDie()
    {
        var Explosion = VRCInstantiate(ExplosionPrefab);
        Explosion.transform.SetPositionAndRotation(transform.position, transform.rotation);
        Destroy(Explosion, 2);
        Audio.PlayOneShot(DeathClip);
        Agent.enabled = false;
        gameObject.SetActive(false);

    }
    
    

    public void NetworkedExplode()
    {
        var Explosion = VRCInstantiate(ExplosionPrefab);
        Explosion.transform.SetPositionAndRotation(transform.position, transform.rotation);
        Destroy(Explosion, 2);
        Audio.PlayOneShot(ExplodeClip);
        Payload.Health -= 5f;
        Agent.enabled = false;
        gameObject.SetActive(false);

    }
}
