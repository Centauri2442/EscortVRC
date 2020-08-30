
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RainTrigger : UdonSharpBehaviour
{
    public bool State;
    
    public Animator Controller;
    
    public void OnTriggerStay(Collider col)
    {
        if (col == null || col.gameObject.layer != 10) return;
        
        Debug.Log(transform.InverseTransformPoint(col.transform.position).z);

        if (transform.InverseTransformPoint(col.transform.position).z > 0)
        {
            State = true;
        }
        else
        {
            State = false;
        }
        Controller.SetBool("IsRaining", State);
        
    }
}
