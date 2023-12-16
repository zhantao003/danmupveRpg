using System.Collections.Generic;
using UnityEngine;

namespace SharedLibrary
{
    public class GameResultResponse :ABJsonDeSerializeObject, IBaseResponse
    {
        public GameResultResponse() {
            //this.WorldTopPlayers = new List<PlayerInfoContent>();
            this.CurrentPlayersRanks = new List<GameResultResponseCPRContent>();
            this.FailReason = "";
        }
        public List<WorldRankPlayerInfoContent> WorldTopPlayers { get; set; }
        public List<GameResultResponseCPRContent> CurrentPlayersRanks { get; set; }
        public string FailReason { get; set; }

        public override void FillDatas(string json)
        {
            CLocalNetMsg msg = new CLocalNetMsg(json);
            this.FailReason = msg.GetString("FailReason");

            CLocalNetArrayMsg arrayWorldTop = msg.GetNetMsgArr("WorldTopPlayers");
            this.WorldTopPlayers = new List<WorldRankPlayerInfoContent>();
            for (int i = 0; i < arrayWorldTop.GetSize(); i++)
            {
                WorldRankPlayerInfoContent item = new WorldRankPlayerInfoContent();
                CLocalNetMsg child = arrayWorldTop.GetNetMsg(i);
                //item.N = child.GetString("N");
                //item.F = child.GetString("F");
                item.E = child.GetLong("E");
                item.U = child.GetString("U");
                WorldTopPlayers.Add(item);
            }

            CLocalNetArrayMsg arrayCurrentPlayers = msg.GetNetMsgArr("CurrentPlayersRanks");
            this.CurrentPlayersRanks = new List<GameResultResponseCPRContent>();
            for (int i = 0; i < arrayCurrentPlayers.GetSize(); i++)
            {
                GameResultResponseCPRContent item = new GameResultResponseCPRContent();
                CLocalNetMsg child = arrayCurrentPlayers.GetNetMsg(i);
                item.UId = child.GetString("UId");
                item.WinTimes = child.GetInt("WinTimes");
                item.Exp = child.GetLong("Exp");
                CurrentPlayersRanks.Add(item);
            }
        }
    }

    public class WorldRankPlayerInfoContent
    {
        public WorldRankPlayerInfoContent()
        {
            //this.N = "";
            //this.F = "";
            this.E = 0;
            this.U = "";
        }
        //public string N { get; set; }
        //public string F { get; set; }
        public long E { get; set; }
        public string U { get; set; }
    }

    public class WorldRankCombineSamePointRank {
        public string U { get; set; }
        public int Rank { get; set; }
        public WorldRankCombineSamePointRank() 
        {
        }
    }

    public class GameResultResponseCPRContent {
        public GameResultResponseCPRContent() {
            this.UId = "";
            //this.Rank = 0;
            this.WinTimes = 0;
            this.Exp = 0;
        }
        public string UId { get; set; }
        //public long Rank { get; set; }
        public int WinTimes { get; set; }
        public long Exp { get; set; }
    }
}
