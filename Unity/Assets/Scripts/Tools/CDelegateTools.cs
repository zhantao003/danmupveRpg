using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//公共委托方法 void()
public delegate void DelegateNFuncCall();

//公共委托方法 void(int)
public delegate void DelegateIFuncCall(int value);

//公共委托方法 void(int, int)
public delegate void DelegateIIFuncCall(int valueA, int valueB);

//公共委托方法 void(bool)
public delegate void DelegateBFuncCall(bool value);

//公共委托方法 void(long)
public delegate void DelegateLFuncCall(long value);

//公共委托方法 void(float)
public delegate void DelegateFFuncCall(float value);

//公共委托方法 void(float,float)
public delegate void DelegateFFFuncCall(float value, float value2);

//公共委托方法 void(string)
public delegate void DelegateSFuncCall(string value);

//公共委托方法 void(string,string)
public delegate void DelegateSSFuncCall(string value, string value2);

//公共委托方法 void(string,int)
public delegate void DelegateSIFuncCall(string value,int value2);

////TBL委托方法
//public delegate void DelegateLoadTBL(CTBLLoader loader);

//公共委托方法 void(GameObject)
public delegate void DelegateGOFuncCall(GameObject value);

//公共委托方法 void(GameObject, int)
public delegate void DelegateGOIFuncCall(GameObject value, int idx);

//公共委托方法 void(GameObject, GameObject)
public delegate void DelegateGOGOFuncCall(GameObject origin, GameObject target);

//公共委托方法 void(params object[])
public delegate void DelegateOFuncCall(params object[] value);

//公共委托方法 void(PuzzleSlot)
public delegate void DelegatePosFuncCall(int nX,int nY);

public delegate void OnDeleagteLoginByTypeSuc(CPlayerBaseInfo info);

///Buff修改事件
public delegate void OnDelegateBuffChgCall(bool add, int nBuffID,int num);

public delegate void DelegateUnituncCall(CPlayerUnit unit);

public delegate void DelegateCampBattleChg(long battle, EMUnitCamp camp);

//切换名字头像显示
public delegate void DelegateNameHeadIconSwitchChg(CBattleMgr.EMDisPlayName displayType);
