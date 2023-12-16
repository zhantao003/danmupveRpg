using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DouyuDanmu
{
    public struct Package
    {
        public PackageHeader Header;

        private static readonly Package s_NoBodyHeartBeatPacket = new Package()
        {
            Header = new PackageHeader()
            {
                Header = -9555,
                ProtoType = DouyuMessageOpcode.HeartBeat,
                PackageLength = 0
            }
        };

        public uint Length => Header.PackageLength;

        public byte[] PacketBody;

        public Package(byte[] bytes)
        {
            var headerBuffer = new ArraySegment<byte>(bytes, 0, PackageHeader.KPacketHeaderLength);
            Header = new PackageHeader(headerBuffer);
            var body = new ArraySegment<byte>(bytes, PackageHeader.KPacketHeaderLength, (int)Header.PackageLength).ToArray();
            PacketBody = body;
        }

        public byte[] ToBytes
        {
            get
            {
                if (PacketBody != null)
                    Header.PackageLength = (uint)PacketBody.Length;
                else
                    Header.PackageLength = 0;

                var arr = new byte[Header.PackageLength + PackageHeader.KPacketHeaderLength];

                try
                {
                    //#if UNITY_2021_2_OR_NEWER || NET5_0_OR_GREATER
                    //                    Array.Copy(((ReadOnlySpan<byte>)Header).ToArray(), arr, Header.Header);
                    //#else
                    //                                        byte[] arrHeaderBytes = (byte[])Header;
                    //                                        Array.Copy(arrHeaderBytes, arr, PackageHeader.KPacketHeaderLength);
                    //#endif

                    byte[] arrHeaderBytes = (byte[])Header;
                    Array.Copy(arrHeaderBytes, arr, PackageHeader.KPacketHeaderLength);

                    if (PacketBody != null)
                        Array.Copy(PacketBody, 0, arr, PackageHeader.KPacketHeaderLength, PacketBody.Length);
                }
                catch(Exception e)
                {
                    Debug.LogError(e.Message);
                }

                return arr;
            }
        }

        #region 传送消息

        public static Package HeartBeat(string token)
        {
            HeartBeat pMsgHeart = new HeartBeat()
            {
                Token = token,
                MilliTime = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds,
                Interval = 30
            };

            byte[] array = ProtobufHelper.ToBytes(pMsgHeart);

            if (array == null) return s_NoBodyHeartBeatPacket;

            Debug.Log("发送心跳包!" + "\r\nToken：" + token + "\r\n时间戳：" + pMsgHeart.MilliTime + "\r\n间隔：" + pMsgHeart.Interval);

            return new Package()
            {
                Header = new PackageHeader()
                {
                    Header = -9555,
                    ProtoType = DouyuMessageOpcode.HeartBeat,
                    PackageLength = (uint)array.Length
                },

                PacketBody = array
            };
        }

        #endregion
    }
}

