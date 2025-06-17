
using System.ComponentModel;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CS5SignalTrasnport : AbstractSignalTrasnporter
{
    protected sbyte[][] connectedCS5_TempSignal = new sbyte[2][];
    public override void StoreConnectedPointer(int id)
    {
        base.StoreConnectedPointer(id);

        if (connectedTransporter[id].GetUdonTypeID() == this.GetUdonTypeID())//キャストチェックできねんか！？
            connectedCS5_TempSignal[id] = ((CS5SignalTrasnport)connectedTransporter[id]).TempSignal;
        //伝送を受ける一時配列の参照を保存する
    }

    [SerializeField] protected sbyte[] NeutralSignal = new sbyte[3];
    [SerializeField] protected sbyte[] TempSignal = new sbyte[3];
    [SerializeField] protected sbyte[] StoredSignal = new sbyte[3];
    [SerializeField] protected sbyte[] ControllerSignal = new sbyte[3];

    public override void StartTransportSignal()
    {
        if (!alreadyAppliedSingal)//LateUpdate-LateUpdateの間ですでに信号送信済みならセット省略
        {
            //Debug.Log("Start " + gameObject.name);
            alreadyAppliedSingal = true;
            ControllerSignal.CopyTo(StoredSignal, 0);//開始時は自身の信号を強制セット
        }
        base.StartTransportSignal();
    }

    bool alreadyAppliedSingal;
    public override void SignalUpdateRecieve()
    {
        for (int signalIndex = 0; signalIndex < TempSignal.Length; signalIndex++)
        {
            if(!alreadyAppliedSingal && ControllerSignal[signalIndex] != 0)
            //コントローラーの入力があるなら信号を書き換える 書き換え済みの場合は無視（他コントローラーがあとから自身の信号を送ってくるので）
            {
                //Debug.Log("Update " + gameObject.name);
                StoredSignal[signalIndex] = (sbyte)(TempSignal[signalIndex] + ControllerSignal[signalIndex]);//和を取って書き換え
            }
            else
            {
                StoredSignal[signalIndex] = TempSignal[signalIndex];
            }
        }
        alreadyAppliedSingal = true;
        base.SignalUpdateRecieve();
        return;
    }
    public override void TransportSignal(bool f_r, int nextHopping, bool direction)
    {

        if (!inited) Initialize();
        int id = f_r ? 1 : 0;
        if (connectedTransporter[id])
        {
            StoredSignal.CopyTo(connectedCS5_TempSignal[id],0);//自身のStoredSignalを相手のTempSignalへ転送
        }
        base.TransportSignal(f_r, nextHopping, direction);
    }

    protected override void ResetTemporary()
    {
        base.ResetTemporary();
        NeutralSignal.CopyTo(TempSignal, 0);
        alreadyAppliedSingal = false;
        //一時的なフラグ等を戻す
    }
}
