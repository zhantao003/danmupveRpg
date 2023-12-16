using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EUserInfoMgr : CSingleMgrBase<EUserInfoMgr>
{
    public EUserInfo pSelf;

    public EMUnitCamp emSelfCamp = 0;

    public Dictionary<long, EUserInfo> dicUserInfo = new Dictionary<long, EUserInfo>();

    public List<EUserInfo> listUserOnlineInfo = new List<EUserInfo>();

    public void AddUser(EUserInfo user)
    {
        if(dicUserInfo.ContainsKey(user.nUserId))
        {
            dicUserInfo[user.nUserId] = user;
        }
        else
        {
            dicUserInfo.Add(user.nUserId, user);
        }
    }

    public EUserInfo GetUser(long id)
    {
        EUserInfo pRes = null;
        if (dicUserInfo.TryGetValue(id, out pRes))
        { 
        
        }

        return pRes;
    }

    public void RemoveUser(long id)
    {
        dicUserInfo.Remove(id);
    }

    #region Onlien User Info

    public void AddOnlineUser(EUserInfo user)
    {
        listUserOnlineInfo.Add(user);
    }

    public EUserInfo GetOnlineUser(string platformId)
    {
        return listUserOnlineInfo.Find(x => x.szPlatformId.Equals(platformId));
    }

    public void RemoveOnlineUser(string platformId)
    {
        listUserOnlineInfo.RemoveAll(x => x.szPlatformId.Equals(platformId));
    }

    public void ClearOnlineUser()
    {
        listUserOnlineInfo.Clear();
    }

    #endregion
}
