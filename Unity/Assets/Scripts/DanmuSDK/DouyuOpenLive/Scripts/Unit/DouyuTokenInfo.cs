using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouyuDanmu
{
    public class DouyuTokenInfo
    {
        public int nCode;
        public string szToken;  //当前场次的token
        public string szWsUrl;  //当钱场次的长链地址
        public string szUserName;       //主播名字
        public string szUserHeadIcon;   //主播头像

        public DouyuTokenInfo(CLocalNetMsg msgContent)
        {
            if(msgContent==null)
            {
                nCode = -1;
                return;
            }

            nCode = msgContent.GetInt("code");
            if (nCode != 200)
            {
                return;
            }

            CLocalNetMsg msgInfo = msgContent.GetNetMsg("data");
            if (msgInfo != null)
            {
                szToken = msgInfo.GetString("accessToken");
                szWsUrl = msgInfo.GetString("socket");
                szUserName = msgInfo.GetString("nickName");
                szUserHeadIcon = msgInfo.GetString("bigAvatar");

                Debug.Log("场次Token：" + szToken);
                Debug.Log("长链地址：" + szWsUrl);
            }
        }

        public void InitByMsg(CLocalNetMsg msgContent)
        {
           
        }
    }
}


