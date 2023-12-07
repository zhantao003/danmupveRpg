using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ETModel
{
    //音频模组管理器
    public class CAudioModelMgr
    {
        #region Instance

        private CAudioModelMgr() { }

        private static CAudioModelMgr m_Instance = null;
        public static CAudioModelMgr Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    //objAudioMgr
                    m_Instance = new CAudioModelMgr();
                }
                return m_Instance;
            }
        }

        #endregion

        //音频表地址
        public static readonly string TBL_AUDIOMODEL_PATH = "TBL/AudioModel";

        //音频模组信息
        public class ST_AudioModelInfo
        {
            public int nID;
            public string szRes;
            public Dictionary<int, ST_AudioModelDataSlot> dicData = new Dictionary<int, ST_AudioModelDataSlot>();
        }

        public class ST_AudioModelDataSlot
        {
            public int nID;
            public int nAudioID;
            public bool bLoop;
            public float fVolum;
            public float fBlend;
            public float fMinRange;
            public float fMaxRange;
        }

        protected Dictionary<int, ST_AudioModelInfo> dicAudioModelInfo = new Dictionary<int, ST_AudioModelInfo>();
        //protected Dictionary<int, Dictionary<int, ST_AudioModelDataSlot>> dicAudioModelData = new Dictionary<int, Dictionary<int, ST_AudioModelDataSlot>>();

        //初始化(只需要调用一次)
        public void Init()
        {
            CTBLInfo.Inst.LoadTBL(TBL_AUDIOMODEL_PATH, OnLoadAudioModelInfo);
        }

        //加载表数据
        protected void OnLoadAudioModelInfo(CTBLLoader loader)
        {
            for (int i = 0; i < loader.GetLineCount(); i++)
            {
                loader.GotoLineByIndex(i);

                ST_AudioModelInfo pInfo = new ST_AudioModelInfo();
                pInfo.nID = loader.GetIntByName("id");
                pInfo.szRes = loader.GetStringByName("path");

                dicAudioModelInfo.Add(pInfo.nID, pInfo);

                Debug.Log("音频模组:" + pInfo.nID + "  " + pInfo.szRes);

                //直接加载模组的数据
                OnLoadAudioModelData(pInfo);
            }
        }

        //获取指定ID的模组信息
        protected ST_AudioModelInfo GetAudioModelInfo(int nModelID)
        {
            ST_AudioModelInfo pInfo = null;
            if (dicAudioModelInfo.TryGetValue(nModelID, out pInfo))
            {
                return pInfo;
            }
            return pInfo;
        }

        //获取指定ID模组的Data信息
        protected Dictionary<int, ST_AudioModelDataSlot> GetAudioModelData(int nModelID)
        {
            ST_AudioModelInfo pModelInfo = GetAudioModelInfo(nModelID);
            if (pModelInfo == null) return null;
            Dictionary<int, ST_AudioModelDataSlot> pInfo = pModelInfo.dicData;
            return pInfo;
        }

        //获取指定DataSlot
        protected ST_AudioModelDataSlot GetAudioModelDataSlot(int nModelID, int nID)
        {
            ST_AudioModelDataSlot pRes = null;
            Dictionary<int, ST_AudioModelDataSlot> pDicData = GetAudioModelData(nModelID);
            if (pDicData == null) return null;

            if (pDicData.TryGetValue(nID, out pRes))
            {
                //return pRes;
            }

            return pRes;
        }

        //加载指定ID的模组数据信息
        protected void OnLoadAudioModelData(ST_AudioModelInfo pModel)
        {
            //ST_AudioModelInfo pModel = GetAudioModelInfo(nModelID);
            if (pModel == null) return;

            //重塑Data容器
            Dictionary<int, ST_AudioModelDataSlot> pData = pModel.dicData;
            if (pData != null)
            {
                pData.Clear();
            }
            else
            {
                pData = new Dictionary<int, ST_AudioModelDataSlot>();
                //dicAudioModelData.Add(nModelID, pData);
            }

            CTBLInfo.Inst.LoadTBL(pModel.szRes, delegate (CTBLLoader loader)
            {
                for (int i = 0; i < loader.GetLineCount(); i++)
                {
                    loader.GotoLineByIndex(i);

                    ST_AudioModelDataSlot pInfo = new ST_AudioModelDataSlot();
                    pInfo.nID = loader.GetIntByName("id");
                    pInfo.nAudioID = loader.GetIntByName("audioID");
                    pInfo.bLoop = ((loader.GetIntByName("loop") == 1) ? true : false);
                    pInfo.fVolum = loader.GetFloatByName("volum");
                    pInfo.fBlend = loader.GetFloatByName("blend");
                    pInfo.fMinRange = loader.GetFloatByName("minRange");
                    pInfo.fMaxRange = loader.GetFloatByName("maxRange");

                    pData.Add(pInfo.nID, pInfo);

                    Debug.Log("音频模组数据:" + pInfo.nID + "  " + pInfo.nAudioID);
                }
            });
        }

        //public CAudioMgr.CAudioSlottInfoWithID GetAudioInfoByModelData(int nModelID, int nDataID)
        //{
        //    CAudioMgr.CAudioSlottInfoWithID pRes = null;

        //    Dictionary<int, ST_AudioModelDataSlot> pDicData = GetAudioModelData(nModelID);
        //    if (pDicData == null) return null;

        //    ST_AudioModelDataSlot pDataSlot = GetAudioModelDataSlot(nModelID, nDataID);
        //    if (pDataSlot == null) return null;

        //    pRes = new CAudioMgr.CAudioSlottInfoWithID();
        //    pRes.nID = pDataSlot.nAudioID;
        //    pRes.fVolum = pDataSlot.fVolum;
        //    pRes.fBlend = pDataSlot.fBlend;
        //    pRes.bLoop = pDataSlot.bLoop;
        //    pRes.vClipRange = new Vector2(pDataSlot.fMinRange, pDataSlot.fMaxRange);

        //    return pRes;
        //}
    }
}