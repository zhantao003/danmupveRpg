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
            //传参排序
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
        /// MD5加密
        /// </summary>
        public static string Md5(this string source)
        {
            //MD5类是抽象类
            MD5 md5 = MD5.Create();
            //需要将字符串转成字节数组
            byte[] buffer = Encoding.UTF8.GetBytes(source);
            //加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择
            byte[] md5Buffer = md5.ComputeHash(buffer);
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            return md5Buffer.Aggregate<byte, string>(null, (current, b) => current + b.ToString("x2"));
        }
    }
}