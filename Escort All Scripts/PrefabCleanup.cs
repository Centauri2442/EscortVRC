
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PrefabCleanup : UdonSharpBehaviour
{
    private float currentTime;
    public float CleanUpIterationTime = 5f;

    public void Update()
    {
        if (transform.childCount > 0)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= CleanUpIterationTime)
            {
                currentTime = 0.0f;
                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
            }
        }
        else
        {
            currentTime = 0.0f;
        }
    }
}
