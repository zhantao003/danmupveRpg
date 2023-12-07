using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EUserInfoMgr : CSingleMgrBase<EUserInfoMgr>
{
    public EUserInfo pSelf;

    public Dictionary<long, EUserInfo> dicUserInfo = new Dictionary<long, EUserInfo>();

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
}
