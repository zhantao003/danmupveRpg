using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EPlayerMgr : MonoBehaviour
{
    public static EPlayerMgr Ins = null;
    public GameObject objPlayerRoot;

    private void Start()
    {
        Ins = this;
    }

    public void CreatePlayer(Vector3 pos, Quaternion rot, EUserInfo user, bool self)
    {
        GameObject objPlayerUnit = GameObject.Instantiate(objPlayerRoot) as GameObject;
        EPlayerUnit pUnit = objPlayerUnit.GetComponent<EPlayerUnit>();
        pUnit.tranRoot.position = pos;
        pUnit.tranRoot.rotation = rot;

        pUnit.SetPlayerInfo(user, self);

        //告诉服务器创建了单位
        if(self)
        {
            DVec3 Pos = new DVec3();
            Pos.X = pos.x; Pos.Y = pos.y; Pos.Z = pos.z;
            DVec4 Rot = new DVec4();
            Rot.X = rot.x; Rot.Y = rot.y; Rot.Z = rot.z; Rot.W = rot.w;
            SessionComponent.Instance.Session.Send(new Actor_CreateTetsPlayerUnit_C2M() { 
                UserId = user.nUserId,
                Pos = Pos,
                Rotation = Rot
            });
        }
    }

    private void OnGUI()
    {
        //if(GUILayout.Button("Create"))
        //{
        //    Vector3 vPos = new Vector3();
        //    vPos.x = Random.Range(-5f, 5f);
        //    vPos.z = Random.Range(-5f, 5f);

        //    EUserInfo pInfo = new EUserInfo();

        //    CreatePlayer(vPos, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), new EUserInfo(), false);
        //}
    }
}
