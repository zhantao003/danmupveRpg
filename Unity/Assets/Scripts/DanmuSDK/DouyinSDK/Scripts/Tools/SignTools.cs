using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace DouyinDanmu
{
    public static class SignTools
    {
        public static Dictionary<string, string> SignParams(Dictionary<string, string> dicParams, string secret)
        {
            //��������
            Dictionary<string, string> sortDic = dicParams.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            string szReqPamamsContent = "";
            foreach (var item in sortDic)
            {
                szReqPamamsContent += item.Key + "=" + item.Value + "&";
            }
            szReqPamamsContent += secret;

            //MD5
            string szSign = Md5(szReqPamamsContent);
            sortDic.Add("sign", szSign);

            return sortDic;
        }

        /// <summary>
        /// MD5����
        /// </summary>
        public static string Md5(this string source)
        {
            //MD5���ǳ�����
            MD5 md5 = MD5.Create();
            //��Ҫ���ַ���ת���ֽ�����
            byte[] buffer = Encoding.UTF8.GetBytes(source);
            //���ܺ���һ���ֽ����͵����飬����Ҫע�����UTF8/Unicode�ȵ�ѡ��
            byte[] md5Buffer = md5.ComputeHash(buffer);
            // ͨ��ʹ��ѭ�������ֽ����͵�����ת��Ϊ�ַ��������ַ����ǳ����ַ���ʽ������
            return md5Buffer.Aggregate<byte, string>(null, (current, b) => current + b.ToString("x2"));
        }
    }
}