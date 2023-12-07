#if UNITY_2020_3_OR_NEWER
using System.Collections.Generic;
using UnityEngine;

[UnityEngine.CreateAssetMenu(fileName = "MockData", menuName = "Game/MockData")]
public class MockData : ScriptableObject
{
    public List<CDanmuChat> dms;
    public List<CDanmuGift> sendGifts;
    public List<CDanmuVipInfo> guards;
    //public List<SuperChat> superChats;
    //public List<SuperChatDel> superChatDel;
    public List<CDanmuLike> likes;
}
#endif