using System.Collections;
using System.Collections.Generic;
using UdonSharpEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRC.SDKBase.Editor.BuildPipeline;
using VRC.Udon;

public class SignalTrasnport_onEditor : IProcessSceneWithReport, IVRCSDKBuildRequestedCallback
{
    public int callbackOrder => 0;

    public void OnProcessScene(Scene scene, BuildReport report)
    {

        Debug.Log("ST_onEditor");
        List<AbstractSignalTrasnporter> transportList = new List<AbstractSignalTrasnporter>();
        //SignalTrasnportManager STM = null;
        foreach (GameObject obj in scene.GetRootGameObjects())
        {
            Lisiting_Transporter(obj.transform,transportList);
        //    Find_SignalTrasnportManager(obj.transform, out STM);
        }
        //STM.SignalTrasnports = transportList.ToArray();
        AbstractSignalTrasnporter[] SignalTrasnports = transportList.ToArray();
        int idCnt = 0;
        foreach (AbstractSignalTrasnporter transporter in SignalTrasnports)
        {
            Debug.Log("SignalTransporter " + transporter.gameObject.name);
            transporter.SignalTrasnports = SignalTrasnports;
            transporter.SignalTransporterID = idCnt;
            //if(transporter.eventProxy1 == null)
            //{
            //    GameObject eventProxy1 = new GameObject("eventProxy1");
            //    Toggle eventProxy1_Toggle = eventProxy1.AddComponent<Toggle>();
            //    foreach(UdonBehaviour attachedUdons in transporter.GetComponents<UdonBehaviour>())
            //    {
                    
            //        if (UdonSharpEditorUtility.GetUdonSharpBehaviourType(attachedUdons).IsInstanceOfType(transporter))
            //        {
            //            Debug.Log("SignalTransporter UB " + UdonSharpEditorUtility.GetUdonSharpBehaviourType(attachedUdons));
            //            UnityAction<bool> action = (bool b) =>
            //            {
            //                attachedUdons.OnPickup();
            //            };
            //            eventProxy1_Toggle.onValueChanged.AddListener(action);
            //        }
            //    }
            //    eventProxy1.transform.SetParent(transporter.transform);
            //    eventProxy1.transform.localPosition = Vector3.zero;
            //    eventProxy1.transform.localRotation = Quaternion.identity;
            //    transporter.eventProxy1 = eventProxy1_Toggle;
            //}
            idCnt++;
        }
        //AbstractSignalTrasnporterÇÃéQè∆ìØä˙ópÇ…SignalTrasnportManagerÇ…IDÇï€ë∂
    }

    void Find_SignalTrasnportManager(Transform parent, out SignalTrasnportManager STM)
    {
        STM = null;

        STM = parent.gameObject.GetComponent<SignalTrasnportManager>();
        if (STM) return;
        STM = parent.gameObject.GetComponentInChildren<SignalTrasnportManager>(true);
    }
    void Lisiting_Transporter(Transform parent, List<AbstractSignalTrasnporter> transportList)
    {
        if (parent.gameObject.GetComponent<AbstractSignalTrasnporter>() != null)
        {
            transportList.Add(parent.gameObject.GetComponent<AbstractSignalTrasnporter>());
        }
        foreach (Transform obj in parent)
        {
            Lisiting_Transporter(obj, transportList);
        }
    }

    public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
    {
        return true;
    }

}
