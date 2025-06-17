
using Cinemachine;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;
using static Cinemachine.CinemachinePathBase;

public class AbstractSignalTrasnporter : UdonSharpBehaviour
{
    //信号伝送ベースクラス
    //ChangeConnectionで相手のポインタを取得する
    //StartTransportSignalで伝送開始
    //SignalUpdateRecieveは外から呼び出された際に呼ばれ、ここで信号に書き込み
    //※LateUpdateで信号送信は行わないこと※


    //Face true = +Z
    //Face false = -Z
    //Reference
    //参照した前後方向のTransporterの各変数を配列保存して高速参照する
    //---------------------------------------------------------------------------------------------ReferBlock
    [SerializeField]protected AbstractSignalTrasnporter[] connectedTransporter = new AbstractSignalTrasnporter[2];
    protected int[] localConnectedTransporterID = new int[2];
    [UdonSynced] protected int[] syncedConnectedTransporterID = new int[2];
    protected bool[][] connectedTransporter_UpdateComingFace = new bool[2][];
    protected int[][] connectedTransporter_UpdateHopping = new int[2][];
    protected bool[][] connectedTransporter_UpdateDirection = new bool[2][];
    //protected Toggle[] connectedTransporter_eventProxy1 = new Toggle[2];
    public virtual void ChangeConnection(bool ConnectingFace, AbstractSignalTrasnporter newConnection,bool FaceofNewConnection)
    {
        int id = ConnectingFace ? 1 : 0;
        connectedTransporter[id] = newConnection;
        if(newConnection)
        {
            syncedConnectedTransporterID[id] = localConnectedTransporterID[id] = connectedTransporter[id].SignalTransporterID;
            connectedFace[id] = FaceofNewConnection;
            StoreConnectedPointer(id);
        }
        else
        {
            syncedConnectedTransporterID[id] = localConnectedTransporterID[id] = -1;
        }
        if (!Networking.IsOwner(this.gameObject))
        {
            this.enabled = true;
            disableCounter = 10;
            RequestSerialization_Mod();
        }
    }

    public virtual void StoreConnectedPointer(int id)
    {
        //store pointer connectedTransporter[id].comingUpdateFace;
        connectedTransporter_UpdateComingFace[id] = connectedTransporter[id].UpdateComingFace;
        connectedTransporter_UpdateHopping[id] = connectedTransporter[id].UpdateHopping;
        connectedTransporter_UpdateDirection[id] = connectedTransporter[id].UpdateDirection;
        //connectedTransporter_eventProxy1[id] = connectedTransporter[id].eventProxy1;//Event Call Hack
    }
    //---------------------------------------------------------------------------------------------ReferBlockEnd

    [SerializeField][UdonSynced] protected bool[] connectedFace = new bool[2];

    //Preset
    [SerializeField] protected AbstractSignalProcessor signalProcessor;
    //[SerializeField] public Toggle eventProxy1;
    [HideInInspector] public AbstractSignalTrasnporter[] SignalTrasnports;
    public int SignalTransporterID;

    //Temporary
    public bool[] UpdateComingFace = new bool[1];
    public int[] UpdateHopping = new int[1];
    public bool[] UpdateDirection = new bool[1];

    protected int safeCounter;

    public virtual void Initialize()
    {
        ChangeConnection(false, connectedTransporter[0], connectedFace[0]);
        ChangeConnection(true, connectedTransporter[1], connectedFace[1]);
        inited = true;
    }
    protected bool inited = false;
    protected virtual void Start()
    {
        if(!inited)Initialize();
    }

    public virtual void StartTransportSignal()
    {
        this.enabled = true;
        disableCounter = 10;
        UpdateHopping[0] = 0;
        TransportSignal(true, UpdateHopping[0], true);
        TransportSignal(false, UpdateHopping[0], false);
    }

    public virtual void InvestSignal()
    {
        //UIコンポーネント経由でUBのPublicメソッドを呼び出してくる
        if (Mathf.Abs(UpdateHopping[0]) > 400)
        {
            Debug.LogError("Infinite loop or too long train!");
            return;
        }
        SignalUpdateRecieve();
        this.enabled = true;
        disableCounter = 10;
        TransportSignal(!UpdateComingFace[0], UpdateHopping[0], UpdateDirection[0]);//来たのとは反対側へ信号を飛ばす
    }

    public virtual void SignalUpdateRecieve()
    {
        //Debug.Log("UpdateSignal " + this.gameObject.name + " from " + UpdateComingFace[0] + " at " + UpdateHopping[0]);
        //信号の更新を受ける
        //UpdateComingFace側信号と比較して、自身の新しい信号を設定する
        //自身の信号を優先したい場合等はtrueを返すと、逆向きに信号を伝播させられる。
        return;
    }

    //送り先への信号書き込みはbaseで呼び出す前に行うこと
    public virtual void TransportSignal(bool f_r,int nextHopping, bool direction)
    {
        if (!inited) Initialize();
        int id = f_r?1:0;
        if (connectedTransporter[id])
        {
            //connectedTransporter[id].UpdateComingFace[0] = connectedFace[id];
            connectedTransporter_UpdateComingFace[id][0] = connectedFace[id];
            connectedTransporter_UpdateHopping[id][0] = nextHopping + (direction ? 1 : -1);
            connectedTransporter_UpdateDirection[id][0] = direction;
            //相手側に処理を移す
            connectedTransporter[id].InvestSignal();
        }
    }

    public void LateUpdate()
    {
        ResetTemporary();
        if (!hasWaitingSync || !Networking.IsOwner(this.gameObject)) TryDisable();
        //信号伝送後はフラグを戻しておく
    }

    protected virtual void ResetTemporary()
    {
        safeCounter = 0;
        UpdateHopping[0] = -2147483647;
    }

    int disableCounter = -1;
    protected void TryDisable()
    {
        disableCounter--;
        if (disableCounter < 0)
        {
            this.enabled = false;
        }
    }

    public override void OnPreSerialization()
    {
        syncedConnectedTransporterID[0] = localConnectedTransporterID[0];
        syncedConnectedTransporterID[1] = localConnectedTransporterID[1];
    }

    public override void OnPostSerialization(SerializationResult result)
    {
        base.OnPostSerialization(result);
        hasWaitingSync = false;
    }

    public override void OnDeserialization()
    {
        for(int id = 0; id < 2; id++)
        {
            if (syncedConnectedTransporterID[id] != localConnectedTransporterID[id])
            {
                ChangeConnection(id == 1, SignalTrasnports[syncedConnectedTransporterID[id]], connectedFace[id]);
            }
        }
    }

    bool hasWaitingSync = false;
    public void RequestSerialization_Mod()
    {
        hasWaitingSync = true;
    }

    public override void Interact()
    {
        //Debug.Log("Debug transport signal " + this.gameObject.name);
        StartTransportSignal();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0, 1f, 1f);
        int selector = 0;
        if (connectedTransporter[0])
        {
            drawConnection(selector);
        }
        Gizmos.color = new Color(1f, 0, 0f, 1f);
        selector = 1;
        if (connectedTransporter[1])
        {
            drawConnection(selector);
        }

    }

    void drawConnection(int selector)
    {
        Gizmos.DrawLine(this.transform.position, connectedTransporter[selector].transform.position * 0.5f + this.transform.position * 0.5f);
    }
}
