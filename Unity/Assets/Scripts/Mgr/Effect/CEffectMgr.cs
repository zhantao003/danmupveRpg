using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEffectMgr
{
    public long nEffID = 1;

    #region Instance

    private static CEffectMgr _ins;
    public static CEffectMgr Instance
    {
        get
        {
            if (_ins == null)
            {
                _ins = new CEffectMgr();
            }
            return _ins;
        }
    }

    #endregion

    Dictionary<long, CEffectBase> dicActiveEffect = new Dictionary<long, CEffectBase>();

    Dictionary<string, List<CEffectBase>> dicEffIdle = new Dictionary<string, List<CEffectBase>>();

    public void CreateEff(string bundle, string effName, Vector3 pos, Quaternion rot, int layer, DelegateGOFuncCall callOver = null)
    {
        //Debug.Log("Create Eff:" + effName);

        CEffectBase pIdleEff = PopEff(effName);
        if(pIdleEff!=null)
        {
            GameObject objIdleEff = pIdleEff.gameObject;
            objIdleEff.transform.SetParent(null);
            objIdleEff.transform.position = pos;
            objIdleEff.transform.localRotation = rot;
            objIdleEff.transform.localScale = Vector3.one;

            pIdleEff.Init();
            pIdleEff.SetLayer(layer);

            //Debug.Log("Create Eff:" + effName + "  Guid:" + pIdleEff.lGuid);

            if (dicActiveEffect.ContainsKey(pIdleEff.lGuid))
            {
                Debug.LogError("Same Eff in Active:" + effName + "    Guid:" + pIdleEff.lGuid);
            }

            dicActiveEffect.Add(pIdleEff.lGuid, pIdleEff);

            callOver?.Invoke(objIdleEff);
            return;
        }

        CResLoadMgr.Inst.ACreateAssetByType(bundle, effName, typeof(GameObject),
           delegate (Object pRes)
           {
               GameObject objEffectRoot = pRes as GameObject;

               //初始化失败
               if (objEffectRoot == null)
               {
                   Debug.LogError("初始化特效失败----" + effName);
                   return;
               }

               GameObject objNewEffect = GameObject.Instantiate(objEffectRoot) as GameObject;
               objNewEffect.transform.SetParent(null);
               objNewEffect.transform.position = pos;
               objNewEffect.transform.localRotation = rot;
               objNewEffect.transform.localScale = Vector3.one;
               objNewEffect.gameObject.name = effName;

               CEffectBase pNewEffect = objNewEffect.GetComponent<CEffectBase>();
               if(pNewEffect == null)
               {
                   Debug.LogError("None Eff Component");
                   GameObject.Destroy(objNewEffect);
                   return;
               }

               pNewEffect.Init();
               pNewEffect.SetLayer(layer);

               //Debug.Log("Create Eff:" + effName + "  Guid:" + pNewEffect.lGuid);

               if (dicActiveEffect.ContainsKey(pNewEffect.lGuid))
               {
                   Debug.LogError("Same Eff in Active:" + effName + "    Guid:" + pNewEffect.lGuid);
               }

               dicActiveEffect.Add(pNewEffect.lGuid, pNewEffect);

               callOver?.Invoke(objNewEffect);
           });
    }

    public void CreateEff(string bundle, string effName, Transform parentRoot, int layer, bool bind = false, DelegateGOFuncCall callOver = null)
    {
        if (parentRoot == null) return;

        CEffectBase pIdleEff = PopEff(effName);
        if (pIdleEff != null)
        {
            GameObject objIdleEff = pIdleEff.gameObject;

            if (bind)
            {
                objIdleEff.transform.SetParent(parentRoot);
            }

            objIdleEff.transform.position = parentRoot.position;
            objIdleEff.transform.forward = parentRoot.forward;
            objIdleEff.transform.localScale = Vector3.one;
            objIdleEff.transform.localRotation = Quaternion.identity;

            pIdleEff.Init();
            pIdleEff.SetLayer(layer);
            //Debug.Log("Pop Eff:" + effName + "   Guid:" + pIdleEff.lGuid);

            //Debug.Log("Create Eff:" + effName + "  Guid:" + pIdleEff.lGuid);

            if (dicActiveEffect.ContainsKey(pIdleEff.lGuid))
            {
                Debug.LogError("Same Eff in Active:" + effName + "    Guid:" + pIdleEff.lGuid);
            }

            dicActiveEffect.Add(pIdleEff.lGuid, pIdleEff);

            callOver?.Invoke(objIdleEff);
            return;
        }

        CResLoadMgr.Inst.ACreateAssetByType(bundle, effName, typeof(GameObject),
          delegate (Object pRes)
          {
              GameObject objEffectRoot = pRes as GameObject;

              //初始化失败
              if (objEffectRoot == null)
              {
                  Debug.LogError("初始化特效失败----" + effName);
                  return;
              }

              GameObject objNewEffect = GameObject.Instantiate(objEffectRoot) as GameObject;
              Transform tranNewEff = objNewEffect.transform;
              objNewEffect.gameObject.name = effName;

              if (parentRoot == null)
              {
                  CEffectBase pTmpEff = objNewEffect.GetComponent<CEffectBase>();
                  pTmpEff.Recycle();
                  return;
              }

              if(bind)
              {
                  tranNewEff.SetParent(parentRoot);
                  tranNewEff.localPosition = Vector3.zero;
                  tranNewEff.localRotation = Quaternion.identity;
                  tranNewEff.localScale = Vector3.one;
              }
              else
              {
                  tranNewEff.SetParent(null);
                  tranNewEff.position = parentRoot.position;
                  tranNewEff.rotation = parentRoot.rotation;
                  tranNewEff.localScale = Vector3.one;
              }

              CEffectBase pNewEffect = objNewEffect.GetComponent<CEffectBase>();
              pNewEffect.Init();
              pNewEffect.SetLayer(layer);

              //Debug.Log("Create Eff:" + effName + "  Guid:" + pNewEffect.lGuid);

              if (dicActiveEffect.ContainsKey(pNewEffect.lGuid))
              {
                  Debug.LogError("Same Eff in Active:" + effName + "    Guid:" + pNewEffect.lGuid);
              }

              dicActiveEffect.Add(pNewEffect.lGuid, pNewEffect);

              callOver?.Invoke(objNewEffect);
          });
    }
    
    public void CreateEffSync(string effName, Transform parentRoot, int layer, bool bind = false, DelegateGOFuncCall callOver = null)
    {
        //Debug.Log("加载特效：" + effName);

        if (parentRoot == null) return;

        CEffectBase pIdleEff = PopEff(effName);
        if (pIdleEff != null)
        {
            GameObject objIdleEff = pIdleEff.gameObject;

            if (bind)
            {
                objIdleEff.transform.SetParent(parentRoot);
            }

            objIdleEff.transform.position = parentRoot.position;
            objIdleEff.transform.forward = parentRoot.forward;
            objIdleEff.transform.localScale = Vector3.one;
            objIdleEff.transform.localRotation = Quaternion.identity;

            pIdleEff.Init();
            pIdleEff.SetLayer(layer);
            //Debug.Log("Pop Eff:" + effName + "   Guid:" + pIdleEff.lGuid);

            //Debug.Log("Create Eff:" + effName + "  Guid:" + pIdleEff.lGuid);

            if (dicActiveEffect.ContainsKey(pIdleEff.lGuid))
            {
                Debug.LogError("Same Eff in Active:" + effName + "    Guid:" + pIdleEff.lGuid);
            }

            dicActiveEffect.Add(pIdleEff.lGuid, pIdleEff);

            callOver?.Invoke(objIdleEff);
            return;
        }

        CResLoadMgr.Inst.SynLoad(effName, CResLoadMgr.EM_ResLoadType.Effect, delegate (Object pRes, object data, bool bSuc)
          {
              GameObject objEffectRoot = pRes as GameObject;

              //初始化失败
              if (objEffectRoot == null)
              {
                  Debug.LogError("初始化特效失败----" + effName);
                  return;
              }

              GameObject objNewEffect = GameObject.Instantiate(objEffectRoot) as GameObject;
              Transform tranNewEff = objNewEffect.transform;
              objNewEffect.gameObject.name = effName;

              if (parentRoot == null)
              {
                  CEffectBase pTmpEff = objNewEffect.GetComponent<CEffectBase>();
                  pTmpEff.Recycle();
                  return;
              }

              if (bind)
              {
                  tranNewEff.SetParent(parentRoot);
                  tranNewEff.localPosition = Vector3.zero;
                  tranNewEff.localRotation = Quaternion.identity;
                  tranNewEff.localScale = Vector3.one;
              }
              else
              {
                  tranNewEff.SetParent(null);
                  tranNewEff.position = parentRoot.position;
                  tranNewEff.rotation = parentRoot.rotation;
                  tranNewEff.localScale = Vector3.one;
              }

              CEffectBase pNewEffect = objNewEffect.GetComponent<CEffectBase>();
              pNewEffect.Init();
              pNewEffect.SetLayer(layer);

              //Debug.Log("Create Eff:" + effName + "  Guid:" + pNewEffect.lGuid);

              if (dicActiveEffect.ContainsKey(pNewEffect.lGuid))
              {
                  Debug.LogError("Same Eff in Active:" + effName + "    Guid:" + pNewEffect.lGuid);
              }

              dicActiveEffect.Add(pNewEffect.lGuid, pNewEffect);

              callOver?.Invoke(objNewEffect);
          });
    }

    public void CreateEffSync(string effName, Vector3 pos, Quaternion rot, int layer, DelegateGOFuncCall callOver = null)
    {
        //Debug.Log("Create Eff:" + effName);

        CEffectBase pIdleEff = PopEff(effName);
        if (pIdleEff != null)
        {
            GameObject objIdleEff = pIdleEff.gameObject;
            objIdleEff.transform.SetParent(null);
            objIdleEff.transform.position = pos;
            objIdleEff.transform.localRotation = rot;
            objIdleEff.transform.localScale = Vector3.one;

            pIdleEff.Init();
            pIdleEff.SetLayer(layer);

            //Debug.Log("Create Eff:" + effName + "  Guid:" + pIdleEff.lGuid);

            if (dicActiveEffect.ContainsKey(pIdleEff.lGuid))
            {
                Debug.LogError("Same Eff in Active:" + effName + "    Guid:" + pIdleEff.lGuid);
            }

            dicActiveEffect.Add(pIdleEff.lGuid, pIdleEff);

            callOver?.Invoke(objIdleEff);
            return;
        }

        CResLoadMgr.Inst.SynLoad(effName, CResLoadMgr.EM_ResLoadType.Effect, delegate (Object pRes, object data, bool bSuc)
        {
               GameObject objEffectRoot = pRes as GameObject;

               //初始化失败
               if (objEffectRoot == null)
               {
                   Debug.LogError("初始化特效失败----" + effName);
                   return;
               }

               GameObject objNewEffect = GameObject.Instantiate(objEffectRoot) as GameObject;
               objNewEffect.transform.SetParent(null);
               objNewEffect.transform.position = pos;
               objNewEffect.transform.localRotation = rot;
               objNewEffect.transform.localScale = Vector3.one;
               objNewEffect.gameObject.name = effName;

               CEffectBase pNewEffect = objNewEffect.GetComponent<CEffectBase>();
               if (pNewEffect == null)
               {
                   Debug.LogError("None Eff Component");
                   GameObject.Destroy(objNewEffect);
                   return;
               }

               pNewEffect.Init();
               pNewEffect.SetLayer(layer);

               //Debug.Log("Create Eff:" + effName + "  Guid:" + pNewEffect.lGuid);

               if (dicActiveEffect.ContainsKey(pNewEffect.lGuid))
               {
                   Debug.LogError("Same Eff in Active:" + effName + "    Guid:" + pNewEffect.lGuid);
               }

               dicActiveEffect.Add(pNewEffect.lGuid, pNewEffect);

               callOver?.Invoke(objNewEffect);
           });
    }

    CEffectBase PopEff(string effName)
    {
        CEffectBase pRes = null;
        List<CEffectBase> listEffIdle = null;
        //Debug.Log("eff:" + effName);
        if (dicEffIdle.TryGetValue(effName, out listEffIdle))
        {
            if(listEffIdle.Count > 0)
            {
                pRes = listEffIdle[0];
                listEffIdle.RemoveAt(0);
            }
        }
        return pRes;
    }

    public void Recycle(CEffectBase effect)
    {
        effect.enabled = false;
        effect.transform.SetParent(null);
        effect.transform.position = new Vector3(20000, 0, 0);
        effect.StopEffect();

        dicActiveEffect.Remove(effect.lGuid);

        string effName = effect.gameObject.name;
        List<CEffectBase> listEffIdle = null;
        if(dicEffIdle.TryGetValue(effName, out listEffIdle))
        {
            listEffIdle.Add(effect);
        }
        else
        {
            listEffIdle = new List<CEffectBase>();
            listEffIdle.Add(effect);
            dicEffIdle.Add(effName, listEffIdle);
        }
    }

    /// <summary>
    /// 是否暂停所有特效
    /// </summary>
    /// <param name="pause"></param>
    public void PauseAllEffect(bool pause)
    {
        foreach(CEffectBase eff in dicActiveEffect.Values)
        {
            if(pause)
            {
                eff.PauseEffect();
            }
            else
            {
                eff.Play(false);
            }
        }
    }

    public long GenerateID()
    {
        long lID = nEffID;
        nEffID += 1;

        //Debug.Log("特效ID：" + lID + "   更新全局ID：" + nEffID);

        return lID;
    }

    public void Clear()
    {
        dicActiveEffect.Clear();
        dicEffIdle.Clear();
    }
}
