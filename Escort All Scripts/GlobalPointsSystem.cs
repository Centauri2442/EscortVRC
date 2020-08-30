
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class GlobalPointsSystem : UdonSharpBehaviour
{
    public int Points;
    public Text PointsDisplay;

    public GlobalSettingsManager Settings;

    private bool Generate;
    private float currentTime;
    private float wantedTime = 5f;
    public int PointsPerCycle = 5;

    public void AddPoints(int points)
    {
        Points += points;
    }

    public void RemovePoints(int points)
    {
        Points -= points;
    }

    public void StartGeneration()
    {
        Generate = true;
    }

    public void Update()
    {
        if (Generate)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= wantedTime)
            {
                Points += PointsPerCycle;
                currentTime = 0.0f;
            }
        }
    }
}
