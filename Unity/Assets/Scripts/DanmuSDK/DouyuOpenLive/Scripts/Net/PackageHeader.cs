using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouyuDanmu
{
    public struct PackageHeader 
    {
        public const int KPacketHeaderLength = 7;

        public short Header;
        public sbyte ProtoType;
        public uint PackageLength;

        public PackageHeader(ArraySegment<byte> bytes)
        {
            if (bytes.Count < KPacketHeaderLength) throw new ArgumentException("No Supported Protocol Header");

            byte[] b = bytes.Array;
            Header = EndianBitConverter.BigEndian.ToInt16(b, 0);
            ProtoType = (sbyte)b[2];
            PackageLength = EndianBitConverter.BigEndian.ToUInt32(b, 3);
        }

        /// <summary>
        /// ���ɵ�ĻЭ���ͷ��
        /// </summary>
        /// <returns>����Ӧ�ĵ�Ļͷ��byte����</returns>
        public static explicit operator byte[](PackageHeader header) => GetBytes(header.Header,
            header.ProtoType, header.PackageLength);

        public static byte[] GetBytes(short headerLength, sbyte prototype, uint packagelen)
        {
            var bytes = new byte[KPacketHeaderLength];
            try
            {
                byte[] h1 = EndianBitConverter.BigEndian.GetBytes(headerLength);
                byte[] p1 = new byte[1];
                p1[0] = (byte)prototype;
                byte[] pkl = EndianBitConverter.BigEndian.GetBytes(packagelen);
                //Debug.Log("ͷ����" + h1.Length + " Э�鳤��" + p1.Length + " �峤��" + pkl.Length);
                Buffer.BlockCopy(h1, 0, bytes, 0, h1.Length);
                Buffer.BlockCopy(p1, 0, bytes, h1.Length, p1.Length);
                Buffer.BlockCopy(pkl, 0, bytes, h1.Length + p1.Length, pkl.Length);
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
           
            return bytes;
        }
    }
}

