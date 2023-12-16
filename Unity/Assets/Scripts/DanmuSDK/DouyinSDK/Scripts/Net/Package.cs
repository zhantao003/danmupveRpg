using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DouyinDanmu
{
    public struct Package
    {
        public string szJsonData;

        public Package(byte[] bytes)
        {
            //var headerBuffer = new ArraySegment<byte>(bytes, 0, PackageHeader.KPacketHeaderLength);
            //Header = new PackageHeader(headerBuffer);
            //var body = new ArraySegment<byte>(bytes, PackageHeader.KPacketHeaderLength, (int)Header.PackageLength).ToArray();
            //PacketBody = body;

            szJsonData = Encoding.UTF8.GetString(bytes);
        }

        #region ´«ËÍÏûÏ¢

        #endregion
    }
}

