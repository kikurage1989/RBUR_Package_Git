
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AbstractSignalTrasnport : UdonSharpBehaviour
{
    //Face true = +Z
    //Face false = -Z
    //Reference
    //参照した前後方向のTransporterの各変数を配列保存して高速参照する
    //---------------------------------------------------------------------------------------------ReferBlock
    [SerializeField]public AbstractSignalTrasnport[] connectedTransporter = new AbstractSignalTrasnport[2];
    int[] localConnectedTransporterID = new int[2];
    [UdonSynced] int[] syncedConnectedTransporterID = new int[2];
    bool[][] connectedTransporter_UpdateComingFace = new bool[2][];
    public virtual void changeConnection(bool ConnectingFace, AbstractSignalTrasnport newConnection,bool FaceofNewConnection)
    {
        int id = ConnectingFace ? 1 : 0;
        connectedTransporter[id] = newConnection;
        if(newConnection)
        {
            syncedConnectedTransporterID[id] = localConnectedTransporterID[id] = connectedTransporter[id].SignalTransporterID;
            //store pointer connectedTransporter[id].comingUpdateFace;
            connectedTransporter_UpdateComingFace[id] = connectedTransporter[id].UpdateComingFace;
            connectedFace[id] = FaceofNewConnection;
        }
        else
        {
            syncedConnectedTransporterID[id] = localConnectedTransporterID[id] = -1;
        }
    }
    //---------------------------------------------------------------------------------------------ReferBlockEnd

    [SerializeField] public bool[] connectedFace = new bool[2];

    //Preset
    [SerializeField] AbstractSignalProcessor signalProcessor;
    public int SignalTransporterID;

    bool alreadySignalUpdated;
    //Temporary
    public bool[] UpdateComingFace = new bool[1];

    int counter;

    protected virtual void Start()
    {
    }

    public void LateUpdate()
    {
        alreadySignalUpdated = false;
        counter = 0;
    }



    public virtual void TransportSignal(bool f_r)
    {
        int id = f_r?1:0;
        //connectedTransporter[id].UpdateComingFace[0] = connectedFace[id];
        connectedTransporter_UpdateComingFace[id][0] = connectedFace[id];
        connectedTransporter[id].OnSpawn();
        Debug.Log("UpdateSignal");
    }

    public virtual bool SignalUpdate()
    {
        return false;
    }
    public override void OnSpawn()
    {
        counter++;
        if(counter > 500)
        {
            Debug.LogError("Infinite loop or too long train!");
            return;
        }
        //sendCustomEventを使用せず、使わないだろうPublicAPIで代替してやる
        if (!alreadySignalUpdated && SignalUpdate())
        {
            TransportSignal(UpdateComingFace[0]);//自身が優先信号を持っている場合は打ち返す。既に自身の信号を発出済みの場合は無視する
        }
        TransportSignal(!UpdateComingFace[0]);//来たのとは反対側へ信号を飛ばす
    }
}
