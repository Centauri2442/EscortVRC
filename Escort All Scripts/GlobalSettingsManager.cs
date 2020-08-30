
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class GlobalSettingsManager : UdonSharpBehaviour
{
    #region General Settings
    
    [UdonSynced] public int DifficultyRating = 1; // 0 - Easy, 1 - Normal, 2 - Hard

    public bool ModifyCoffinHealth = true;
    public bool ModifyEnemyHealth = true;
    public Dropdown Dropdown;

    private int PreviousDifficulty;

    [UdonSynced] private int PayloadSelection;

    public GlobalPointsSystem PointsSystem;

    public GameObject SettingsMenu;

    public GameObject[] PayloadPreview;

    [Header("Payload Settings")] 
    public CoffinDanceAI[] PayloadController;
    public float PayloadEasyHealth = 200;
    public float PayloadDefaultHealth = 150;
    public float PayloadHardHealth = 100;

    [Header("Drone Settings")]
    public float EasyDroneHealth = 100f;
    public float DefaultDroneHealth = 200f;
    public float HardDroneHealth = 400f;
    public int PointsPerHit = 2;
    
    [Header("Big Drone Settings")]
    public float EasyBigDroneHealth = 300f;
    public float DefaultBigDroneHealth = 600f;
    public float HardBigDroneHealth = 900f;

    public int BigPointsPerHit = 3;
    
    [Header("Massive Drone Settings")]
    public float EasyMassiveDroneHealth = 1200f;
    public float DefaultMassiveDroneHealth = 2400f;
    public float HardMassiveDroneHealth = 3600f;
    
    public int MassivePointsPerHit = 4;

    [Header("Destroyer Worm Settings")] 
    public float EasyDestroyerHealth = 500f;
    public float DefaultDestroyerHealth = 1000f;
    public float HardDestroyerHealth = 1500f;
    
    [Header("Turret Cart Settings")] 
    public float EasyTurretCartHealth = 50f;
    public float DefaultTurretCartHealth = 100f;
    public float HardTurretCartHealth = 150f;

    public int TurretCartPointsPerHit = 2;
    
    [Header("Buff Settings")]
    public float DamageDampening = 1f;

    //public bool InstaDeath;
    private bool Timer;
    private float currentTime;
    private float wantedTime = 10f;
    
    #endregion

    public void Start()
    {
        if (Networking.LocalPlayer.isMaster)
        {
            SettingsMenu.SetActive(true);
        }
        else
        {
            SettingsMenu.SetActive(false);
        }
    }
    
    public void Update()
    {
        if(Networking.LocalPlayer.isMaster) PayloadSelection = Dropdown.value;

        for (int i = 0; i < PayloadController.Length; i++)
        {
            PayloadController[i].Selection = PayloadSelection;
        }

        for (int i = 0; i < PayloadPreview.Length; i++)
        {
            if(i == PayloadSelection)
            {
                PayloadPreview[i].SetActive(true);
            }
            else
            {
                PayloadPreview[i].SetActive(false);
            }
        }
        
        if (!Networking.LocalPlayer.isMaster) return;
        /*
        if (InstaDeath)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= wantedTime)
            {
                InstaDeath = false;
                currentTime = 0.0f;
            }
        }*/
        
        
    }

    #region Apply Settings
    
    public void ApplySettings()
    {
        SetHealth();
        SendCustomNetworkEvent(NetworkEventTarget.All, "NetworkApply");
    }
    public void NetworkApply()
    {
        foreach (var Controller in PayloadController)
        {
            for (int i = 0; i < Controller.Payloads.Length; i++)
            {
                if (i == Controller.Selection)
                {
                    Controller.Payloads[i].SetActive(true);
                }
                else
                {
                    Controller.Payloads[i].SetActive(false);
                }
            }
        }

        switch (DifficultyRating)
        {
            case 0:
                DamageDampening = 1.0f;
                break;
            case 1:
                DamageDampening = 1.0f;
                break;
            case 2:
                DamageDampening = 1.2f;
                break;
        }
    }
    
    #endregion

    #region Difficulty Settings
    
    public void SetToEasy()
    {
        DifficultyRating = 0;
    }
    public void SetToNormal()
    {
        DifficultyRating = 1;
    }
    public void SetToHard()
    {
        DifficultyRating = 2;
    }
    
    #endregion
    
    #region Payload Settings
    
    public void SetPayloadModifierActive()
    {
        ModifyCoffinHealth = true;
    }
    
    public void SetPayloadModifierInactive()
    {
        ModifyCoffinHealth = false;
    }

    #endregion

    #region Health Settings

    public void SetEnemyModifierActive()
    {
        ModifyEnemyHealth = true;
    }
    
    public void SetEnemyModifierInactive()
    {
        ModifyEnemyHealth = false;
    }
    
    public void SetHealth()
    {
        switch (DifficultyRating)
        {
            case 0:
            {
                if (ModifyCoffinHealth)
                {
                    foreach (var Controller in PayloadController)
                    {
                        Controller.Health = PayloadEasyHealth;
                        Controller.DefaultHealth = PayloadEasyHealth;
                    }
                }

                break;
            }
            case 1:
            {
                if (ModifyCoffinHealth)
                {
                    foreach (var Controller in PayloadController)
                    {
                        Controller.Health = PayloadDefaultHealth;
                        Controller.DefaultHealth = PayloadDefaultHealth;
                    }
                }

                break;
            }
            case 2:
            {
                if (ModifyCoffinHealth)
                {
                    foreach (var Controller in PayloadController)
                    {
                        Controller.Health = PayloadHardHealth;
                        Controller.DefaultHealth = PayloadDefaultHealth;
                    }
                }

                break;
            }
        }
    }

    #endregion
}
