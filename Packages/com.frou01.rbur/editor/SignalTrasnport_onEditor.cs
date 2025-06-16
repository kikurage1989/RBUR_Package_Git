using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SignalTrasnport_onEditor : IProcessSceneWithReport
{
    public int callbackOrder => 0;
    public List<AbstractSignalTrasnport> target = new List<AbstractSignalTrasnport>();

    public void OnProcessScene(Scene scene, BuildReport report)
    {
        foreach (AbstractSignalTrasnport transporter in target)
        {
            if (transporter.connectedTransporter[0]) transporter.changeConnection(false, transporter.connectedTransporter[0], transporter.connectedFace[0]);
            if (transporter.connectedTransporter[1]) transporter.changeConnection(true , transporter.connectedTransporter[1], transporter.connectedFace[1]);
            //‰Šú‰»
        }
        //AbstractSignalTrasnport‚ÌQÆ“¯Šú—p‚ÉSignalTrasnportManager‚ÉID‚ğ•Û‘¶
    }

    void Proceed(Transform parent)
    {
        if (parent.gameObject.GetComponent<AbstractSignalTrasnport>() != null)
        {
            target.Add(parent.gameObject.GetComponent<AbstractSignalTrasnport>());
        }
        foreach (Transform obj in parent)
        {
            Proceed(obj);
        }
    }

}
