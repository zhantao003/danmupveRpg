using System;
using System.Collections.Generic;

namespace SharedLibrary
{
    /// <summary>
    /// 主播信息
    /// </summary>
    public class Broadcaster
    {
        public Broadcaster(string upId, string cName,string faceUrl,string UId)
        {
            this.UpId = upId;
            this.CName = cName;
            this.FaceUrl= faceUrl;
            this.Salt = "";
            this.LastHeartBeat= DateTime.MinValue;
            this.PlayerInRooms = new List<PlayerInRoom>();
            this.Exp = 0;
            this.RankScore = 0;
            this.TotalSpending = 0;
            this.UId =UId;
            //this.DailyMissionTookCounter = 0;
            //this.LastRefreshDailyMissionDate = DateTime.MinValue;
            //this.Resource1 = 0;
            //this.Resource2 = 0;
            this.BroadcasterState = BroadcasterState.Active;
            //this.Structure1Lv = 0;
            //this.Structure2Lv = 0;
            //this.HarborName = "";
            //this.MotherShipName = "";
            //this.MotherShipAvatar = 0;
            //this.AnnounceIslandBattleCounter = 0;
            //this.IslandBattleWinTimes = 0;
        }
        public Broadcaster()
        {
        }
        public int Id { get; set; }
        public string UpId { get; set; }
        public string CName { get; set; }
        public string FaceUrl { get; set; }
        public string Salt { get; set; }
        public DateTime LastHeartBeat { set; get; }
        public List<PlayerInRoom> PlayerInRooms { get; set; }
        public long Exp { get; set; }
        public int RankScore { get; set; }
        public long TotalSpending { get; set; }
        public string UId { get; set; }
        //public int DailyMissionTookCounter { get; set; }
        //public DateTime LastRefreshDailyMissionDate { get; set; }
        //public long Resource1 { get; set; }
        //public long Resource2 { get; set; }
        public BroadcasterState BroadcasterState { get; set; }
        //public int Structure1Lv { get; set; }
        //public int Structure2Lv { get; set; }
        //public string HarborName { get; set; }
        //public string MotherShipName { get; set; }
        //public int MotherShipAvatar { get; set; }
        //public int AnnounceIslandBattleCounter { get; set; }
        //public int IslandBattleWinTimes { get; set; }
    }

    public enum BroadcasterState
    {
        Active = 0, Limited = 1, Banned = 2
    }
}
