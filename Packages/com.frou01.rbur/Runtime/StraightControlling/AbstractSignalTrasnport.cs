
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AbstractSignalTrasnport : UdonSharpBehaviour
{
    //Reference
    //参照した前後方向のTransporterの各変数を配列保存して高速参照する
    //---------------------------------------------------------------------------------------------ReferBlock
    [SerializeField]AbstractSignalTrasnport[] connectedTransporter = new AbstractSignalTrasnport[2];
    int[] localConnectedTransporterID = new int[2];
    [UdonSynced] int[] syncedConnectedTransporterID = new int[2];
    bool[][] connectedTransporter_comingUpdateFace = new bool[2][];
    public virtual void changeConnection(bool ConnectingFace, AbstractSignalTrasnport newConnection,bool FaceofNewConnection)
    {
        int id = ConnectingFace ? 1 : 0;
        connectedTransporter[id] = newConnection;
        syncedConnectedTransporterID[id] = localConnectedTransporterID[id] = connectedTransporter[id].SignalTransporterID;
        //store pointer connectedTransporter[id].comingUpdateFace;
        connectedTransporter_comingUpdateFace[id] = connectedTransporter[id].comingUpdateFace;

        connectedFace[id] = FaceofNewConnection;
    }
    //---------------------------------------------------------------------------------------------ReferBlockEnd

    bool[] connectedFace = new bool[2];

    //Preset
    [SerializeField] AbstractSignalProcessor signalProcessor;
    public int SignalTransporterID;
    //Temporary
    public bool[] comingUpdateFace = new bool[1];

    protected virtual void Start()
    {
        
    }



    public virtual void UpdateSignal(bool f_r)
    {
        int id = f_r?1:0;
        //connectedTransporter[id].comingUpdateFace[0] = connectedFace[id];
        connectedTransporter_comingUpdateFace[id][0] = connectedFace[id];
        connectedTransporter[id].OnSpawn();
    }
    public override void OnSpawn()
    {
        //sendCustomEventを使用せず、使わないだろうPublicAPIで代替してやる
        UpdateSignal(!comingUpdateFace[0]);//来たのとは反対側へ信号を飛ばす
    }
}
