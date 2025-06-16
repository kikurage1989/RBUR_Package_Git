using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SignalTrasnport_onEditor : IProcessSceneWithReport
{
    public int callbackOrder => 0;

    public void OnProcessScene(Scene scene, BuildReport report)
    {
        //AbstractSignalTrasnport‚ÌQÆ“¯Šú—p‚ÉSignalTrasnportManager‚ÉID‚ğ•Û‘¶
    }
}
