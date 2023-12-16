using System;
using System.Collections.Generic;

namespace SharedLibrary
{
    public interface IBaseResponse { 
        public string FailReason { get; set; }
    }
    public class AuthenticationResponse:ABJsonDeSerializeObject ,IBaseResponse
    {
        public AuthenticationResponse()
        {
            this.Token = "";
            this.ServerUTCTime = DateTime.UtcNow;
            this.FailReason = "";
            this.Key = "";
            this.Broadcaster = null;
        }
        public string Token { get; set; }
        public DateTime ServerUTCTime { get; set; }
        public Broadcaster Broadcaster { get; set; }
        public string Key { get; set; }
        public string FailReason { get; set; }

        public override void FillDatas(string json)
        {
            CLocalNetMsg msg = new CLocalNetMsg(json);
            this.FailReason = msg.GetString("FailReason");
            this.Token = msg.GetString("Token");
            this.ServerUTCTime = (DateTime)Convert.ChangeType(msg.GetString("ServerUTCTime"),typeof(DateTime));
            this.Key = msg.GetString("Key");
            Broadcaster = new Broadcaster();
            CLocalNetMsg bcMsg = msg.GetNetMsg("Broadcaster");
            Broadcaster.UpId = bcMsg.GetString("UpId");
            Broadcaster.CName = bcMsg.GetString("CName");
            Broadcaster.FaceUrl = bcMsg.GetString("FaceUrl");
            Broadcaster.Salt = bcMsg.GetString("Salt");
            Broadcaster.LastHeartBeat = (DateTime)Convert.ChangeType(bcMsg.GetString("LastHeartBeat"), typeof(DateTime));
            Broadcaster.Exp = bcMsg.GetLong("Exp");
            Broadcaster.RankScore = bcMsg.GetInt("RankScore");
            Broadcaster.TotalSpending = bcMsg.GetLong("TotalSpending");
            Broadcaster.UId = bcMsg.GetString("UId");
            Broadcaster.BroadcasterState = (BroadcasterState)bcMsg.GetInt("BroadcasterState");
        }
    }

    public class BroadcasterEXPGainResponse: ABJsonDeSerializeObject, IBaseResponse
    {
        public BroadcasterEXPGainResponse()
        {
            this.FailReason = "";
            this.TotalExp = 0;
        }
        public long TotalExp { get; set; }
        public string FailReason { get; set; }

        public override void FillDatas(string json)
        {
            CLocalNetMsg msg = new CLocalNetMsg(json);
            this.FailReason = msg.GetString("FailReason");
            this.TotalExp = msg.GetLong("TotalExp");
        }
    }
    public class GetPlayerInfoResponse : ABJsonDeSerializeObject, IBaseResponse
    {
        public GetPlayerInfoResponse()
        {
            this.PlayerInRoom = null;
            //this.PlayerRank = 9999;
            this.FailReason = "";
            this.PlayerIdentity = 0;
            //this.CurrentShipAId = 0;
            //this.CurrentBurstAId = 0;
            //this.CurrentFaceAId = 0;
            //this.CurrentMotherAId = 0;
            //this.PlayerEquipmentContents = new List<PlayerEquipmentContent>();
            this.Name = "";
            this.FaceUrl = "";
        }
        public PlayerInRoom PlayerInRoom { get; set; }
        public string FailReason { get; set; }

        public int PlayerIdentity { get; set; }
        public string Name { get; set; }
        public string FaceUrl { get; set; }

        public override void FillDatas(string json)
        {
            CLocalNetMsg msg = new CLocalNetMsg(json);
            this.FailReason = msg.GetString("FailReason");
            this.PlayerIdentity = msg.GetInt("PlayerIdentity");
            this.Name = msg.GetString("Name");
            this.FaceUrl = msg.GetString("FaceUrl");

            CLocalNetMsg pirMsg = msg.GetNetMsg("PlayerInRoom");
            PlayerInRoom = new PlayerInRoom();
            PlayerInRoom.WorldRank = pirMsg.GetInt("WorldRank");
            PlayerInRoom.Exp = pirMsg.GetLong("Exp");
            PlayerInRoom.WinTimes = pirMsg.GetInt("WinTimes");


            PlayerInRoom.Broadcaster = new Broadcaster();
            CLocalNetMsg bcMsg = pirMsg.GetNetMsg("Broadcaster");
            PlayerInRoom.Broadcaster.UpId = bcMsg.GetString("UpId");
            PlayerInRoom.Broadcaster.CName = bcMsg.GetString("CName");
            PlayerInRoom.Broadcaster.FaceUrl = bcMsg.GetString("FaceUrl");
            PlayerInRoom.Broadcaster.Salt = bcMsg.GetString("Salt");
            PlayerInRoom.Broadcaster.LastHeartBeat = (DateTime)Convert.ChangeType(bcMsg.GetString("LastHeartBeat"), typeof(DateTime));
            PlayerInRoom.Broadcaster.Exp = bcMsg.GetLong("Exp");
            PlayerInRoom.Broadcaster.RankScore = bcMsg.GetInt("RankScore");
            PlayerInRoom.Broadcaster.TotalSpending = bcMsg.GetLong("TotalSpending");
            PlayerInRoom.Broadcaster.UId = bcMsg.GetString("UId");
            PlayerInRoom.Broadcaster.BroadcasterState = (BroadcasterState)bcMsg.GetInt("BroadcasterState");

            PlayerInRoom.Player = new Player();
            CLocalNetMsg playerMsg = pirMsg.GetNetMsg("Player");
            PlayerInRoom.Player.UId = playerMsg.GetString("UId");
            PlayerInRoom.Player.UserCName = playerMsg.GetString("UserCName");
            PlayerInRoom.Player.UserFace = playerMsg.GetString("UserFace");
            PlayerInRoom.Player.Spending = playerMsg.GetLong("Spending");
            PlayerInRoom.Player.Identity = playerMsg.GetInt("Identity");
            PlayerInRoom.Player.ExpireDate = (DateTime)Convert.ChangeType(playerMsg.GetString("ExpireDate"), typeof(DateTime));
            PlayerInRoom.Player.PlayerState = (PlayerState)playerMsg.GetInt("PlayerState");

        }
    }

    public class GetSinglePlayerInfoResponse: ABJsonDeSerializeObject, IBaseResponse
    {
        public GetSinglePlayerInfoResponse()
        {
            result = null;
            this.FailReason = "";
        }
        public WorldRankPlayerInfoContent result { get; set; }
        public string FailReason { get; set; }

        public override void FillDatas(string json)
        {
            CLocalNetMsg msg = new CLocalNetMsg(json);
            this.FailReason = msg.GetString("FailReason");

            CLocalNetMsg resultMsg = msg.GetNetMsg("result");
            result = new WorldRankPlayerInfoContent();
            //result.N = resultMsg.GetString("N");
            //result.F = resultMsg.GetString("F");
            result.E = resultMsg.GetLong("E");
            result.U = resultMsg.GetString("U");
        }
    }
    public class GetTopRanksListResponse:ABJsonDeSerializeObject, IBaseResponse
    {
        public GetTopRanksListResponse()
        {
            result = null;
            this.FailReason = "";
        }
        public List<WorldRankPlayerInfoContent> result { get; set; }
        public string FailReason { get; set; }

        public override void FillDatas(string json)
        {
            CLocalNetMsg msg = new CLocalNetMsg(json);
            this.FailReason = msg.GetString("FailReason");
            result = new List<WorldRankPlayerInfoContent>();
            CLocalNetArrayMsg array = msg.GetNetMsgArr("result");
            for (int i = 0; i < array.GetSize(); i++) {
                WorldRankPlayerInfoContent item = new WorldRankPlayerInfoContent();
                CLocalNetMsg child = array.GetNetMsg(i);
                //item.N = child.GetString("N");
                //item.F = child.GetString("F");
                item.E = child.GetLong("E");
                item.U = child.GetString("U");
                result.Add(item);
            }
        }
    }

    public class HeartBeatResponse: ABJsonDeSerializeObject, IBaseResponse
    {
        public HeartBeatResponse()
        {
            this.Token = "";
            this.FailReason = "";
            //this.DailyQuestTimesRemain = 0;
        }
        public string Token { get; set; }
        public string FailReason { get; set; }

        public override void FillDatas(string json)
        {
            CLocalNetMsg msg = new CLocalNetMsg(json);
            this.Token = msg.GetString("Token");
            this.FailReason = msg.GetString("FailReason");
        }
        //public int DailyQuestTimesRemain { get; set; }
    }
    public class PlayerSpendingRespons: IBaseResponse
    {
        public PlayerSpendingRespons()
        {
            this.FailReason = "";
        }
        public string FailReason { get; set; }
    }

    public class PlayersSpendingsResponse: IBaseResponse
    {
        public PlayersSpendingsResponse()
        {
            this.FailReason = "";
            this.BroadcasterExp = 0;
        }
        public string FailReason { get; set; }
        public long BroadcasterExp { get; set; }
    }

    public class PlayerGetPointsResponse: IBaseResponse
    {
        public string FailReason { get; set; }
        public long TotalPoints { get; set; }
        public PlayerGetPointsResponse()
        {
            this.FailReason = "";
            this.TotalPoints = 0;
        }
    }
}
