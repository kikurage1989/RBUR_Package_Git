
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SignalTransportTester : UdonSharpBehaviour
{
    [SerializeField] UdonBehaviour target;
    void Start()
    {
        
    }

    public void Update()
    {
        target.SendCustomEvent("_interact");
    }
}
