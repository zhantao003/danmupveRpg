using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouyuDanmu
{
    public class DouyuTokenInfo
    {
        public int nCode;
        public string szToken;  //��ǰ���ε�token
        public string szWsUrl;  //��Ǯ���εĳ�����ַ
        public string szUserName;       //��������
        public string szUserHeadIcon;   //����ͷ��

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

                Debug.Log("����Token��" + szToken);
                Debug.Log("������ַ��" + szWsUrl);
            }
        }

        public void InitByMsg(CLocalNetMsg msgContent)
        {
           
        }
    }
}


