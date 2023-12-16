using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class Player
    {
        public Player(string uId,string userCName, string userFace)
        {
            this.UId = uId;
            this.UserCName = userCName;
            this.UserFace = userFace;
            //this.CurrentShipAID = 0;
            //this.CurrentBurstAID = 0;
            //this.CurrentFaceAID = 0;
            //this.CurrentMotherAID = 0;
            //this.UsableShipAvatars = new List<UsableShipAvatar>();
            //this.UsableBurstAvatars = new List<UsableBurstAvatar>();
            //this.UsableFaceAvatars = new List<UsableFaceAvatar>();
            //this.UsableMotherShipAvatars = new List<UsableMotherShipAvatar>();
            this.PlayerInRooms = new List<PlayerInRoom>();
            this.Spending = 0;
            this.Identity = 0;
            this.ExpireDate = DateTime.Now;
            this.PlayerState = PlayerState.Active;
            //this.Resource1 = 0;
            //this.Resource2 = 0;
            //this.DailyQuestDoneCounter = 0;
            //this.LastRefreshDailyLimitDate = DateTime.MinValue;
            //this.Resource1GainToday = 0;
            //this.Resource2GainToday = 0;
            //this.TotalResources1Gain = 0;
            //this.TotalResources2Gain = 0;
            //this.PlayerUsableEquipments = new List<PlayerUsableEquipment>();
        }

        public Player() { }

        public int Id { get; set; }
        public string UId { get; set; }
        public string UserCName { get; set; }
        public string UserFace { get; set; }
        //public List<UsableShipAvatar> UsableShipAvatars { get; set; }
        //public List<UsableBurstAvatar> UsableBurstAvatars { get; set; }
        //public List<UsableFaceAvatar> UsableFaceAvatars { get; set; }
        //public List<UsableMotherShipAvatar> UsableMotherShipAvatars { get; set; }
        //public int CurrentShipAID { get; set; }
        //public int CurrentBurstAID { get; set; }
        //public int CurrentFaceAID { get; set; }
        //public int CurrentMotherAID { get; set; }
        public List<PlayerInRoom> PlayerInRooms { get; set; }
        public long Spending { get; set; }

        public int Identity { get; set; }
        public DateTime ExpireDate { get; set; }
        public PlayerState PlayerState { get; set; }
        //public long Resource1 { get; set; }
        //public long Resource2 { get; set; }
        //public int DailyQuestDoneCounter { get; set; }
        //public DateTime LastRefreshDailyLimitDate { get; set; }
        //public long Resource1GainToday { get; set; }
        //public long Resource2GainToday { get; set; }
        //public long TotalResources1Gain { get; set; }
        //public long TotalResources2Gain { get; set; }
        //public List<PlayerUsableEquipment> PlayerUsableEquipments { get; set; }
    }

    public enum PlayerState { 
        Active = 0, Limited = 1,Banned = 2
    }
}
