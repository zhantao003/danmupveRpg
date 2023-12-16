using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class PlayerInRoom
    {
        public PlayerInRoom(Broadcaster broadcaster, Player player)
        {
            this.Player = player;
            this.Broadcaster = broadcaster;
            this.Exp = 0;
            this.WinTimes = 0;
            this.WorldRank = 9999;
        }
        public PlayerInRoom() { }
        public int Id { get; set; }
        //绑定最后一次登录的主播
        public Broadcaster Broadcaster { get; set; }
        public Player Player { get; set; }
        //经验
        public long Exp { get; set; }
        public int WinTimes { get; set; }

        public int WorldRank { get; set; }
    }
}
