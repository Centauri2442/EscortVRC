
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Frigate : UdonSharpBehaviour
{
    public GameObject[] Ring;

    private float currentTime;
    public float WaitTime = 8f;

    private bool HasFired;
    
    public void Update()
    {
        if (HasFired) return;

        currentTime += Time.deltaTime;
        if (currentTime >= WaitTime)
        {
            foreach (var obj in Ring)
            {
                obj.SetActive(true);
            }

            HasFired = true;
        }
    }
}
