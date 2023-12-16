using System.Collections.Generic;

namespace SharedLibrary
{
    public class AuthenticationRequest: ABJsonSerializeObject
    {
        public AuthenticationRequest(string upId, string bCName, string bFace, string version, string UId)
        {
            this.UpId = upId;
            this.BCName = bCName;
            this.BFace = bFace;
            this.Version = version;
            this.UId = UId;
        }
        private AuthenticationRequest()
        {
        }

        public string UpId { get; set; }
        public string BCName { get; set; }
        public string BFace { get; set; }
        public string Version { get; set; }
        public string UId { get; set; }

        public override CLocalNetMsg GetJsonMsg()
        {
            CLocalNetMsg msg = new CLocalNetMsg();
            msg.SetString("UpId", UpId);
            msg.SetString("BCName", BCName);
            msg.SetString("BFace", BFace);
            msg.SetString("Version", Version);
            msg.SetString("UId", UId);
            return msg;
        }
        //public string Password { get; set; }
    }

    public class BroadcasterEXPGainRequest : ABJsonSerializeObject
    {
        public BroadcasterEXPGainRequest(long expGain)
        {
            this.ExpGain = expGain;
        }
        private BroadcasterEXPGainRequest() { }
        public long ExpGain { get; set; }

        public override CLocalNetMsg GetJsonMsg()
        {
            CLocalNetMsg msg = new CLocalNetMsg();
            msg.SetLong("ExpGain", ExpGain);
            return msg;
        }
    }

    public class GameResultRequest: ABJsonSerializeObject
    {
        public GameResultRequest(List<GameResultContent> gameResultContents)
        {
            this.GameResultContents = gameResultContents;
        }
        protected GameResultRequest() { }
        public List<GameResultContent> GameResultContents { get; set; }

        public override CLocalNetMsg GetJsonMsg()
        {
            CLocalNetMsg msg = new CLocalNetMsg();
            CLocalNetArrayMsg array = new CLocalNetArrayMsg();
            for (int i = 0; i < GameResultContents.Count; i++)
            {
                CLocalNetMsg msgChild = new CLocalNetMsg();
                msgChild.SetString("UId", GameResultContents[i].UId);
                msgChild.SetLong("ExpGain", GameResultContents[i].ExpGain);
                msgChild.SetBoolAsInt("Win", GameResultContents[i].Win);
                array.AddMsg(msgChild);
            }
            msg.SetNetMsgArr("GameResultContents",array);
            return msg;
        }
    }
    public class GameResultContent
    {
        public GameResultContent(string uId, long expGain)
        {
            this.UId = uId;
            this.ExpGain = expGain;
            this.Win = false;
        }
        private GameResultContent() { }
        public string UId { get; set; }
        public long ExpGain { get; set; }
        public bool Win { get; set; }
    }

    public class GetPlayerInfoRequest: ABJsonSerializeObject
    {
        public GetPlayerInfoRequest(string uId, string cName, string faceUrl,long vipLv/*, bool useEquipment*/)
        {
            this.UId = uId;
            this.CName = cName;
            this.FaceUrl = faceUrl;
            this.VipLv = vipLv;
        }
        private GetPlayerInfoRequest() { }
        public string UId { get; set; }
        public string CName { get; set; }
        public string FaceUrl { get; set; }
        public long VipLv { get; set; }

        public override CLocalNetMsg GetJsonMsg()
        {
            CLocalNetMsg msg = new CLocalNetMsg();
            msg.SetString("UId", UId);
            msg.SetString("CName", CName);
            msg.SetString("FaceUrl", FaceUrl);
            msg.SetLong("VipLv", VipLv);
            return msg;
        }
    }

    public class GetSinglePlayerInfoRequest: ABJsonSerializeObject
    {
        public string uid { get; set; }
        public GetSinglePlayerInfoRequest(string uid)
        {
            this.uid = uid;
        }

        public override CLocalNetMsg GetJsonMsg()
        {
            CLocalNetMsg msg = new CLocalNetMsg();
            msg.SetString("uid", uid);
            return msg;
        }
    }

    public class GetTopRanksListRequest: ABJsonSerializeObject
    {
        public int ListCount { get; set; }
        public GetTopRanksListRequest(int listCount)
        {
            ListCount = listCount;
        }

        public override CLocalNetMsg GetJsonMsg()
        {
            CLocalNetMsg msg = new CLocalNetMsg();
            msg.SetInt("ListCount", ListCount);
            return msg;
        }
    }

    public class HeartBeatRequest: ABJsonSerializeObject
    {
        public HeartBeatRequest(string bCName, string bFace, string version, string UId)
        {
            this.BCName = bCName;
            this.BFace = bFace;
            this.Version = version;
            this.UId = UId;
        }
        private HeartBeatRequest() { }
        public string BCName { get; set; }
        public string BFace { get; set; }
        public string Version { get; set; }
        public string UId { get; set; }

        public override CLocalNetMsg GetJsonMsg()
        {
            CLocalNetMsg msg = new CLocalNetMsg();
            msg.SetString("BCName", BCName);
            msg.SetString("BFace", BFace);
            msg.SetString("Version", Version);
            msg.SetString("UId", UId);
            return msg;
        }
    }

    public class PlayerSpendingRequest: ABJsonSerializeObject
    {
        public string UpId { get; set; }
        public string UId { get; set; }
        public string CName { get; set; }
        public string FaceUrl { get; set; }
        public long Spending { get; set; }
        public PlayerSpendingRequest(string upId, string uId, string cName, string faceUrl, long spending)
        {
            this.UpId = upId;
            this.UId = uId;
            this.CName = cName;
            this.FaceUrl = faceUrl;
            this.Spending = spending;
        }

        public override CLocalNetMsg GetJsonMsg()
        {
            CLocalNetMsg msg = new CLocalNetMsg();
            msg.SetString("UpId", UpId);
            msg.SetString("UId", UId);
            msg.SetString("CName", CName);
            msg.SetString("FaceUrl", FaceUrl);
            msg.SetLong("Spending", Spending);
            return msg;
        }
    }

    public class PlayersSpendingsRequest : ABJsonSerializeObject
    {
        public List<PlayerSpendingRequest> PlayerSpendingRequests { get; set; }
        public PlayersSpendingsRequest(){
            this.PlayerSpendingRequests = new List<PlayerSpendingRequest>();
        }

        public override CLocalNetMsg GetJsonMsg()
        {
            CLocalNetMsg msg = new CLocalNetMsg();
            CLocalNetArrayMsg array = new CLocalNetArrayMsg();
            for (int i = 0; i < PlayerSpendingRequests.Count; i++)
            {
                CLocalNetMsg msgChild = new CLocalNetMsg();
                msg.SetString("UpId", PlayerSpendingRequests[i].UpId);
                msg.SetString("UId", PlayerSpendingRequests[i].UId);
                msg.SetString("CName", PlayerSpendingRequests[i].CName);
                msg.SetString("FaceUrl", PlayerSpendingRequests[i].FaceUrl);
                msg.SetLong("Spending", PlayerSpendingRequests[i].Spending);
                array.AddMsg(msgChild);
            }
            msg.SetNetMsgArr("PlayerSpendingRequests", array);
            return msg;
        }
    }

    public class SetPlayerStateRequest: ABJsonSerializeObject
    {
        public string Key { get; set; }
        public string UID { get; set; }
        public PlayerState PlayerState { get; set; }
        public string FailReason { get; set; }

        public override CLocalNetMsg GetJsonMsg()
        {
            CLocalNetMsg msg = new CLocalNetMsg();
            msg.SetString("Key", Key);
            msg.SetString("UID", UID);
            msg.SetInt("PlayerState", (int)PlayerState);
            msg.SetString("FailReason", FailReason);
            return msg;
        }
    }
    public class SetBroadcasterStateRequest: ABJsonSerializeObject
    {
        public string Key { get; set; }
        public string UpId { get; set; }
        public BroadcasterState BroadcasterState { get; set; }
        public string FailReason { get; set; }

        public override CLocalNetMsg GetJsonMsg()
        {
            CLocalNetMsg msg = new CLocalNetMsg();
            msg.SetString("Key", Key);
            msg.SetString("UpId", UpId);
            msg.SetInt("BroadcasterState", (int)BroadcasterState);
            msg.SetString("FailReason", FailReason);
            return msg;
        }
    }

    public class PlayerGetPointsRequest: ABJsonSerializeObject
    {
        public string UId { get; set; }
        public long Gain { get; set; }
        public PlayerGetPointsRequest(string uId, long Gain)
        {
            this.UId = uId;
            this.Gain = Gain;
        }

        public override CLocalNetMsg GetJsonMsg()
        {
            CLocalNetMsg msg = new CLocalNetMsg();
            msg.SetString("UId", UId);
            msg.SetLong("Gain", Gain);
            return msg;
        }
    }

    public class GetRoomRanksRequest : ABJsonSerializeObject
    {
        public int GetCount { get; set; }
        public GetRoomRanksRequest(int GetCount = 100) {
            this.GetCount = GetCount;
        }

        public override CLocalNetMsg GetJsonMsg()
        {
            CLocalNetMsg msg = new CLocalNetMsg();
            msg.SetInt("GetCount", GetCount);
            return msg;
        }
    }
}
