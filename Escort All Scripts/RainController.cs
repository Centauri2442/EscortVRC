
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RainController : UdonSharpBehaviour
{
    public bool IsOutside;

    public Animator Transition;

    private bool FirstOutside = true;
    private bool FirstInside = true;

    public void Update()
    {
        if (IsOutside && FirstOutside)
        {
            FirstInside = true;
            FirstOutside = false;
            Transition.SetBool("IsRaining", true);
        }
        else if(FirstInside)
        {
            FirstInside = false;
            FirstOutside = true;
            Transition.SetBool("IsRaining", false);
        }
    }
}
